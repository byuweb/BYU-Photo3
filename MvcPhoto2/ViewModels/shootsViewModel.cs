using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.Models.ViewModels
{
    public class shootsViewModel
    {
        public string CategoryName { get; set; }
        public int categoryId { get; set; }
        public IEnumerable<shootList> allShoots { get; set; }
    }
    public class shootList
    {
        public int id { get; set; }
        public string title { get; set; }
        public string href { get; set; }
    }
}