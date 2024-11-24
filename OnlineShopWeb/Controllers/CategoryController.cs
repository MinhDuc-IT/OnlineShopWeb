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
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ListCategory()
        {
            var items = db.Categories.Where(x => x.IsDeleted == false).ToList();
            return PartialView("_ListCategory", items);
        }
    }
}