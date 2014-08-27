using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoto2.Models
{
    public partial class mailOrder
    {
        public int OrderId { get; set; }
        public string Email { get; set; }
        public string Links { get; set; }
        public decimal Total { get; set; }
    }
}