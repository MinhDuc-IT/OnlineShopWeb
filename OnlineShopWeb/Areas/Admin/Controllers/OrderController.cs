using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrdersByStatus(string status = "All")
        {
            //ViewBag.Status = status;

            if (Enum.TryParse(status, out OrderStatus parsedStatus))
            {
                if (parsedStatus == OrderStatus.All)
                {
                    var orders = db.Orders.Include(o => o.OrderDetails).OrderByDescending(o => o.OrderDate).ToList();
                    return PartialView("_OrderList", orders);
                }

                var orderByStatus = db.Orders.Include(o => o.OrderDetails).Where(o => o.Status == parsedStatus).OrderByDescending(o => o.OrderDate).ToList();
                return PartialView("_OrderList", orderByStatus);
            }

            var allOrders = db.Orders.OrderByDescending(o => o.OrderDate).ToList();
            return PartialView("_OrderList", allOrders);
        }

        public ActionResult ConfirmOrder(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" }, JsonRequestBehavior.AllowGet);
            }

            if (order.Status != OrderStatus.Processing)
            {
                string message = order.Status == OrderStatus.Shipping
                    ? "Order is already being shipped."
                    : "Order cannot be confirmed because its current status is not valid for confirmation.";

                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                order.Status = OrderStatus.Shipping;
                db.SaveChanges();
                return Json(new { success = true, message = "Order confirmed successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to confirm the order", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
