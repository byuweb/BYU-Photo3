using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MvcPhoto2.Search;
using MvcPhoto2.ViewModels;
using System.Drawing.Imaging;
using System.Drawing;

using System;
using MvcPhoto2.Model;
using System.Xml.Linq;
using System.Xml;
using MvcPhoto2.Models;
using System.Text.RegularExpressions;

namespace MvcPhoto2.Controllers
{
    public class PhotoSearchController : Controller
    {
        //private IEnumerable<Metadata> _searchResults;
        private IEnumerable<Metadata> _searchResults;
        MetaDataRepository objData;
        public PhotoSearchController()
        {
            string path = "~/Shoots";
            objData = new MetaDataRepository(path);
        }
        [Authorize(Roles = "Photographer")]
        public ActionResult Index(string searchTerm, string searchField, string type , string checkbox, int PageIndex = -1)
        {

            int totalResults = 0;
            if (!Directory.Exists(PhotoSearch._luceneDir)) Directory.CreateDirectory(PhotoSearch._luceneDir);
            // perform Lucene search
            if (string.IsNullOrEmpty(type))
                _searchResults = string.IsNullOrEmpty(searchField)
                                    ? PhotoSearch.Search(searchTerm)
                                    : PhotoSearch.Search(searchTerm, searchField);
            else if (type == "default")
                _searchResults = string.IsNullOrEmpty(searchField)
                                    ? PhotoSearch.SearchDefault(searchTerm)
                                    : PhotoSearch.SearchDefault(searchTerm, searchField);

           
            List<SearchResultShoots> searchResultShoots = new List<SearchResultShoots>();
            foreach (var searchContent in _searchResults)
            {
                if (searchResultShoots.Count == 0)
                {
                    SearchResultsPhotos photo = new SearchResultsPhotos();
                    photo.fileName = searchContent.fileName;
                    photo.photoSrc = searchContent.photoSrc;
                    photo.photoHref = searchContent.photoHref;

                    SearchResultShoots shoot = new SearchResultShoots();
                    shoot.name = searchContent.shootName;
                    shoot.shootHref = searchContent.shootHref;
                    shoot.ListOfPhotos = new List<SearchResultsPhotos>();
                    shoot.ListOfPhotos.Add(photo);
                    searchResultShoots.Add(shoot);
                }
                else
                {
                    bool Found = false;
                    foreach (var resultsAlreadyIn in searchResultShoots)
                    {
                        if (resultsAlreadyIn.name == searchContent.shootName)
                        {
                            SearchResultsPhotos photo = new SearchResultsPhotos();
                            photo.fileName = searchContent.fileName;
                            photo.photoSrc = searchContent.photoSrc;
                            photo.photoHref = searchContent.photoHref;
                            resultsAlreadyIn.ListOfPhotos.Add(photo);
                            Found = true;
                        }
                    }
                    if (!Found)
                    {
                        SearchResultsPhotos photo = new SearchResultsPhotos();
                        photo.fileName = searchContent.fileName;
                        photo.photoSrc = searchContent.photoSrc;
                        photo.photoHref = searchContent.photoHref;

                        SearchResultShoots shoot = new SearchResultShoots();
                        shoot.name = searchContent.shootName;
                        shoot.shootHref = searchContent.shootHref;
                        shoot.ListOfPhotos = new List<SearchResultsPhotos>();
                        shoot.ListOfPhotos.Add(photo);
                        searchResultShoots.Add(shoot);
                    }
                }
                totalResults++;
            }
            if (checkbox != "on") //shoot seach
                return View(new PhotoIndexViewModel
                {
                    SampleSearchResults = searchResultShoots,
                    SearchTerm = searchTerm,
                    AllSearchIndexData = _searchResults
                });
            else // photo search
            {
                bool endResults = false;
                int tPage = totalResults;

                if (totalResults > 0) tPage = totalResults / 50;
                if (PageIndex < 0) PageIndex = 0;
                else PageIndex += 50;
                if (PageIndex >= totalResults) tPage = PageIndex;

                return View("PhotoSearchResults",
                    new PhotoIndexViewModel { 
                        SampleSearchResults = searchResultShoots, 
                        SearchTerm = searchTerm, 
                        AllSearchIndexData = _searchResults, 
                        PageIndex = PageIndex, 
                        totalPages = tPage,
                        }
                    );
            
                //PageIndex is how many photos are being displayed
            }
        }
        public ActionResult Search(FormCollection form)
        {
            return RedirectToAction("Index", new { searchTerm = form["searchTerm"], checkbox = form["checkbox"] });
        }
        /*public ActionResult Search(string searchTerm, string searchField, bool checkbox =false)
        {
            return RedirectToAction("Index", new { searchTerm, searchField, checkbox });
        }*/
        public ActionResult dateSearch(string GeneralCategory, string SecondCategory, string Year, string searchField, string searchTerm)
        {
            searchTerm = " " + Year + " " + GeneralCategory + " " + searchTerm;
            searchField = "";
            return RedirectToAction("Index", new {  type = "default", searchTerm, searchField });
        }

