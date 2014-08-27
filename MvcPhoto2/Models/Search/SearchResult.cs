using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.Models
{
    public class SearchResultShoots
    {
        public string name { get; set; }
        public string shootHref { get; set; }
        public List<SearchResultsPhotos> ListOfPhotos { get; set; }

    }
    public class SearchResultsPhotos
    {
        public string photoHref { get; set; }
        public string photoSrc { get; set; }
        public string fileName { get; set; }
    }
}