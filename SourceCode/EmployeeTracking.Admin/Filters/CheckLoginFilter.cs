using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmployeeTracking.Admin.Filters
{
    public class CheckLoginFilter : ActionFilterAttribute
    {
        public bool return_url { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url.ToString();
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            List<string> Error = new List<string>();
            if (action != "Login")
            {
                if (HttpContext.Current.Session["Account"] == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult
                        {

                            Data = new
                            {
                                Status = false,
                                Html = "Please login",
                                Message = "Please login"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }

                    else
                    {
                        if (return_url)
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" }, { "ur", HttpUtility.UrlEncode(url) } });
                        else
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                    }
                }
            }

            if (Error.Count > 0)
            {
                HttpContext.Current.Session["Account"] = null;
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary{{ "controller", "Account" },
                                    { "action", "Login"}
                                    });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}