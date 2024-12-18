using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetRevenueByWeek(DateTime dateSelected)
        {
            var startOfWeek = dateSelected.AddDays(-(int)dateSelected.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);

            // Lấy tất cả các đơn hàng trong tuần đã chọn
            var orders = db.Orders
                          .Where(o => o.OrderDate >= startOfWeek && o.OrderDate <= endOfWeek)
                          .ToList();

            // Tạo danh sách doanh thu cho mỗi ngày trong tuần (Thứ Hai đến Chủ Nhật)
            var revenue = new decimal[7];

            foreach (var order in orders)
            {
                int dayIndex = (int)(order.OrderDate.DayOfWeek - DayOfWeek.Monday);
                if (dayIndex < 0) dayIndex += 7; // Đảm bảo đúng chỉ số cho các ngày trong tuần
                revenue[dayIndex] += order.ToTalAmount; // Cộng doanh thu vào ngày tương ứng
            }

            return Json(new { success = true, revenue = revenue }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLatestOrders()
        {
            var orders = db.Orders
                            .OrderByDescending(o => o.OrderDate) 
                            .Take(20)
                            .Include(o => o.Customer)
                             .AsEnumerable()
                            .Select(o => new
                            {
                                o.OrderId,
                                totalAmount = o.ToTalAmount.ToString("N0"), 
                                o.Status,
                                o.RecipientName,
                                orderDate = o.OrderDate.ToString("dd/MM/yyyy"),
                                customerName = o.Customer.Name, 
                                customerEmail = o.Customer.Email 
                            })
                            .ToList();

            return Json(new { success = true, latestOrders = orders }, JsonRequestBehavior.AllowGet);
        }
    }
}