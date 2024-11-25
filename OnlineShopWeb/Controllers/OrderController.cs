using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using OnlineShopWeb.ViewModels;
using OnlineShopWeb.Helpers;
using System.Diagnostics;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OnlineShopWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly VnPayService vnPayService = new VnPayService();

        public ActionResult Checkout()
        {
            var selectedItems = Session["SelectedItems"] as List<SelectedItem>;

            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["Message"] = "Không có sản phẩm nào trong đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }

            var productIds = selectedItems.Select(c => c.ProductId).Distinct().ToList();
            var products = db.Products.Where(p => productIds.Contains(p.ProductId)).ToDictionary(p => p.ProductId);

            var orderItems = new List<OrderItem>();

            foreach (var selectedItem in selectedItems)
            {
                if (!products.TryGetValue(selectedItem.ProductId, out var product))
                {
                    TempData["Message"] = $"Sản phẩm với ID {selectedItem.ProductId} không còn tồn tại.";
                    return RedirectToAction("Index", "Cart"); 
                }

                if (selectedItem.Quantity <= 0 || selectedItem.Quantity > product.Stock)
                {
                    TempData["Message"] = $"Số lượng sản phẩm {product.Name} không hợp lệ.";
                    return RedirectToAction("Index", "Cart"); 
                }

                orderItems.Add(new OrderItem(product.ProductId, product.Image, product.Name, product.Price ,selectedItem.Quantity));
            }

            Session["OrderItems"] = orderItems;

            return View(orderItems);
        }

        [HttpPost]
        public ActionResult Payment(PaymentRequest paymentRequest)
        {
            if (paymentRequest.PaymentMethod == "NCB")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = paymentRequest.Total,
                    CreatedDate = DateTime.Now,
                    Description = $"{paymentRequest.FullName}-{paymentRequest.Mobile}",
                    FullName = paymentRequest.FullName,
                    OrderId = new Random().Next(1000, 10000)
                };

                var httpContext = System.Web.HttpContext.Current;
                return Redirect(vnPayService.CreatePaymentUrl(httpContext, vnPayModel));
            }

            TempData["Message"] = "Lỗi thanh toán VN Pay";
            return RedirectToAction("PaymentFail");
        }

        public ActionResult PaymentCallBack()
        {
            var userId = db.Users.FirstOrDefault(u => u.Name == "test")?.CustomerId ?? 0;
            if (userId == 0)
            {
                TempData["Message"] = "Người dùng không hợp lệ.";
                return RedirectToAction("PaymentFail");
            }

            var httpContext = System.Web.HttpContext.Current;
            var response = vnPayService.PaymentExecute(httpContext.Request.QueryString);

            if (response?.VnPayResponseCode != "00")
            {
                TempData["Message"] = VNPayError.GetMessage(response?.VnPayResponseCode);
                return RedirectToAction("PaymentFail");
            }

            var orderItems = Session["OrderItems"] as List<OrderItem>;

            if (!orderItems.Any())
            {
                TempData["Message"] = "Không có sản phẩm nào trong đơn hàng.";
                return RedirectToAction("PaymentFail");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newOrder = new Order
                    {
                        CustomerId = userId,
                        OrderDate = DateTime.Now,
                        ToTalAmount = (decimal)response.Amount
                    };

                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    var orderDetails = orderItems.Select(item => new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList();

                    db.OrderDetails.AddRange(orderDetails);

                    var productIds = orderItems.Select(item => item.ProductId).ToList();
                    var products = db.Products.Where(p => productIds.Contains(p.ProductId)).ToList();
                    
                    foreach (var item in orderItems)
                    {
                        var product = products.First(p => p.ProductId == item.ProductId);
                        product.Stock -= item.Quantity;
                        product.Click = 0;
                    }

                    //var cartUser = db.Carts.FirstOrDefault(c => c.CustomerId == userId);
                    //var cartItemsRemove = db.CartItems.Where(c => productIds.Contains(c.ProductId) && c.CartId == cartUser.CartId).ToList();

                    //db.CartItems.RemoveRange(cartItemsRemove);

                    db.SaveChanges();

                    //Cart cart = (Cart)Session["Cart"];
                    //if (cart != null)
                    //{
                    //    foreach (var item in cartItemsRemove)
                    //    {
                    //        cart.CartItems.Remove(item);
                    //    }
                    //}

                    //Session["Cart"] = cart;
                    //HttpContext.Session.Remove("OrderItems");

                    transaction.Commit();

                    TempData["Message"] = "Thanh toán VNPay thành công";
                    return RedirectToAction("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Message"] = "Đã xảy ra lỗi trong quá trình xử lý thanh toán.";
                    return RedirectToAction("PaymentFail");
                }
            }
        }

        public ActionResult PaymentFail()
        {
            return View();
        }

        public ActionResult PaymentSuccess()
        {
            HttpContext.Session.Remove("OrderItems");
            return View();
        }

        // GET: Order
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.Customer);
            return View(await orders.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
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
