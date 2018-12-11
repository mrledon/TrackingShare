using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.CommonData;
using System.Web.Routing;

namespace EmployeeTracking.Admin.Controllers
{
    public class BasicController : Controller
    {

        public bool return_url { get; set; }

        #region " [ Protected overriding method ] "

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ////Get user model from session
            //var _user = filterContext.HttpContext.Session[CommonHelper.SESSION_LOGIN_NAME] as Utils.CommonModel.UserLoginModel;
            //if (_user != null)
            //{
            //    UserID = _user.UserID;
            //    this.SessionID = _user.SessionID;
            //    ViewBag.userID = UserID;
            //}
            //Stop if allowAnonymous attribute
            bool skipAuth = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                 || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                 || filterContext.IsChildAction;
            if (skipAuth)
            {
                return;
            }

            //var url = filterContext.HttpContext.Request.Url.ToString();
            //string controller = filterContext.RouteData.Values["controller"].ToString();
            //string action = filterContext.RouteData.Values["action"].ToString();
            //List<string> Error = new List<string>();
            //if (action != "Login")
            //{
            //    if (Session["Account"] == null)
            //    {
            //        if (filterContext.HttpContext.Request.IsAjaxRequest())
            //        {
            //            filterContext.Result = new JsonResult
            //            {

            //                Data = new
            //                {
            //                    Status = false,
            //                    Html = "Please login",
            //                    Message = "Please login"
            //                },
            //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //            };
            //        }

            //        else
            //        {
            //            if (return_url)
            //                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" }, { "ur", HttpUtility.UrlEncode(url) } });
            //            else
            //                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
            //        }
            //    }
            //}

            //if (Error.Count > 0)
            //{
            //   Session["Account"] = null;
            //    filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary{{ "controller", "Account" },
            //                        { "action", "Login"}
            //                        });
            //}
            //base.OnActionExecuting(filterContext);

            ////Check user login
            //if (_user == null || _user.UserID.ToString().Length == 0 || _user.UserName.Length == 0)
            //{
            //    //Return to login page
            //    throw new UnauthorizedAccessException();
            //}
            base.OnActionExecuting(filterContext);

            var _areaName = "";
            if (filterContext.RouteData.DataTokens["area"] != null)
            {
                _areaName = filterContext.RouteData.DataTokens["area"].ToString().ToLower();
            }
            var _controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            var _actionName = filterContext.RouteData.Values["action"].ToString().ToLower();

        }

        /// <summary>
        /// Execute after action method finish
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Execute before render view engine
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //Notification
            //if (TempData[CommonHelper.EXECUTE_RESULT] != null)
            //{
            //    Utils.CommonModel.ExecuteResultModel _result = TempData[CommonHelper.EXECUTE_RESULT] as Utils.CommonModel.ExecuteResultModel;
            //    ViewBag.msg = _result.Message;
            //    ViewBag.msgT = _result.Status == ResponseStatusCodeHelper.Success ? Notifier.TYPE.Success : Notifier.TYPE.Error;
            //    TempData.Clear();
            //}
            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// Execute when has some errors
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            filterContext.ExceptionHandled = true;

            ////Try to collect data was deleted or without permission
            //if (filterContext.Exception is DataAccessException)
            //{
            //    filterContext.Result = new ViewResult
            //    {
            //        ViewName = "~/Areas/Administrator/Views/AdmError/DataAccess.cshtml"
            //    };
            //    return;
            //}
            ////controller or service exception
            //if (filterContext.Exception is ControllerException || filterContext.Exception is ServiceException)
            //{
            //    filterContext.Result = new ViewResult
            //    {
            //        ViewName = "~/Areas/Administrator/Views/AdmError/Error.cshtml"
            //    };
            //    return;
            //}
            //Member dont have permission try to access
            //Only use in Common module (this class)
            //Return when check permision.
            if (filterContext.Exception is MemberAccessException)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Areas/Administrator/Views/AdmError/Forbiden.cshtml"
                };
                return;
            }
            //Error accur in business logic code
            //Only use in Common module (get access, get setting)
            if (filterContext.Exception is ApplicationException)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Areas/Administrator/Views/AdmError/Error.cshtml"
                };
                return;
            }
        }

        #endregion

        #region " [ Authorize ] "

        private string GetFunctionCode(string areaName, string controllerName, string actionName)
        {
            switch (controllerName)
            {
                case "Statistic":
                    return FunctionCode.STATISTIC;
                default:
                    return "";
            }
            return "";
        }

        #endregion

        public RedirectToRouteResult ToHome()
        {
            return RedirectToAction("Index", "Home");
        }
        public void SetMessage(TempDataDictionary TempData, string data, string cls = "")
        {
            TempData["MessagePage"] = data;
            TempData["cls"] = cls;
        }
        public user GetCurrentAccount()
        {
            try
            {
                return (user)Session["Account"];
            }
            catch
            {
                return null;
            }
        }
    }
}