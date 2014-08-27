using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoto2.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        [Authorize]
        public ActionResult LogOn()
        {
            return RedirectToAction("Index", "Home");
        }


        public ActionResult LogOff()
        {
            DotNetCasClient.CasAuthentication.SingleSignOut();
            return RedirectToAction("Index", "Home");
            //return Redirect("http://photo.byu.edu/");
        }
    }
}
