using OnlineShopWeb.Attributes;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthenticateUserAttribute { Order = 1 });
            //filters.Add(new AuthorizeUserAttribute { Order = 2 });
            filters.Add(new HandleErrorAttribute());
        }
    }
}
