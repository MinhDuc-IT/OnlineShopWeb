using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;

namespace OnlineShopWeb.Controllers
{
    public class UserProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateViewNum(int id)
        {
            var user = Session["User"] as User;
            if (user == null)
            {
                return Json(new { success = false });
            }

            var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if(product != null)
            {
                product.Click += 1;
                product.LastViewed = DateTime.Now;
            }

            int userId = user.CustomerId;
            var userProduct = db.UserProducts.FirstOrDefault(x => x.UserId == userId && x.ProductId == id);
            if (userProduct != null)
            {
                userProduct.ViewNum += userProduct.ViewNum + 1;
                userProduct.LastViewed = DateTime.Now;

                db.SaveChanges();
                return Json(new { success = true });
            }
            var obj = new UserProduct
            {
                UserId = userId,
                ProductId = id,
                ViewNum = 1,
                LastViewed = DateTime.Now,
            };
            db.UserProducts.Add(obj);
            db.SaveChanges();
            return Json(new { success = true });
        }
        public ActionResult GetRecommenProducts()
        {
            var userSession = Session["User"] as User;
            if (userSession != null)
            {
                var user = db.Users.FirstOrDefault(u => u.CustomerId == userSession.CustomerId);

                if(user != null)
                {
                    var recommendedProducts = user.UserProducts
                                       .Where(up => up.ViewNum > 0 ||
                            (up.LastViewed.HasValue && up.LastViewed.Value >= DateTime.Now.AddDays(-7)))
                                       .OrderByDescending(up => up.ViewNum) 
                                       .ThenByDescending(up => up.LastViewed)
                                       .Select(up => up.Product)
                                       .Take(10)
                                       .ToList();

                    if (recommendedProducts.Count < 10)
                    {
                        var additionalProducts = user.UserProducts
                            .Where(up => up.ViewNum > 0 ||
                            (up.LastViewed.HasValue && up.LastViewed.Value >= DateTime.Now.AddDays(-30)))
                            .OrderByDescending(up => up.ViewNum)
                            .ThenByDescending(up => up.LastViewed)
                            .Select(up => up.Product)
                            .Take(10)
                            .ToList();

                        recommendedProducts.AddRange(additionalProducts);
                    }
                    return PartialView("_RecommenProducts", recommendedProducts.Distinct().ToList());
                }

                return PartialView("_RecommenProducts", null);
            }

            return PartialView("_RecommenProducts", null);
        }
    }
}