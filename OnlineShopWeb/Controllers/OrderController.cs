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
        private ApplicationDbContext db = new ApplicationDbContext();

        private VnPayService vnPayService = new VnPayService();

        private List<OrderItem> orderItems = new List<OrderItem>();

        public ActionResult Checkout()
        {
            // Tạo một đối tượng User mới
            var newUser = new User
            {
                Name = "test",
                Email = "johndoe@example.com",
                Password = "test",
                Phone = "1234567890",
                Address = "123 Main Street",
                Role = "Customer"
            };

            // Thêm user vào DbContext và lưu vào cơ sở dữ liệu
            db.Users.Add(newUser);

            // Tìm hoặc tạo mới Brand
            var brand = db.Brands.FirstOrDefault(b => b.Name == "test") ?? new Brand
            {
                Name = "test",
                Description = "test"
            };

            // Tìm hoặc tạo mới Category
            var category = db.Categories.FirstOrDefault(c => c.Name == "test") ?? new Category
            {
                Name = "test",
                Description = "test",
                IsDeleted = false
            };

            // Tìm hoặc tạo mới Product
            var product = db.Products.FirstOrDefault(p => p.Name == "test") ?? new Product
            {
                Name = "test",
                Brand = brand,           // Gán Brand trực tiếp vào Product
                Description = "test",
                Image = "https://placehold.co/90x90?text=OrderItem",
                Price = 30000,
                Stock = 99999,
                Category = category,     // Gán Category trực tiếp vào Product
                IsDeleted = false
            };

            // Kiểm tra và thêm các bản ghi mới vào DbContext
            if(newUser.CustomerId == 0) db.Users.Add(newUser);
            if (brand.BrandId == 0) db.Brands.Add(brand);
            if (category.CategoryId == 0) db.Categories.Add(category);
            if (product.ProductId == 0) db.Products.Add(product);

            // Lưu các thay đổi vào cơ sở dữ liệu
            db.SaveChanges();


            return View();
        }

        [HttpPost]
        public JsonResult GetOrderItem(List<CheckoutRequest> checkoutRequests)
        {

            //int userId = int.Parse(User.FindFirstValue("userId"));
            //int userId = db.Users.FirstOrDefault(u => u.Name == "test").CustomerId;

            if (checkoutRequests == null || checkoutRequests.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    data = "",
                    message = "No items provided."
                }, JsonRequestBehavior.AllowGet);
            }

            foreach (var checkoutItem in checkoutRequests)
            {
                if (checkoutItem.productId < 0 || checkoutItem.quantity < 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid product ID or quantity."
                    }, JsonRequestBehavior.AllowGet);
                }

                var product = db.Products.FirstOrDefault(p => p.ProductId == checkoutItem.productId);
                if (product == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Product with ID {checkoutItem.productId} not found."
                    }, JsonRequestBehavior.AllowGet);
                }

                if (checkoutItem.quantity > product.Stock)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Insufficient stock for product with ID {checkoutItem.productId}. Only {product.Stock} items are available."
                    }, JsonRequestBehavior.AllowGet);
                }

                var item = new OrderItem(product.ProductId, product.Image, product.Name, product.Price, checkoutItem.quantity);
                orderItems.Add(item);
            }

            return Json(new
            {
                success = true,
                data = orderItems,
                message = "Order items retrieved successfully."
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Payment(PaymentRequest paymentRequest)
        {
            //int userId = int.Parse(User.FindFirstValue("userId"));
            //int userId = db.Users.FirstOrDefault(u => u.Name == "test").CustomerId;

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
            TempData["Message"] = $"Lỗi thanh toán VN Pay";
            return RedirectToAction("PaymentFail");
        }

        public ActionResult PaymentCallBack()
        {

            //int userId = int.Parse(User.FindFirstValue("userId"));
            int userId = db.Users.FirstOrDefault(u => u.Name == "test").CustomerId;

            var httpContext = System.Web.HttpContext.Current;
            var response = vnPayService.PaymentExecute(httpContext.Request.QueryString);

            if (response == null || response.VnPayResponseCode != "00")
            {
                var errorMessage = VNPayError.GetMessage(response?.VnPayResponseCode);

                TempData["Message"] = $"{errorMessage}";
                return RedirectToAction("PaymentFail");
            }

            var newOrder = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.Now,
                ToTalAmount = (decimal)response.Amount
            };

            if (newOrder == null || orderItems == null)
            {
                TempData["Message"] = "Order data missing.";
                return RedirectToAction("PaymentFail");
            }

            db.Orders.Add(newOrder);
            db.SaveChanges();

            List<OrderDetail> orderDetails = orderItems.Select(orderItem => new OrderDetail
            {
                OrderId = newOrder.OrderId,
                ProductId = orderItem.ProductId,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity
            }).ToList();

            db.OrderDetails.AddRange(orderDetails);
            db.SaveChanges();

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }

        public ActionResult PaymentFail()
        {
            return RedirectToAction("Checkout");
        }

        public ActionResult PaymentSuccess()
        {
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

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Users, "CustomerId", "Name");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderId,CustomerId,OrderDate,ToTalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Users, "CustomerId", "Name", order.CustomerId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.CustomerId = new SelectList(db.Users, "CustomerId", "Name", order.CustomerId);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderId,CustomerId,OrderDate,ToTalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Users, "CustomerId", "Name", order.CustomerId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
