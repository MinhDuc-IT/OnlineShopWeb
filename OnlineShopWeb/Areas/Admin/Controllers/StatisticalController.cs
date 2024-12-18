using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Statistical
        public ActionResult StatisticalCategory()
        {
            // Existing counts...
            var count_product = db.Products.Count();
            var count_order = db.Orders.Count();
            var count_category = db.Categories.Count();
            var count_user = db.Users.Count();

            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;

            return View();
        }

        public ActionResult Top5Products()
        {
            var topProducts = db.Products
                .OrderByDescending(p => p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0)
                .Take(5)
                .Select(p => new
                {
                    Name = p.Name,
                    TotalQuantity = p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0
                })
                .ToList();

            return Json(topProducts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RevenueAndProfit(int year)
        {
            var monthlyData = new List<MonthlyStatistics>();

            for (int month = 1; month <= 12; month++)
            {
                var orders = db.Orders
                    .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month)
                    .ToList();

                decimal totalRevenue = orders.Sum(o => o.ToTalAmount);
                decimal totalCost = orders.SelectMany(o => o.OrderDetails)
                                           .Sum(od => od.Quantity * db.Products.Where(p => p.ProductId == od.ProductId).Select(p => p.CostPrice).FirstOrDefault());

                decimal profit = totalRevenue - totalCost;

                monthlyData.Add(new MonthlyStatistics
                {
                    Month = month,
                    Revenue = totalRevenue,
                    Profit = profit
                });
            }

            return Json(monthlyData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAvailableYears()
        {
            var years = db.Orders.Select(o => o.OrderDate.Year).Distinct().ToList();
            return Json(years, JsonRequestBehavior.AllowGet);
        }

    }
}
