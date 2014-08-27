﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using MvcPhoto2.Model;

namespace MvcPhoto2.Search
{
    public static class PhotoSearch
    {
        // properties
        public static string _luceneDir =
            Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Photo_index");
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
                var hits_limit = 10000;
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
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.doc))).ToList();
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
   
        


    }

}