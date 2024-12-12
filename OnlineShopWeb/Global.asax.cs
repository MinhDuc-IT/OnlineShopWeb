using Newtonsoft.Json;
using OnlineShopWeb.Attributes;
using OnlineShopWeb.Models;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OnlineShopWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;

            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                Server.ClearError();
                Response.Redirect("~/Error/PageNotFound");
            }
        }
    }
}
