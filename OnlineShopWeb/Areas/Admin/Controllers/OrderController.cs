using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using OnlineShopWeb.ViewModels;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string ApiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        //private const string SecretKey = "BYKJBHPPZKQMKBIBGGXIYKWYFAYSJXCW"; // SecretKey của merchant
        private static readonly string SecretKey = ConfigurationManager.AppSettings["VnPay:HashSecret"];
        //private const string TmnCode = "NJJ0R8FS"; // Mã định danh thanh toán của bạn
        private static readonly string TmnCode = ConfigurationManager.AppSettings["VnPay:TmnCode"];
        private static readonly HttpClient client = new HttpClient(); // Tái sử dụng HttpClient

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrdersByStatus(string status = "All")
        {
            // Kiểm tra nếu status có thể được chuyển đổi thành một giá trị hợp lệ của enum OrderStatus
            if (Enum.TryParse(status, out OrderStatus parsedStatus))
            {
                // Nếu trạng thái là "All", lấy tất cả các đơn hàng và chuyển đổi sang OrderDTO
                if (parsedStatus == OrderStatus.All)
                {
                    var orders = db.Orders.Include(o => o.OrderDetails)
                                          .OrderByDescending(o => o.OrderDate)
                                          .ToList();
                    //return PartialView("_OrderList", orders);
                    var orderDTOs = orders.Select(order => new OrderDTO
                    {
                        OrderId = order.OrderId,
                        Status = order.Status.ToString(),
                        PaymentStatus = order.PaymentStatus.ToString(),
                        PaymentMethod = order.PaymentMethod,  // Thêm phương thức thanh toán
                        TotalAmount = order.ToTalAmount,
                        OrderNotes = order.OrderNotes,  // Thêm ghi chú đơn hàng
                        OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                        {
                            ProductName = od.Product.Name,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            ProductImage = od.Product.Image  // Chuyển đổi ảnh thành base64
                        }).ToList()
                    }).ToList();

                    return Json(orderDTOs, JsonRequestBehavior.AllowGet);
                }

                // Nếu trạng thái là một giá trị hợp lệ trong enum, lấy đơn hàng theo trạng thái đó
                var orderByStatus = db.Orders.Include(o => o.OrderDetails)
                                             .Where(o => o.Status == parsedStatus)
                                             .OrderByDescending(o => o.OrderDate)
                                             .ToList();

                var orderByStatusDTOs = orderByStatus.Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    Status = order.Status.ToString(),
                    PaymentStatus = order.PaymentStatus.ToString(),
                    PaymentMethod = order.PaymentMethod,  // Thêm phương thức thanh toán
                    TotalAmount = order.ToTalAmount,
                    OrderNotes = order.OrderNotes,  // Thêm ghi chú đơn hàng
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        ProductImage = od.Product.Image  // Chuyển đổi ảnh thành base64
                    }).ToList()
                }).ToList();

                return Json(orderByStatusDTOs, JsonRequestBehavior.AllowGet);
            }

            // Nếu không truyền tham số trạng thái hợp lệ, trả về tất cả đơn hàng
            var allOrders = db.Orders.OrderByDescending(o => o.OrderDate).ToList();
            var allOrderDTOs = allOrders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                PaymentMethod = order.PaymentMethod,  // Thêm phương thức thanh toán
                TotalAmount = order.ToTalAmount,
                OrderNotes = order.OrderNotes,  // Thêm ghi chú đơn hàng
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    ProductImage = od.Product.Image // Chuyển đổi ảnh thành base64
                }).ToList()
            }).ToList();

            return Json(allOrderDTOs, JsonRequestBehavior.AllowGet);
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

        public ActionResult CancelOrder(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (order.Status != OrderStatus.Processing)
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang xử lý." }, JsonRequestBehavior.AllowGet);
            }

            if (order.PaymentStatus == PaymentStatus.Completed && order.PaymentMethod == "NCB")
            {
                order.OrderNotes = "Đang xử lí hoàn tiền. Vui lòng chờ";
            }
            else if (order.PaymentStatus == PaymentStatus.Pending && order.PaymentMethod == "COD")
            {
                order.OrderNotes = "Hủy đơn hàng thành công";
            }
            else
            {
                return Json(new { success = false, message = "Không thể hủy đơn hàng với trạng thái hiện tại." }, JsonRequestBehavior.AllowGet);
            }

            order.Status = OrderStatus.Canceled;

            try
            {
                db.SaveChanges();
                return Json(new { success = true, message = "Hủy đơn hàng thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi hủy đơn hàng: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> TransactionQuery(int orderId)
        {
            // Tìm kiếm đơn hàng dựa trên orderId
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);

            // Kiểm tra nếu đơn hàng không tồn tại
            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            // Thông tin truy vấn
            string requestId = Guid.NewGuid().ToString();
            string version = "2.1.0";
            string command = "querydr";
            string txnRef = order.OrderCode; // Mã đơn hàng
            string transactionDate = order.OrderDate.ToString("yyyyMMddHHmmss"); // Thời gian giao dịch
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss"); // Thời gian phát sinh yêu cầu
            string ipAddr = "127.0.0.1"; // Địa chỉ IP giả định (cập nhật nếu cần)
            string orderInfo = "Truy vấn giao dịch đơn hàng: " + order.OrderId;

            // Tạo chuỗi để mã hóa checksum
            string rawData = string.Join("|",
                requestId, version, command, TmnCode, txnRef, transactionDate, createDate, ipAddr, orderInfo);
            string secureHash = GenerateSecureHash(rawData);

            // Đối tượng JSON yêu cầu
            var requestData = new
            {
                vnp_RequestId = requestId,
                vnp_Version = version,
                vnp_Command = command,
                vnp_TmnCode = TmnCode,
                vnp_TxnRef = txnRef,
                vnp_TransactionDate = transactionDate,
                vnp_CreateDate = createDate,
                vnp_IpAddr = ipAddr,
                vnp_OrderInfo = orderInfo,
                vnp_SecureHash = secureHash
            };

            // Gửi yêu cầu đến VNPay
            try
            {
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    // Đọc kết quả JSON từ VNPay
                    var result = await response.Content.ReadAsStringAsync();
                    var vnPayResponse = JsonConvert.DeserializeObject<VnPayTransactionResponse>(result);

                    // Trả về JSON thành công
                    return Json(new
                    {
                        success = true,
                        message = vnPayResponse.VnpMessage,
                        data = vnPayResponse
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Trường hợp lỗi từ phía VNPay
                    return Json(new
                    {
                        success = false,
                        message = "Lỗi truy vấn giao dịch từ VNPay.",
                        reason = response.ReasonPhrase
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Trường hợp lỗi khi gửi yêu cầu
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi gửi yêu cầu.",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> RefundRequest(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            if (order.Status != OrderStatus.Canceled)
            {
                return Json(new { success = false, message = "Đơn hàng không thể hoàn tiền vì trạng thái không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            if (order.PaymentStatus != PaymentStatus.Completed || order.PaymentMethod != "NCB")
            {
                return Json(new { success = false, message = "Không thể hoàn tiền vì trạng thái thanh toán chưa hoàn tất hoặc phương thức thanh toán không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            var user = GetUser();
            if (user == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục." }, JsonRequestBehavior.AllowGet); // Return a JSON response
            }

            string requestId = Guid.NewGuid().ToString();
            string version = "2.1.0";
            string command = "refund";
            string transactionType = "02";
            string txnRef = order.OrderCode;
            string amount = ((int)order.ToTalAmount * 100).ToString();
            string transactionDate = order.OrderDate.ToString("yyyyMMddHHmmss");
            string createBy = user.Name;
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string ipAddr = "127.0.0.1";
            string orderInfo = "Refund request for order " + order.OrderId;

            string transactionNo = "";

            string rawData = string.Join("|",
                requestId, version, command, TmnCode, transactionType, txnRef, amount, transactionNo,
                transactionDate, createBy, createDate, ipAddr, orderInfo);

            string secureHash = GenerateSecureHash(rawData);

            var requestData = new
            {
                vnp_RequestId = requestId,
                vnp_Version = version,
                vnp_Command = command,
                vnp_TmnCode = TmnCode,
                vnp_TransactionType = transactionType,
                vnp_TxnRef = txnRef,
                vnp_Amount = amount,
                vnp_TransactionNo = transactionNo,
                vnp_TransactionDate = transactionDate,
                vnp_CreateBy = createBy,
                vnp_CreateDate = createDate,
                vnp_IpAddr = ipAddr,
                vnp_OrderInfo = orderInfo,
                vnp_SecureHash = secureHash
            };

            try
            {
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var vnPayResponse = JsonConvert.DeserializeObject<VnPayRefundResponse>(result);

                    if(vnPayResponse == null)
                    {
                        return Json(new { success = false, message = "Lỗi trong giao dịch hoàn tiền." }, JsonRequestBehavior.AllowGet);
                    }

                    if (vnPayResponse != null && vnPayResponse.Vnp_ResponseCode == "00")
                    {
                        order.PaymentStatus = PaymentStatus.Refunded;
                        order.OrderNotes = "Hoàn tiền thành công";
                        db.SaveChanges();
                    }
                    return Json(new
                    {
                        success = true,
                        message = vnPayResponse.Vnp_Message,
                        data = vnPayResponse
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi trong giao dịch hoàn tiền." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi yêu cầu hoàn tiền: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        // Hàm tạo mã checksum
        private string GenerateSecureHash(string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(SecretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private User GetUser()
        {
            var user = Session["User"] as User;
            if (user == null)
            {
                return null; 
            }

            return db.Users.FirstOrDefault(u => u.CustomerId == user.CustomerId);
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
