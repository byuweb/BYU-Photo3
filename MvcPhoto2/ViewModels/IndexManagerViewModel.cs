using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.ViewModels
{
    public class IndexManagerViewModel
    {
        public string PhotoShootNames { get; set; }
        public List<string> Result { get; set; }
        public List<string> ResultFail { get; set; }
    }
}