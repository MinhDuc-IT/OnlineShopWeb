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
using System.Security.Claims;
using AngleSharp.Io;

namespace OnlineShopWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly VnPayService vnPayService = new VnPayService();

        // Hiển thị giỏ hàng đã chọn và kiểm tra tính hợp lệ
        public ActionResult Checkout()
        {
            var selectedItems = Session["SelectedItems"] as List<SelectedItem>;

            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["Message"] = "Không có sản phẩm nào trong đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }

            var orderItems = GetOrderItems(selectedItems);
            if (orderItems == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            Session["OrderItems"] = orderItems;
            return View(orderItems);
        }

        // Tạo URL thanh toán VNPay
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

                Session["OrderInfo"] = paymentRequest;

                var httpContext = System.Web.HttpContext.Current;
                return Redirect(vnPayService.CreatePaymentUrl(httpContext, vnPayModel));
            }

            if (paymentRequest.PaymentMethod == "COD")
            {
                return ProcessCODPayment(paymentRequest);
            }
            TempData["Message"] = "Phương thức thanh toán không hỗ trợ";
            return RedirectToAction("PaymentFail");
        }

        // Xử lý phản hồi sau thanh toán VNPay
        public ActionResult PaymentCallBack()
        {
            var userId = GetUserId();
            if (userId == 0)
            {
                TempData["Message"] = "Người dùng không hợp lệ.";
                return RedirectToAction("PaymentFail");
            }

            var response = vnPayService.PaymentExecute(System.Web.HttpContext.Current.Request.QueryString);
            if (response?.VnPayResponseCode != "00")
            {
                TempData["Message"] = VNPayError.GetMessage(response?.VnPayResponseCode);
                return RedirectToAction("PaymentFail");
            }

            var orderItems = Session["OrderItems"] as List<OrderItem>;
            if (orderItems == null || !orderItems.Any())
            {
                TempData["Message"] = "Không có sản phẩm nào trong đơn hàng.";
                return RedirectToAction("PaymentFail");
            }

            return ProcessNCBOrder(userId, orderItems, response);
        }

        // Hiển thị thông báo lỗi thanh toán
        public ActionResult PaymentFail()
        {
            return View();
        }

        // Hiển thị thông báo thanh toán thành công
        public ActionResult PaymentSuccess()
        {
            return View();
        }

        // Hiển thị danh sách đơn hàng
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.Customer);
            return View(await orders.ToListAsync());
        }

        // Chi tiết đơn hàng
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        public ActionResult CancelOrder(int orderId)
        {
            // chuyển ỏder status = cancel, client lấy danh sách đơn hàng  thêm mục cancel
            // hiển thị thêm tình trạng thanh toán và phương thức thanh toán
            return PartialView("_OrderList");
        }

        public ActionResult GetOrdersByStatus(string status = "All")
        {
            ViewBag.Status = status;
            var userId = GetUserId();

            if (Enum.TryParse(status, out OrderStatus parsedStatus))
            {
                if (parsedStatus == OrderStatus.All)
                {
                    var orders = db.Orders.Include(o => o.OrderDetails)
                        .Where(o => o.CustomerId == userId).OrderByDescending(o => o.OrderDate).ToList();
                    return PartialView("_OrderList", orders);
                }

                var orderByStatus = db.Orders.Include(o => o.OrderDetails)
                    .Where(o => o.CustomerId == userId && o.Status == parsedStatus).OrderByDescending(o => o.OrderDate).ToList();
                return PartialView("_OrderList", orderByStatus);
            }

            var allOrders = db.Orders.Where(o => o.CustomerId == userId).OrderByDescending(o => o.OrderDate).ToList();
            return PartialView("_OrderList", allOrders);
        }

        // Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper Methods

        // Lấy danh sách sản phẩm từ session
        private List<OrderItem> GetOrderItems(List<SelectedItem> selectedItems)
        {
            var productIds = selectedItems.Select(c => c.ProductId).Distinct().ToList();
            var products = db.Products.Where(p => productIds.Contains(p.ProductId)).ToDictionary(p => p.ProductId);

            var orderItems = new List<OrderItem>();
            foreach (var selectedItem in selectedItems)
            {
                if (!products.TryGetValue(selectedItem.ProductId, out var product))
                {
                    TempData["Message"] = $"Sản phẩm với ID {selectedItem.ProductId} không còn tồn tại.";
                    return null;
                }

                if (selectedItem.Quantity <= 0 || selectedItem.Quantity > product.Stock)
                {
                    TempData["Message"] = $"Số lượng sản phẩm {product.Name} không hợp lệ.";
                    return null;
                }

                orderItems.Add(new OrderItem(product.ProductId, product.Image, product.Name, product.Price, selectedItem.Quantity));
            }

            return orderItems;
        }

        // Xử lý logic đặt hàng
        private ActionResult ProcessNCBOrder(int userId, List<OrderItem> orderItems, VnPaymentResponseModel response)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var orderInfo = Session["OrderInfo"] as PaymentRequest;

                    // Thêm đơn hàng
                    var newOrder = new Order
                    {
                        CustomerId = userId,
                        OrderDate = DateTime.ParseExact(response.TransactionDate, "yyyyMMddHHmmss", null),
                        ToTalAmount = response.Amount != null ? ((decimal)response.Amount) / 100 : 0,
                        RecipientName = orderInfo.FullName,
                        RecipientAddress = orderInfo.Address,
                        RecipientPhoneNumber = orderInfo.Mobile,
                        Status = OrderStatus.Processing,
                        OrderCode = response.OrderCode,
                        PaymentMethod = response.PaymentMethod,
                        OrderNotes = orderInfo.OrderNotes,
                        PaymentStatus = PaymentStatus.Completed,
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    // Thêm chi tiết đơn hàng
                    var orderDetails = orderItems.Select(item => new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList();
                    db.OrderDetails.AddRange(orderDetails);

                    // Cập nhật sản phẩm
                    UpdateProductStock(orderItems);

                    // Xóa sản phẩm trong giỏ hàng
                    ClearCart(userId, orderItems);

                    ClearSession();

                    transaction.Commit();
                    TempData["Message"] = "Thanh toán VNPay thành công";
                    return RedirectToAction("PaymentSuccess");
                }
                catch
                {
                    transaction.Rollback();
                    TempData["Message"] = "Đã xảy ra lỗi trong quá trình xử lý thanh toán.";
                    return RedirectToAction("PaymentFail");
                }
            }
        }

        private ActionResult ProcessCODPayment(PaymentRequest paymentRequest)
        {
            var userId = GetUserId();

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Thêm đơn hàng
                    var newOrder = new Order
                    {
                        CustomerId = userId,
                        OrderDate = DateTime.Now,
                        ToTalAmount = (decimal)paymentRequest.Total,
                        RecipientName = paymentRequest.FullName,
                        RecipientAddress = paymentRequest.Address,
                        RecipientPhoneNumber = paymentRequest.Mobile,
                        Status = OrderStatus.Processing,
                        PaymentMethod = paymentRequest.PaymentMethod,
                        OrderNotes = paymentRequest.OrderNotes,
                        PaymentStatus = PaymentStatus.Pending
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    var orderItems = Session["OrderItems"] as List<OrderItem>;
                    if (orderItems == null || !orderItems.Any())
                    {
                        TempData["Message"] = "Không có sản phẩm nào trong đơn hàng.";
                        return RedirectToAction("PaymentFail");
                    }

                    // Thêm chi tiết đơn hàng
                    var orderDetails = orderItems.Select(item => new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = item.ProductId,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList();
                    db.OrderDetails.AddRange(orderDetails);

                    // Cập nhật sản phẩm
                    UpdateProductStock(orderItems);

                    // Xóa sản phẩm trong giỏ hàng
                    ClearCart(userId, orderItems);

                    ClearSession();

                    transaction.Commit();
                    TempData["Message"] = "Thanh toán VNPay thành công";
                    return RedirectToAction("PaymentSuccess");
                }
                catch
                {
                    transaction.Rollback();
                    TempData["Message"] = "Đã xảy ra lỗi trong quá trình xử lý thanh toán.";
                    return RedirectToAction("PaymentFail");
                }
            }
        }

        // Cập nhật số lượng sản phẩm trong kho
        private void UpdateProductStock(List<OrderItem> orderItems)
        {
            var productIds = orderItems.Select(item => item.ProductId).ToList();
            var products = db.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

            foreach (var item in orderItems)
            {
                var product = products.First(p => p.ProductId == item.ProductId);
                product.Stock -= item.Quantity;
                product.Click = 0;
            }

            db.SaveChanges();
        }

        // Xóa sản phẩm trong giỏ hàng
        private void ClearCart(int userId, List<OrderItem> orderItems)
        {
            // Lấy giỏ hàng từ cơ sở dữ liệu
            var cart = db.Carts.FirstOrDefault(c => c.CustomerId == userId);
            if (cart != null)
            {
                var productIds = orderItems.Select(item => item.ProductId).ToList();

                // Lấy các mặt hàng cần xóa khỏi cơ sở dữ liệu
                var cartItems = db.CartItems
                    .Where(c => c.CartId == cart.CartId && productIds.Contains(c.ProductId))
                    .ToList();

                // Xóa các mặt hàng khỏi cơ sở dữ liệu
                db.CartItems.RemoveRange(cartItems);
                db.SaveChanges();
            }

            // Cập nhật giỏ hàng trong session
            var sessionCart = Session["Cart"] as Cart;
            if (sessionCart != null)
            {
                var itemsToRemove = sessionCart.CartItems
                    .Where(ci => orderItems.Any(oi => oi.ProductId == ci.ProductId))
                    .ToList();

                foreach (var item in itemsToRemove)
                {
                    sessionCart.CartItems.Remove(item);
                }

                // Cập nhật lại Session["Cart"]
                Session["Cart"] = sessionCart;
            }
        }

        private void ClearSession()
        {
            HttpContext.Session.Remove("OrderItems");
            HttpContext.Session.Remove("OrderInfo");
        }

        // Lấy ID người dùng hiện tại
        private int GetUserId()
        {
            var user = Session["User"] as User;

            return db.Users.FirstOrDefault(u => u.CustomerId == user.CustomerId)?.CustomerId ?? 0;
        }

        #endregion
    }
}
