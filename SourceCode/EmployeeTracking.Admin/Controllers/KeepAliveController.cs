using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class KeepAliveController : Controller
    {
        // GET: KeepAlive
        [HttpPost]
        public EmptyResult keepalive()
        {
            return new EmptyResult();
        }
    }
}