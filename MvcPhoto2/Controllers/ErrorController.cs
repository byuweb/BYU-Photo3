using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoto2.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Error/NotAuthorized

        public ActionResult NotAuthorized()
        {
            return View();
        }

        //
        // GET: /Error/Error404

        public ActionResult Error404()
        {
            return View();
        }
        
        //
        // GET: /Error/Error500

        public ActionResult Error500()
        {
            return View();
        }

        //
        // GET: /Error/CookiesRequired

        public ActionResult CookiesRequired()
        {
            return View();
        }
    }
}
