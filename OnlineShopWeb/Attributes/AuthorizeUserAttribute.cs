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

            if (user == null)
            {
                var cookie = httpContext.Request.Cookies["AuthCookie"];
                if (cookie != null)
                {
                    string encryptData = cookie.Value;
                    string userJson = Security.Decrypt(encryptData);
                    user = JsonConvert.DeserializeObject<User>(userJson);

                    httpContext.Session["User"] = user;
                }
            }

            if (user == null || string.IsNullOrEmpty(user.Role) || string.IsNullOrEmpty(Roles))
            {
                return false;
            }

            string[] requiredRoles = Roles.Split(',');
            return Array.Exists(requiredRoles, role => role.Equals(user.Role, StringComparison.OrdinalIgnoreCase));
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.Session["User"] as User;
            if (user == null)
            {
                string currentController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string currentAction = filterContext.ActionDescriptor.ActionName;
                if (currentController != "Account" && currentAction != "Login")
                {
                    filterContext.HttpContext.Session["CurrentUrl"] = "/" + currentController + "/" + currentAction;
                }
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            else
            {
                filterContext.Result = new RedirectResult("/Error/PageNotFound"); 
            }
        }
    }
}
