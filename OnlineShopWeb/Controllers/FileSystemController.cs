using ElFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Controllers
{
    [RoutePrefix("el-finder-file-system")]
    public class FileSystemController : Controller
    {
        // Phương thức Connector để xử lý các yêu cầu từ elFinder
        // Đường dẫn: /el-finder-file-system/connector
        [Route("connector")]
        public ActionResult Connector()
        {
            var connector = GetConnector();
            return connector.Process(HttpContext.Request);
        }

        // Phương thức Thumbs để tạo và truy vấn ảnh thumbnail
        // Đường dẫn: /el-finder-file-system/thumb/{hash}
        [Route("thumb/{hash}")]
        public ActionResult Thumbs(string hash)
        {
            var connector = GetConnector();
            return connector.GetThumbnail(HttpContext.Request, HttpContext.Response, hash);
        }

        // Khởi tạo Connector với cấu hình thư mục và URL
        private Connector GetConnector()
        {
            // Thư mục gốc lưu trữ file là ~/Content/files
            string pathroot = "Content/images";

            // Khởi tạo driver cho elFinder
            var driver = new FileSystemDriver();

            // Đường dẫn vật lý đến thư mục gốc
            string rootDirectoryPath = Path.Combine(Server.MapPath("~"), pathroot);

            // Kiểm tra nếu thư mục gốc không tồn tại
            DirectoryInfo directoryInfo = new DirectoryInfo(rootDirectoryPath);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Directory {rootDirectoryPath} not found.");
            }

            // Tạo URL tuyệt đối cho thư mục gốc và thumbnail            Uri uri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            Uri uri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            string url = $"{uri.Scheme}://{uri.Authority}/{pathroot}/";
            string urlthumb = $"{uri.Scheme}://{uri.Authority}/el-finder-file-system/thumb/";

            // Tạo đối tượng Root với DirectoryInfo và URL
            var root = new Root(directoryInfo, url)
            {
                IsReadOnly = false, // Có thể thay đổi nếu muốn thư mục chỉ đọc
                IsLocked = false,   // Khóa thư mục nếu cần
                Alias = "Files",    // Tên hiển thị cho thư mục
            };
            root.ThumbnailsSize = 100;
            driver.AddRoot(root);

            // Trả về đối tượng Connector
            return new Connector(driver);
        }
    }
}