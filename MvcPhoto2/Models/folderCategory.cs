using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.Models
{
    public class folderCategory
    {
        public string categoryName { get; set; }
        public int categoryId { get; set; }
        public Nullable<int> parentId { get; set; }
        public string parentCategory { get; set; }
        public string Url { get; set; }
    }
}