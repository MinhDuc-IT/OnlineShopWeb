using Newtonsoft.Json;
using OnlineShopWeb.Helpers;
using OnlineShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopWeb.Attributes
{
    public class AuthenticateUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string currentUrl = httpContext.Request.Url.AbsolutePath;
            string currentController = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            string currentAction = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();

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
                    if (user.Role == "Admin")
                    {
                        return false;
                    }
                }
            }

            if (user == null)
            {
                if (currentController == "Home" && currentAction == "Index")
                {
                    return true;
                }
                return false;
            }

            if (user.Role == "Admin" &&
                !currentUrl.StartsWith("/admin") &&
                !currentUrl.StartsWith("/Admin") &&
                !currentUrl.StartsWith("/Account"))
            {
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string currentUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
            string currentController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string currentAction = filterContext.ActionDescriptor.ActionName;

            var user = filterContext.HttpContext.Session["User"] as User;
            if (user != null)
            {
                if (user.Role == "Admin")
                {
                    if (!currentUrl.StartsWith("/Admin"))
                    {
                        filterContext.Result = new RedirectResult("/Admin");
                        return;
                    }
                }
            }
            else
            {

                if (currentController != "Account" && currentAction != "Login")
                {
                    filterContext.HttpContext.Session["CurrentUrl"] = "/" + currentController + "/" + currentAction;
                    filterContext.Result = new RedirectResult("/Account/Login");
                }
            }

            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}