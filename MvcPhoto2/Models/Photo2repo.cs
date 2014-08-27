using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcPhoto2.Models;
using System.IO;
using System.Data;
using System.Data.Entity;
using MvcPhoto2.Models.Folders;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity.Validation;
using System.Diagnostics;
using MvcPhoto2.Models.ViewModels;
using System.Text.RegularExpressions;

namespace MvcPhoto2.Models
{
    public class Photo2repo : IPhotorepo
    {
        private Photo2Entities1 context = new Photo2Entities1();
        
       
        //Email Cart Database
        public List<EmailCart> getAllEmailItems()
        {
            return context.EmailCarts.ToList();
        }
        public EmailCart getEmailItem(int id)
        {
            return context.EmailCarts.Find(id);

        }
        public void removeEmailItem(EmailCart shoot)
        {
            context.EmailCarts.Remove(shoot);
            context.SaveChanges();
        }
        public void removeAllEmailItems(List<EmailCart> shoots)
        {
            foreach (var shoot in shoots)
            {
                    removeEmailItem(shoot);
            }
            context.SaveChanges();
        }
        public void AddToEmail(EmailCart emailcart)
        {
         
                context.EmailCarts.Add(emailcart);
                context.SaveChanges();
   
        }

        //folders Names
        public List<folderCategory> getAllFolderNames(string path, int depth = 0)
        {

            List<folderCategory> folders = new List<folderCategory>();
            if (depth <= 2)
            {
                string dash = "";
                //for (int i = 0; i < depth; i++) dash += "-"; 
                foreach (var folder in new DirectoryInfo(path).GetDirectories())
                {
                    
                    folderCategory f = new folderCategory();
                    f.categoryName = dash + folder.Name;
                    f.parentCategory = folder.Parent.Name;
                    f.Url = folder.FullName;
                    folders.Add(f);
                    int nextDepth = depth + 1;
                    List<folderCategory> children = getAllFolderNames(path + "/" + folder.Name, nextDepth);
                    folders.AddRange(children);
                }
            }
            return folders;
        }
        //All folders for json
        public List<foldersJson> GetAll()
        {
            string path = HttpContext.Current.Server.MapPath("~/Shoots");
            string firstPath = path;
            HttpRuntime.Close();
            List<foldersJson> allfolders = new List<foldersJson>();
            int id = 0;
            foreach (var f in new DirectoryInfo(path).GetDirectories())
            {
                folderAttributes fa = new folderAttributes();
                fa.href = "/photos/index?id=" + f.FullName;
                foldersJson fj = new foldersJson();
                fj.data = new foldersData();
                fj.data.title = f.Name;
                fj.data.id = 1;
                fj.data.attr = fa;
                List<foldersJson> specificList = new List<foldersJson>();
                fj.children = recursiveGetChildren(f, id, firstPath, 1);
                fj.state = (fj.children.Count() != 0) ? "open" : "";
                allfolders.Add(fj);
                id++;
            }



            return allfolders;

        }
        //recursive function
        private List<foldersJson> recursiveGetChildren(DirectoryInfo folder, int id, string folderPath, int level)
        {
            List<foldersJson> allfolders = new List<foldersJson>();
            if (level != 4)
            {
                folderPath = folderPath + @"/" + folder.Name;
                foreach (var f in new DirectoryInfo(folderPath).GetDirectories())
                {
                    folderAttributes fa = new folderAttributes();
                    fa.href = "/photos/index?id=" + f.FullName;
                    foldersJson fj = new foldersJson();

                    fj.data = new foldersData();
                    fj.data.title = f.Name;
                    fj.data.id = id;
                    fj.data.attr = fa;
                    List<foldersJson> specificList = new List<foldersJson>();
                    fj.children = recursiveGetChildren(f, id++, folderPath, level + 1);
                    fj.state = (fj.children.Count() != 0) ? "open" : "";
                    if (level == 2) fj.state = (fj.children.Count() != 0) ? "closed" : "";
                    allfolders.Add(fj);
                    id++;
                }
                return allfolders;
            }
            else
            {
                return allfolders;
            }
        }
        public List<shootList> getAllShoots(string url)
        {
            List<shootList> allShoots = new List<shootList>();
          
            string[] splitUrl = Regex.Split(url, "Shoots");
            string[] urlWords = urlWords = splitUrl[1].Split('\\'); ;
           
            int wordsCount = urlWords.Count();
            if (wordsCount == 5)
            {
                foreach (var s in new DirectoryInfo(url).GetDirectories())
                {
                    string newUrl = "/Shoots" + splitUrl[1] + "/" + s.Name;
                    if (Directory.EnumerateFileSystemEntries(s.FullName, "index.html").Any()) newUrl = newUrl + "/index.html";
                    else newUrl = newUrl + "/default.htm";
                    shootList sL = new shootList();
                    sL.href = newUrl;
                    sL.title = s.Name;
                    allShoots.Add(sL);
                }
            }
            else
            {
                foreach (var s in new DirectoryInfo(url).GetDirectories())
                {
                    allShoots.AddRange(recGetAllShoots(url + "\\" + s.Name, wordsCount + 1));
                }
            }
            return allShoots;
        }
        private List<shootList> recGetAllShoots(string url, int count)
        {
            if (count == 5)
            {
                string[] splitUrl = Regex.Split(url, "Shoots");
                List<shootList> Shoots = new List<shootList>();
                foreach (var s in new DirectoryInfo(url).GetDirectories())
                {
                    string newUrl = "/Shoots" + splitUrl[1] + "/" + s.Name;
                    if (Directory.EnumerateFileSystemEntries(s.FullName, "index.html").Any()) newUrl = newUrl + "/index.html";
                    else newUrl = newUrl + "/default.htm";
                    shootList sL = new shootList();
                    sL.href = newUrl;
                    sL.title = s.Name;
                    Shoots.Add(sL);
                }
                return Shoots;
            }
            else
            {
                List<shootList> Shoots = new List<shootList>();
                foreach (var s in new DirectoryInfo(url).GetDirectories())
                {
                    Shoots.AddRange(recGetAllShoots(url + "\\" + s.Name, count + 1));
                }
                return Shoots;
            }
        }
        


        public void Dispose()
        {
            context.Dispose();
        }
    }
}