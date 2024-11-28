using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShopWeb.Helpers;
using OnlineShopWeb.Models;
using System.Text.RegularExpressions;

namespace OnlineShopWeb.Attributes
{
    public class AuthenticateUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string currentUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
            string currentController = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string currentAction = filterContext.ActionDescriptor.ActionName;

            if (filterContext.HttpContext.Session["User"] == null)
            {
                var cookie = filterContext.HttpContext.Request.Cookies["AuthCookie"];
                if (cookie != null)
                {
                    string encryptData = cookie.Value;
                    string userJson = Security.Decrypt(encryptData);
                    var user = JsonConvert.DeserializeObject<User>(userJson);
                    filterContext.HttpContext.Session["User"] = user;
                    if (user.Role == "Admin")
                    {
                        if (!currentUrl.StartsWith("/Admin"))
                        {
                            filterContext.Result = new RedirectResult("/Admin");
                            return;
                        }
                    }
                    else
                    {
                        base.OnActionExecuting(filterContext);
                    }
                }
                else
                {
                    if (currentController == "Home" && currentAction == "Index")
                    {
                        base.OnActionExecuting(filterContext);
                        return;
                    }

                    if (currentController != "Account" && currentAction != "Login")
                    {
                        filterContext.HttpContext.Session["CurrentUrl"] = "/" + currentController + "/" + currentAction;
                        filterContext.Result = new RedirectResult("/Account/Login");
                    }
                }
            }
            else
            {
                var user = filterContext.HttpContext.Session["User"] as User;
                if (user != null && user.Role == "Admin")
                {
                    if (!currentUrl.StartsWith("/Admin") && !currentUrl.StartsWith("/admin") && !currentUrl.StartsWith("/Account"))
                    {
                        if (currentController != "Error" && currentAction != "PageNotFound")
                        {
                            filterContext.Result = new RedirectResult("/Error/PageNotFound");
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
