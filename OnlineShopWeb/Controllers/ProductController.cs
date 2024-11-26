using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
        public ActionResult loadMoreProducts(int page = 1, int pageSize = 3) 
        {
            var products = db.Products
                            .OrderBy(b => b.ProductId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                            .Select(b => new {
                                b.ProductId,
                                b.Price,
                                b.Name,
                                ImageUrl = Url.Content("~/Content/images/home/product1.jpg") 
                            }).ToList();

            bool hasMore = db.Products.Count() > page * pageSize;

            return Json(new { success = true, products = products, hasMore = hasMore }, JsonRequestBehavior.AllowGet);
        }
    }
}