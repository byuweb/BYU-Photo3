using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoto2.Models.ViewModels
{
    public class yearsViewModel
    {
        public IEnumerable<SelectListItem> years { get; set; }
        public IEnumerable<SelectListItem> categories { get; set; }
        public int categoryId { get; set; }
        public int beginYear { get; set; }
        public int endyear { get; set; }
    }
}