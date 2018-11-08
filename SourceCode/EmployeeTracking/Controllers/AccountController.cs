using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class AccountController : BasicController
    {
        // GET: Account
        public ActionResult Login()
        {

            return RedirectToAction("Index","Home");
        }

        

    }
}