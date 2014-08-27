using System.Collections.Generic;
using System.Web.Mvc;
using MvcPhoto2.Model;
using MvcPhoto2.Models;

namespace MvcPhoto2.ViewModels
{
    public class PhotoIndexViewModel
    {
        public Metadata metaData { get; set; }
        //public IEnumerable<Metadata> AllMetaData { get; set; }
        public IEnumerable<Metadata> AllSearchIndexData { get; set; }

        public List<SearchResultShoots> SampleSearchResults { get; set; }
        public IList<SelectListItem> SearchFieldList { get; set; }
        public IList<SelectListItem> yearFieldList { get; set; }
        public IList<SelectListItem> GeneralCategoryFieldList { get; set; }
        public IList<SelectListItem> SecondCategoryFieldList { get; set; }
        public string SearchTerm { get; set; }
        public string SearchField { get; set; }
        public string GeneralCategory { get; set; }
        public string SecondCategory { get; set; }
        public string Year { get; set; }
        public string type { get; set; }
        public int PageIndex { get; set; }
        public int nextPage { get; set; }
        public int pageNumber { get; set; }
        public int totalPages { get; set; }
    }
}