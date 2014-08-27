using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoto2.Models.ViewModels;
using MvcPhoto2.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MvcPhoto2.Controllers
{
    public class photosController : Controller
    {
        //
        // GET: /photos/
        //get the repo that in order to read data from categories to find the correct directories.
        private Photo2repo repo = new Photo2repo();
        

        public string publicUrl = "";
        public ActionResult Index(string id)
        {
            Debug.WriteLine("This is my output.");
            publicUrl = id;
            string[] words = id.Split('\\');
            //create a view model to display all the data.
            shootsViewModel vm = new shootsViewModel();
            
            vm.CategoryName = words[words.Count()-1];
            vm.allShoots = repo.getAllShoots(id);
            return View(vm);
        }
        [Authorize(Roles = "Photographer")]
        public ActionResult addtoEmail(string title, string url, int id)
        {
            EmailCart emailcart = new EmailCart();
            emailcart.photoShootName = title;
            emailcart.link = url;
            emailcart.netId = User.Identity.Name;
            repo.AddToEmail(emailcart);
            string[] splitUrl = Regex.Split(url, "/" + title);
            
            url = Server.MapPath(splitUrl[0]); 
            
            return Redirect("/photos/index?id=" + url);
        }
        

    }
}
