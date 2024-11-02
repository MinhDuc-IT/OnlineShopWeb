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

namespace OnlineShopWeb.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private VnPayService vnPayService = new VnPayService();

        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutRequest checkoutRequest)
        {
            if (checkoutRequest.PaymentMethod == "Vnpay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = checkoutRequest.Total,
                    CreatedDate = DateTime.Now,
                    Description = $"{checkoutRequest.FullName}-{checkoutRequest.Mobile}",
                    FullName = checkoutRequest.FullName,
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
            var httpContext = System.Web.HttpContext.Current;

            var response = vnPayService.PaymentExecute(httpContext.Request.QueryString);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Lưu đơn hàng vào database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }


        public ActionResult PaymentFail()
        {
            return View();
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
