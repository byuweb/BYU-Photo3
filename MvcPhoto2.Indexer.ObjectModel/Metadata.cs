using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;

using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;

namespace MvcPhoto2.Indexer.ObjectModel
{
    public class MetaDataRepository
    {
        public Metadata Get(int id)
        {
            return GetAll().SingleOrDefault(x => x.id.Equals(id));
        }

        private string path;
        public MetaDataRepository(string p)
        {
            path = p;
        }
        public List<Metadata> GetAll()
        {


            List<Metadata> allPhotos = new List<Metadata>();
            foreach (var folder in new DirectoryInfo(path).GetDirectories())
            {
                allPhotos.AddRange(recursiveSearch(folder, 0, path));
            }

            return allPhotos;

        }
        //public List<Metadata> getUpdated(string photoShootPaths)
        public void getUpdated(string photoShootPaths)
        {
            //List<Metadata> allPhotos = new List<Metadata>();
            foreach (var folder in new DirectoryInfo(photoShootPaths).GetDirectories())
            {
                List<Metadata> allPhotos = new List<Metadata>();
                PhotoSearch.AddUpdateLuceneIndex(recursiveSearch(folder, 0, photoShootPaths));

                //allPhotos.AddRange(recursiveSearch(folder, 0, photoShootPaths));
            }

            //return allPhotos;
        }
        private List<Metadata> recursiveSearch(DirectoryInfo folder, int id, string folderPath)
        {
            List<Metadata> allPhotos = new List<Metadata>();
            if ((folder.Name == "image") || (folder.Name == "images"))
            {
                folderPath = folderPath + @"/" + folder.Name;
                string[] extensions = new[] { ".jpg", ".tiff", ".bmp" };
                int photoNumber = 1;
                FileInfo[] t = new DirectoryInfo(folderPath).GetFiles();
                foreach (FileInfo photo in new DirectoryInfo(folderPath).GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray())
                {
                    Console.WriteLine(photo.Name + "\n");
                    if (photo.Name[0] != '.')
                    {
                        allPhotos.Add(photoMetadata(photo.FullName, photo.Name, id, photoNumber));
                        photoNumber++;
                        id++;
                    }
                }
                return allPhotos;
            }
            else if (folder.GetDirectories().Count() > 0)
            {
                folderPath = folderPath + @"/" + folder.Name;
                foreach (var f in new DirectoryInfo(folderPath).GetDirectories())
                {
                    PhotoSearch.AddUpdateLuceneIndex(recursiveSearch(f, id, folderPath));
                    //allPhotos.AddRange(recursiveSearch(f, id, folderPath));
                }
                return allPhotos;
            }
            else
            {
                return allPhotos;
            }

        }
        public Metadata photoMetadata(string fullname, string name, int id, int photoNumber)
        {
            Metadata Meta = new Metadata();
            string htmlPictureFilePath = "";
            string[] path = fullname.Split('\\');
            int i = 0;
            string fullHtmlPath = "";
            for (int j = 1; j < path.Count(); j++)
            {

                if (path[j] == "Shoots")
                {
                    i = j;
                    break;
                }
                else
                {
                    fullHtmlPath += "/" + path[j];
                }
            }
            for (; i < path.Count() - 2; i++) htmlPictureFilePath += "/" + path[i];
            Meta.photoHref = htmlPictureFilePath + "/" + path[path.Count() - 2] + "/page" + photoNumber + ".html";
            Meta.description = getHtmlFromPage(fullHtmlPath + Meta.photoHref);

            Meta.fileName = name;
            Meta.id = id;
            Meta.photographer = "";
            Meta.filePath = fullname;
            Meta.shootName = path[path.Count() - 3];

            Meta.photoSrc = htmlPictureFilePath + "/" + path[path.Count() - 2] + "/" + path[path.Count() - 1];
            Meta.shootHref = htmlPictureFilePath;
            Meta.descriptionWithoutNumbers = RemoveNumbers(Meta.description);
            Meta.date = "";
            Meta.dateCategories = "";
            // Create an Image object. Get Metadata from Image

            System.GC.WaitForPendingFinalizers();
            System.Drawing.Image image = new Bitmap(fullname);
            // Get the PropertyItems property from image.
            PropertyItem[] propItems = image.PropertyItems;
            int[] propertyItemId = image.PropertyIdList;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            string tempDate = "";
            foreach (PropertyItem propItem in propItems)
            {
                //gets title
                if (propItem.Id == 270)
                {
                    Meta.description = encoding.GetString(propItem.Value);
                    Meta.descriptionWithoutNumbers = RemoveNumbers(Meta.description);
                }
                if (propItem.Id == 36867) tempDate = encoding.GetString(propItem.Value);
            }
            Meta.dateCategories = getYear(tempDate) + " " + path[path.Count() - 5] + " " + path[path.Count() - 4];
            Meta.date = ChangeDate(tempDate);
            image.Dispose();

            return Meta;

        }
        //
        // Only pulls xml because the photos sometimes dont contain very readable metadata on description.
        public string getHtmlFromPage(string htmlpath)
        {
            try
            {
                string innerText = "";
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.Load(@"P:\" + htmlpath);
                //htmlDoc.Load(htmlpath);
                if (htmlDoc.DocumentNode != null)
                {
                    HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//table");
                    innerText = bodyNode.InnerText;
                }
                return innerText;
            }
            catch
            {
                return "";
            }
        }
        //
        // gives a string that makes searches more specific
        public string RemoveNumbers(string text)
        {
            Regex regEx = new Regex("[0-9]+");
            StringBuilder sb = new StringBuilder();
            foreach (char a in text)
            {
                if (!regEx.IsMatch(a.ToString()))
                {
                    sb.Append(a);
                }
            }
            return sb.ToString();
        }
        //
        // used to change the format of the date stored in the image.
        public string ChangeDate(string text)
        {
            if (text == "") return "";
            StringBuilder sb = new StringBuilder();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] words = text.Split(delimiterChars);
            if (words[1] == "01") sb.Append("January");
            if (words[1] == "02") sb.Append("Febuary");
            if (words[1] == "03") sb.Append("March");
            if (words[1] == "04") sb.Append("April");
            if (words[1] == "05") sb.Append("May");
            if (words[1] == "06") sb.Append("June");
            if (words[1] == "07") sb.Append("July");
            if (words[1] == "08") sb.Append("August");
            if (words[1] == "09") sb.Append("September");
            if (words[1] == "10") sb.Append("October");
            if (words[1] == "11") sb.Append("November");
            if (words[1] == "12") sb.Append("December");


            sb.Append(" " + words[0]);
            return sb.ToString();
        }
        public string getYear(string text)
        {
            StringBuilder sb = new StringBuilder();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] words = text.Split(delimiterChars);
            sb.Append(words[0]);
            return sb.ToString();
        }



    }

    public class Metadata
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public string description { get; set; }
        public string descriptionWithoutNumbers { get; set; }
        public string photographer { get; set; }
        public string filePath { get; set; }
        public string shootName { get; set; }
        public string photoHref { get; set; }
        public string shootHref { get; set; }
        public string date { get; set; }
        public string dateCategories { get; set; }
        public bool isChecked { get; set; }
        public string photoSrc { get; set; }
    }
}
