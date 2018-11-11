using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Core;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Admin.Filters;

namespace EmployeeTracking.Admin.Controllers
{
    public class HomeController : BasicController
    {
        [CheckLoginFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}