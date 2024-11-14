using System;
using System.Collections.Generic;
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
        public ActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
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
    }
}