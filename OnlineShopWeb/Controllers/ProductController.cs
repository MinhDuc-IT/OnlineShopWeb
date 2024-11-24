using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? id)
        {
            var items = db.Products.ToList();
            if (id != null)
            {
                items = items.Where(x => x.ProductId == id).ToList();
            }
            return View(items);
        }
        public ActionResult ProductCategory(int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.CategoryId == id).ToList();
            }
            var cate = db.Categories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Name;
            }

            ViewBag.CateId = id;
            return View(items);
        }
    }
}