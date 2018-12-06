using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EmployeeTracking.Admin.Filters
{
    public class RoleFilter : ActionFilterAttribute
    {
        public string ActionName { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            List<String> session = (List<String>)HttpContext.Current.Session["Roles"]; // array

            //var session = new List<String>()
            //{
            //    "EditFilter", "EditGroup"
            //};
            //if (ActionName != "Login")
            if (!session.Contains(ActionName))
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
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" }});
            }
            base.OnActionExecuting(filterContext);
        }
    }
}