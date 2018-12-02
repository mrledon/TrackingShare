using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Admin.Filters;

namespace EmployeeTracking.Admin.Controllers
{
    public class KeepAliveController : Controller
    {
        // GET: KeepAlive
        [HttpPost]
        [CheckLoginFilter]
        public EmptyResult keepalive()
        {
            return new EmptyResult();
        }
    }
}