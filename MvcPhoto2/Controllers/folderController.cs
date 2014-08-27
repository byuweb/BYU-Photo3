using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoto2.Models;
using MvcPhoto2.Models.Folders;
using MvcPhoto2.Models.ViewModels;


namespace MvcPhoto2.Controllers
{ 
    public class folderController : Controller
    {
        public JsonResult getJson()
        {
            Photo2repo repo = new Photo2repo();           
            List<foldersJson> allCategories = repo.GetAll();
            return Json(allCategories, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult displayfolder()
        {
            return PartialView();
        }

    }
}