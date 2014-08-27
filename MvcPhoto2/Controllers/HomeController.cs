using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MvcPhoto2.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MvcPhoto2.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index(string result)
        {
            
            TempData["Result"] = result;
            return View();
        }
        

    }
}
