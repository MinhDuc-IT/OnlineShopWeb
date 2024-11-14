using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShopWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              name: "CategoryProduct",
              url: "danh-muc-san-pham/{id}",
              defaults: new { controller = "Product", action = "ProductCategory", id = UrlParameter.Optional },
              namespaces: new[] { "OnlineShopWeb.Controllers" }
          );
            routes.MapRoute(
              name: "CategoryProduct",
              url: "danh-muc-san-pham/{id}",
              defaults: new { controller = "Product", action = "ProductCategory", id = UrlParameter.Optional },
              namespaces: new[] { "OnlineShopWeb.Controllers" }
          );


            // elFinder's connector route
            //routes.MapRoute(null, "connector", new { controller = "File", action = "Index" });

            // Định nghĩa route cho elFinder
            routes.MapRoute(
                name: "ElFinderConnector",
                url: "el-finder-file-system/connector",
                defaults: new { controller = "FileSystem", action = "Connector" }
            );

            routes.MapRoute(
                name: "ElFinderThumb",
                url: "el-finder-file-system/thumb/{hash}",
                defaults: new { controller = "FileSystem", action = "Thumbs" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShopWeb.Controllers" }
            );
        }
    }
}
