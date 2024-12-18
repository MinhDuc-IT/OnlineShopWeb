using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace OnlineShopWeb.Areas.Admin.Controllers
{

    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class ProductController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Products1
        public ActionResult Index(string searchQuery, string searchBy, int? page)
        {
            // Lấy danh sách sản phẩm từ database
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category).AsQueryable();

            // Lọc sản phẩm dựa trên tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower(); // Chuyển chữ thường để tìm kiếm không phân biệt
                switch (searchBy)
                {
                    case "ProductName":
                        products = products.Where(p => p.Name.ToLower().Contains(searchQuery));
                        break;
                    case "Brand":
                        products = products.Where(p => p.Brand != null && p.Brand.Name.ToLower().Contains(searchQuery));
                        break;
                    case "Category":
                        products = products.Where(p => p.Category != null && p.Category.Name.ToLower().Contains(searchQuery));
                        break;
                    default:
                        products = products.Where(p => p.Name.ToLower().Contains(searchQuery));
                        break;
                }
            }

            // Sắp xếp theo tên
            products = products.OrderBy(p => p.Name);

            // Tính toán phân trang
            int pageSize = 5;
            int pageNumber = page ?? 1;
            int totalItems = products.Count();
            var pagedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Truyền thông tin phân trang và tìm kiếm vào ViewBag
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SearchBy = searchBy;

            return View(pagedProducts);
        }

        // GET: Admin/Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products1/Create
        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Admin/Products1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]  // Thêm attribute này
        public ActionResult Create([Bind(Include = "ProductId,Name,BrandId,Description,Image,Price,Stock,CategoryId,IsDeleted,CostPrice")] Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (Image != null && Image.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        // Save the image as a base64 string
                        Image.InputStream.CopyTo(memoryStream);
                        product.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                // Handle description field (since it's contenteditable)
                var description = Request.Form["Description"];
                product.IsDeleted = false;
                product.Description = description;

                // Check if product already exists (based on the specified conditions)
                var existingProduct = db.Products.FirstOrDefault(p =>
                    p.Name == product.Name &&
                    p.BrandId == product.BrandId &&
                    p.Price == product.Price &&
                    p.CategoryId == product.CategoryId &&
                    !p.IsDeleted);

                if (existingProduct != null)
                {
                    // If a product is found, increase the stock
                    existingProduct.Stock += product.Stock;
                    db.Entry(existingProduct).State = EntityState.Modified;
                }
                else
                {
                    // If no product is found, add the new product
                    db.Products.Add(product);
                }

                db.SaveChanges();

                // Redirect to the Index page after saving
                return RedirectToAction("Index");
            }

            // If ModelState is invalid, re-populate the drop-down lists and return the view
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }




        // GET: Admin/Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ProductId,Name,BrandId,Description,Image,Price,Stock,CategoryId,IsDeleted,CostPrice")] Product product,
                     HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý ảnh chính
                    if (Image != null && Image.ContentLength > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            Image.InputStream.CopyTo(memoryStream);
                            product.Image = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        // Lấy ảnh cũ nếu không tải ảnh mới
                        var existingProduct = db.Products.AsNoTracking()
                            .FirstOrDefault(p => p.ProductId == product.ProductId);
                        if (existingProduct != null)
                        {
                            product.Image = existingProduct.Image;
                        }
                    }

                    // Xử lý Description
                    var description = Request.Form["Description"];
                    product.Description = description;

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewBag.BrandId = new SelectList(db.Brands, "BrandId", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }



        // GET: Admin/Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
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