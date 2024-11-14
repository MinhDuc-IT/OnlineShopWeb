using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string query)
        {
            ViewBag.Query = query;
            IEnumerable<Product> items = db.Products.Where(x => x.Name.Contains(query)).ToList();
            return View(items);
        }
    }
}