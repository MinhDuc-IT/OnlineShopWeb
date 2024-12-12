using OnlineShopWeb.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // Thêm namespace này
using OnlineShopWeb.Models;
using OnlineShopWeb.Data;

namespace OnlineShopWeb.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProductController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Product

        public ActionResult Index(int? id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();

            if (id.HasValue)
            {
                products = products.Where(p => p.ProductId == id).ToList();
            }

            return View(products);
        }

        public ActionResult ProductCategory(int id)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToList();

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                ViewBag.CateName = category.Name;
            }

            ViewBag.CateId = id;
            return View(products);
        }

        public ActionResult GetProductsByBrand(int brandId)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.BrandId == brandId)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();

                return PartialView("_ProductListPartial", products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToList();

                return PartialView("_ProductListPartial", products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult loadMoreProducts(int page = 1, int pageSize = 3) 
        {
            var products = _context.Products
                            .OrderBy(b => b.ProductId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                            .Select(b => new {
                                b.ProductId,
                                b.Price,
                                b.Name,
                                b.Image
                            }).ToList();

            bool hasMore = _context.Products.Count() > page * pageSize;

            return Json(new { success = true, products = products, hasMore = hasMore }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

    }
}