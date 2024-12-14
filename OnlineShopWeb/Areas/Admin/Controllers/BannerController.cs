using OnlineShopWeb.Attributes;
using OnlineShopWeb.Data;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Areas.Admin.Controllers
{
    [AuthenticateUser]
    [AuthorizeUser(Roles = "Admin")]
    public class BannerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            IEnumerable<Banner> Banners = db.Banners.ToList();
            return View(Banners);
        }
        public ActionResult GetBannerByPage(int crrPage, int pageSize, string searchText = "")
        {
            var query = db.Banners.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(u => u.title.Contains(searchText));
            }

            var totalRecords = query.Count();

            var banners = query
                            .OrderBy(b => b.Id)
                            .Skip((crrPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()
                            .Select(b => new {
                                b.Id,
                                b.title,
                                b.description,
                                ImageBase64 = b.Image != null ? Convert.ToBase64String(b.Image) : null,
                                b.IsDeleted
                            }).ToList();

            return Json(new
            {
                success = true,
                data = banners,
                totalRecords = totalRecords,
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddBanner()
        {
            return View();
        }
        [HttpPost]
        public ActionResult postAddBanner()
        {
            try
            {
                var banner = new Banner();
                string title = Request.Form["title"];
                string description = Request.Form["description"];
                banner.title = title;
                banner.description = description;

                HttpPostedFileBase file = Request.Files["urlImage"];
                if (file != null)
                {
                    byte[] fileBytes;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(file.ContentLength);
                        banner.Image = fileBytes;
                    }
                }
                db.Banners.Add(banner);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult EditBanner(int id)
        {
            var banner = db.Banners.Find(id);
            return View(banner);
        }
        [HttpPost]
        public ActionResult postEditBanner()
        {
            try
            {
                int bannerId = Convert.ToInt32(Request.Form["Id"]);
                var banner = db.Banners.Find(bannerId);
                if (banner == null) return Json(new { success = false, message = "Banner not found" });

                string title = Request.Form["title"];
                string description = Request.Form["description"];
                bool IsDeleted = bool.Parse(Request.Form["IsDelete"]);
                banner.title = title;
                banner.description = description;
                banner.IsDeleted = IsDeleted;

                HttpPostedFileBase file = Request.Files["urlImage"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        banner.Image = binaryReader.ReadBytes(file.ContentLength);
                    }
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult deleteBanner(int Id)
        {
            try
            {
                var banner = db.Banners.Find(Id);
                banner.IsDeleted = true;
                db.SaveChanges();
                return Json(new { success = true, });
            }
            catch
            {
                return Json(new { success = false, });
            }
        }
    }
}