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
using OnlineShopWeb.ViewModels;

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
                .Where(p => p.IsDeleted ==  false)
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
                .Where(p => p.CategoryId == id && p.IsDeleted == false)
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
                    .Where(p => p.BrandId == brandId && p.IsDeleted == false)
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
                    .Where(p => p.CategoryId == categoryId && p.IsDeleted == false)
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

        //public List<Product> GetHotProducts(DateTime startDate, DateTime endDate)
        //{
        //    var viewedProducts = _context.Products
        //        .Where(p => p.LastViewed >= startDate && p.LastViewed <= endDate)
        //        .Select(p => new ViewedProduct
        //        {
        //            ProductId = p.ProductId,
        //            ViewCount = p.Click 
        //        })
        //        .ToList(); 

        //    var orderedProducts = _context.Orders
        //        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
        //        .SelectMany(o => o.OrderDetails)
        //        .GroupBy(od => od.ProductId)
        //        .Select(g => new OrderedProduct
        //        {
        //            ProductId = g.Key,
        //            OrderCount = g.Count() 
        //        })
        //        .ToList();

        //    // Combine the results in-memory
        //    var hotProducts = viewedProducts
        //        .Join(
        //            orderedProducts,
        //            vp => vp.ProductId,
        //            op => op.ProductId,
        //            (vp, op) => new HotProduct
        //            {
        //                ProductId = vp.ProductId,
        //                HotScore = vp.ViewCount * 0.3 + op.OrderCount * 0.7
        //            })
        //        .OrderByDescending(p => p.HotScore)
        //        .Take(10)
        //        .ToList();

        //    //var result = _context.Products
        //    //    .Where(p => hotProducts.Select(hp => hp.ProductId).Contains(p.ProductId))
        //    //    .ToList();

        //    var result = new List<Product>();

        //    foreach (var item in hotProducts)
        //    {
        //        var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
        //        if(product != null)
        //        {
        //            result.Add(product);
        //        }
        //    }

        //    return result;
        //}

        public List<Product> GetHotProducts(DateTime startDate, DateTime endDate)
        {
            // Fetch viewed products, handle null case
            var viewedProducts = _context.Products
                .Where(p => p.LastViewed >= startDate && p.LastViewed <= endDate && p.IsDeleted == false)
                .Select(p => new ViewedProduct
                {
                    ProductId = p.ProductId,
                    ViewCount = p.Click
                })
                .ToList() ?? new List<ViewedProduct>();

            // Fetch ordered products, handle null case
            var orderedProducts = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.ProductId)
                .Select(g => new OrderedProduct
                {
                    ProductId = g.Key,
                    OrderCount = g.Count()
                })
                .ToList() ?? new List<OrderedProduct>();

            // Combine the results in-memory
            var hotProducts = viewedProducts
                .Join(
                    orderedProducts,
                    vp => vp.ProductId,
                    op => op.ProductId,
                    (vp, op) => new HotProduct
                    {
                        ProductId = vp.ProductId,
                        HotScore = vp.ViewCount * 0.3 + op.OrderCount * 0.7
                    })
                .Union(viewedProducts
                    .Where(vp => !orderedProducts.Any(op => op.ProductId == vp.ProductId))
                    .Select(vp => new HotProduct
                    {
                        ProductId = vp.ProductId,
                        HotScore = vp.ViewCount * 0.3
                    }))
                .Union(orderedProducts
                    .Where(op => !viewedProducts.Any(vp => vp.ProductId == op.ProductId))
                    .Select(op => new HotProduct
                    {
                        ProductId = op.ProductId,
                        HotScore = op.OrderCount * 0.7
                    }))
                .OrderByDescending(p => p.HotScore)
                .Take(10)
                .ToList();

            var result = new List<Product>();

            foreach (var item in hotProducts)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    result.Add(product);
                }
            }
            return result;
        }

        public ActionResult HotProducts()
        {
            // Tính sản phẩm hot trong tuần
            var hotProductsThisWeek = GetHotProducts(DateTime.Now.AddDays(-7), DateTime.Now);

            if (hotProductsThisWeek.Count < 10)
            {
                var hotProductsThisMonth = GetHotProducts(DateTime.Now.AddMonths(-1), DateTime.Now);
                hotProductsThisWeek.AddRange(hotProductsThisMonth);
            }

            return PartialView("_HotProducts", hotProductsThisWeek.Distinct().ToList());
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
                .Where(p => p.IsDeleted == false)
                            .OrderBy(b => b.ProductId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                            .Select(b => new {
                                b.ProductId,
                                price = b.Price.ToString("N0"),
                                b.Name,
                                b.Image
                            }).ToList();

            bool hasMore = _context.Products.Count() > page * pageSize;

            return Json(new { success = true, products = products, hasMore = hasMore }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id && p.IsDeleted == false);
            if (product == null)
            {
                return HttpNotFound();
            }

            product.Click += 1;
            product.LastViewed = DateTime.Now;


            var user = Session["User"] as User;

            if(user != null)
            {
                int userId = user.CustomerId;
                var userProduct = _context.UserProducts.FirstOrDefault(x => x.UserId == userId && x.ProductId == id);
                if (userProduct != null)
                {
                    userProduct.ViewNum += userProduct.ViewNum + 1;
                    userProduct.LastViewed = DateTime.Now;

                    return View(product);
                }
                var obj = new UserProduct
                {
                    UserId = userId,
                    ProductId = id,
                    ViewNum = 1,
                    LastViewed = DateTime.Now,
                };

                _context.UserProducts.Add(obj);
                _context.SaveChanges();
            }

            return View(product);
        }

    }
}