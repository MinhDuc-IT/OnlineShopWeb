using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListCategory()
        {
            var items = db.Categories.ToList();
            items = items.Where(x => x.IsDeleted == false).ToList();
            return PartialView("_ListCategory", items);
        }
    }
}