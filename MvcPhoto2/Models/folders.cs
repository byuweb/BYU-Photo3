using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MvcPhoto2.Models.Folders;

namespace MvcPhoto2.Models
{
    public class foldersClass
    {
        private string path;
        public foldersClass(string Shootpath)
        {
            path = HttpContext.Current.Server.MapPath(Shootpath);
        }

        public List<foldersJson> GetAll()
        {
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
                    fj.children = recursiveGetChildren(f, id++, folderPath, level+1);
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

            
    }
}