using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoto2.Models;

namespace MvcPhoto2.Controllers
{ 
    public class UserController : Controller
    {
        private Photo2Entities1 db = new Photo2Entities1();

        //
        // GET: /User/
        private List<SelectListItem> role_list = new
              List<SelectListItem> {
				                     	new SelectListItem {Text = "Admin", Value = "Admin"},
				                     	new SelectListItem {Text = "Photographer", Value = "Photographer"},
				                     	
				                     };
        [Authorize(Roles = "Admin")]
        public ViewResult Index()
        {
            return View(db.Authentications.ToList());
        }

        //
        // GET: /User/Details/5
         [Authorize(Roles = "Admin")]
        public ViewResult Details(string id)
        {
            Authentication authentication = db.Authentications.Find(id);
            return View(authentication);
        }

        //
        // GET: /User/Create
         [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.selectList = role_list;
            return View();
        } 

        //
        // POST: /User/Create

        //NOT WORKING ON TYPO???????
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateUser(Authentication authentication)
        {
            if (ModelState.IsValid)
            {
                db.Authentications.Add(authentication);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(authentication);
        }
        
        //
        // GET: /User/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ViewBag.selectList = role_list;
            Authentication authentication = db.Authentications.Find(id);
            return View(authentication);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(Authentication authentication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(authentication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(authentication);
        }

        //
        // GET: /User/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            Authentication authentication = db.Authentications.Find(id);
            return View(authentication);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {            
            Authentication authentication = db.Authentications.Find(id);
            db.Authentications.Remove(authentication);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles= "Photographer")]
        public string GetNetId()
        {
            Authentication authentication = db.Authentications.Find(User.Identity.Name);
            //string Name = User.Identity.Name;
            //string Name = Byu.Utilities.LDAP.Lookup.getDetails(User.Identity.Name).FullName;
           
            string name = authentication.Name;
            
            return name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}