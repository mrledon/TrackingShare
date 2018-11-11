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
    public class CategoryController : BasicController
    {
        // GET: Category

            private EmployeeRepo _employeeRepo;
        public CategoryController()
        {
            _employeeRepo = new EmployeeRepo();
        }


        [CheckLoginFilter]
        public ActionResult Employeemanager()
        {

            //var ListEmployee = _employeeRepo.GetAllEmployee();

            return View();
        }
        public ActionResult Storemanager()
        {
            return View();
        }
        public ActionResult Attendancemanagement()
        {
            return View();
        }
        public ActionResult Imagemanagement()
        {
            return View();
        }
    }
}