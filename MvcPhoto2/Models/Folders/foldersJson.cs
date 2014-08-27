using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.Models.Folders
{
    public class foldersJson
    {
        public foldersData data { get; set; }
        public string state { get; set; }
        public List<foldersJson> children { get; set; }
    }
    public class foldersData
    {
        public string title { get; set; }
        public folderAttributes attr { get; set; }
        public string state { get; set; }
        public int id { get; set; }
    }
    public class folderAttributes
    {
        public string href { get; set; }
    }
}