        public ActionResult SearchDefault(string searchTerm, string searchField)
        {
            
            return RedirectToAction("Index", new { type = "default", searchTerm, searchField });
        }

        public ActionResult PhotoSearchResults(FormCollection form)
        {
            var t = form["AllSearchIndexData"];
            string l = form["SearchTerm"];
            //return View(AllSearchIndexData);
            return View();
        }

        /*
         
         * 
         * -----------------------------Advanced Search Features-----------------------------------------*/
        private Photo2repo repo = new Photo2repo();

        [Authorize(Roles = "Photographer")]
        public ActionResult AdvancedSearch()
        {
            //get list of folders
            string path = HttpContext.Server.MapPath("~/Shoots");
            IEnumerable<folderCategory> folders = repo.getAllFolderNames(path);
            List<SelectListItem> Category_field_list = new List<SelectListItem>();
            List<string> names = new List<string>();
            bool nameExists = false;
            foreach (var f in folders)
            {
                foreach (var n in names) if (n == f.categoryName) nameExists = true;
                if (!nameExists)
                {
                    Category_field_list.Add(
                         new SelectListItem { Text = f.categoryName, Value = f.categoryName }
                        );
                    names.Add(f.categoryName);
                }
                
            }
            var search_field_list = new
               List<SelectListItem> {
				                     	new SelectListItem {Text = "(All Fields)", Value = ""},
				                     	new SelectListItem {Text = "Date", Value = "date"},
				                     	new SelectListItem {Text = "Name", Value = "Name"},
				                     	new SelectListItem {Text = "Description", Value = "description"}
				                     };
            List<SelectListItem> year_field_list = new List<SelectListItem>();
            for(int i= 2000; i <= DateTime.Now.Year; i++)
            {
                year_field_list.Add(
                    new SelectListItem {
                        Text = i.ToString(), 
                        Value = i.ToString()
                    });
            }
            
           PhotoIndexViewModel viewModel = new PhotoIndexViewModel();
          
           viewModel.SearchFieldList = search_field_list;
           viewModel.yearFieldList = year_field_list;
           viewModel.GeneralCategoryFieldList = Category_field_list;
            return View(viewModel);
        }
        //
        //
        // Return to Email
        private Photo2Entities1 db = new Photo2Entities1();
        public ActionResult AddToEmailCart(string photoShootName, string link,string searchTerm, string searchField, string type)
        {
            EmailCart emailcart = new EmailCart();
            emailcart.photoShootName = photoShootName;
            emailcart.link = link;
            emailcart.netId = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.EmailCarts.Add(emailcart);
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { searchTerm = searchTerm, searchField = searchField, type = type });
        }
    }
}