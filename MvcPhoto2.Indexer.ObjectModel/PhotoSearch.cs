using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

using System.Drawing;
using System.Drawing.Imaging;

namespace MvcPhoto2.Indexer.ObjectModel
{
    public static class PhotoSearch
    {
        // properties
        public static string _luceneDir =
            Path.Combine(PathsClass.ServerPath, "Photo_index");


        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }


        // search methods
        public static IEnumerable<Metadata> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<Metadata>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Close();
            reader.Dispose();
            searcher.Close();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }
        public static IEnumerable<Metadata> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<Metadata>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }
        public static IEnumerable<Metadata> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<Metadata>() : _search(input, fieldName);
        }


        // main search method
        private static IEnumerable<Metadata> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<Metadata>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_29, new[] { "id", "fileName", "description", "photographer", "filePath", "shootName", "photoHref", "shootHref", "descriptionWithoutNumbers", "date", "dateCategories", "photoSrc" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    //var hits = searcher.Search(query, null, hits_limit, Sort.INDEXORDER).ScoreDocs;
                    var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }


        // map Lucene search index to data
        private static IEnumerable<Metadata> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<Metadata> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }
        private static Metadata _mapLuceneDocumentToData(Document doc)
        {
            return new Metadata
            {
                id = Convert.ToInt32(doc.Get("id")),
                fileName = doc.Get("fileName"),
                description = doc.Get("description"),
                photographer = doc.Get("photographer"),
                filePath = doc.Get("filePath"),
                shootName = doc.Get("shootName"),
                photoHref = doc.Get("photoHref"),
                shootHref = doc.Get("shootHref"),
                descriptionWithoutNumbers = doc.Get("descriptionWithoutNumbers"),
                date = doc.Get("date"),
                dateCategories = doc.Get("dateCategories"),
                photoSrc = doc.Get("photoSrc"),
                isChecked = false

            };
        }


        // add/update/clear search index data 
        public static void AddUpdateLuceneIndex(Metadata Metadata)
        {
            AddUpdateLuceneIndex(new List<Metadata> { Metadata });
        }
        public static void AddUpdateLuceneIndex(IEnumerable<Metadata> Metadatas)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entries if any)
                foreach (var Metadata in Metadatas) _addToLuceneIndex(Metadata, writer);

                // close handles
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }
        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Close();
                writer.Dispose();
            }
        }
        private static void _addToLuceneIndex(Metadata Metadata, IndexWriter writer)
        {
            // remove older index entry
            //var searchQuery = new TermQuery(new Term("id", Metadata.id.ToString()));
            //writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("id", Metadata.id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("fileName", Metadata.fileName, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("description", Metadata.description, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("photographer", Metadata.photographer, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("filePath", Metadata.filePath, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("shootName", Metadata.shootName, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("photoHref", Metadata.photoHref, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("shootHref", Metadata.shootHref, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("descriptionWithoutNumbers", Metadata.descriptionWithoutNumbers, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("date", Metadata.date, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("dateCategories", Metadata.dateCategories, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("photoSrc", Metadata.photoSrc, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }



    }
}
