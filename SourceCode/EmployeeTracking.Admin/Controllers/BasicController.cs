using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;

namespace EmployeeTracking.Admin.Controllers
{
    public class BasicController : Controller
    {
        public RedirectToRouteResult ToHome()
        {
            return RedirectToAction("Index", "Home");
        }
        public void SetMessage(TempDataDictionary TempData, string data, string cls = "")
        {
            TempData["MessagePage"] = data;
            TempData["cls"] = cls;
        }
        public users GetCurrentAccount()
        {
            try
            {
                return (users)Session["Account"];
            }
            catch
            {
                return null;
            }
        }
    }
}