using OnlineShopWeb.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShopWeb.Helpers;

namespace OnlineShopWeb.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string Roles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.Session["User"] as User;

            if (user == null || string.IsNullOrEmpty(user.Role) || string.IsNullOrEmpty(Roles))
            {
                return false;
            }

            string[] requiredRoles = Roles.Split(',');
            return Array.Exists(requiredRoles, role => role.Equals(user.Role, StringComparison.OrdinalIgnoreCase));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Error/PageNotFound");
        }
    }
}
