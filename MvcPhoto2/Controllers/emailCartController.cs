using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoto2.Models;
using System.Net.Mail;


namespace MvcPhoto2.Controllers
{ 
    public class emailCartController : Controller
    {
        private Photo2repo repo = new Photo2repo();
        //
        // GET: /emailCart/
        [Authorize(Roles = "Photographer")]
        public ViewResult Index()
        {
            return View(repo.getAllEmailItems());
        }


        //**********************
        //Email
        //**********************
        [Authorize(Roles = "Photographer")]
        public ActionResult Email(string result = "")
        {

            ViewBag.result = result;
            return View();
        }


        //**********************
        //Send Email
        //**********************
        [Authorize(Roles = "Photographer")]
        [HttpGet]
        public ActionResult SendEmail()
        {
            return PartialView();
        }



        //**********************
        //Send Email
        //**********************
        [Authorize(Roles = "Photographer")]
        [HttpPost]
        public ActionResult SendEmail(email newEmail)
        {
            List<EmailCart> allPhotoShoots = repo.getAllEmailItems();
            if (ModelState.IsValid)
            {
                List<EmailCart> clearList = new List<EmailCart>();
                string to = newEmail.to;
                string from = newEmail.from;
                MailMessage message = new MailMessage(from, to);
                message.Subject = newEmail.subject;
                message.IsBodyHtml = true;
                message.Body = newEmail.message;
                foreach (var shoots in allPhotoShoots)
                {
                    if (shoots.netId == User.Identity.Name)
                    {
                        message.Body = message.Body + "<br><br>  <a href=\"photo3.byu.edu" + shoots.link + ">" + shoots.photoShootName + "</a>";
                        clearList.Add(shoots);
                    }
                }
                SmtpClient client = new SmtpClient("gateway.byu.edu");
                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                       ex.ToString());
                }
                repo.removeAllEmailItems(clearList);
            }

            return RedirectToAction("Email", new { result = "Email has been sent." });
        }



        //**********************
        //DeleteAll
        //**********************
        [Authorize(Roles = "Photographer")]
        public ActionResult deleteAll()
        {
            List<EmailCart> clearList = new List<EmailCart>();
            foreach (var shoot in repo.getAllEmailItems())
            {
                if (shoot.netId == User.Identity.Name)
                {
                    clearList.Add(shoot);
                }
            }
            repo.removeAllEmailItems(clearList);
          return RedirectToAction("Email", new { result = "Photo shoots have been cleared." });
        }


        //**********************
        //List of shoots added
        //**********************
        [Authorize(Roles = "Photographer")]
        public ActionResult ListOfShootsAdded()
        {
            

            ViewBag.Url =  Request.Url.AbsoluteUri;
            List<EmailCart> emailCart = new List<EmailCart>();
            foreach (var email in repo.getAllEmailItems())
            {
                if (email.netId == User.Identity.Name)
                {
                    emailCart.Add(email);
                }
            }
            return PartialView(emailCart.ToList());
        }


        //**********************
        //Delete
        //**********************
        [Authorize(Roles = "Photographer")]
        public ActionResult Delete(int id, string url)
        { 
           //The URl attribute doesnt always return the correct value this needs to be fixed.
            EmailCart emailcart = repo.getEmailItem(id);
            repo.removeEmailItem(emailcart);
            return RedirectToAction("Index","Home");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
        }
    }
}