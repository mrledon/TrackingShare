using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class CategoryController : BasicController
    {
        // GET: Category
        public ActionResult Employeemanager()
        {
            return View();
        }
        public ActionResult Storemanager()
        {
            return View();
        }
        public ActionResult Attendancemanagement ()
        {
            return View();    
        }
        public ActionResult Imagemanagement()
        {
            return View();
        }
    }
}