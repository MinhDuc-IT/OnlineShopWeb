using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class BrandController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ListBrand()
        {
            var items = db.Brands.ToList();
            return PartialView("_ListBrand", items);
        }
    }
}