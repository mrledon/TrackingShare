using EmployeeTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EmployeeTracking.Controllers.Api
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        private Employeetracking_devEntities db;
        public TestController()
        {
            
        }



        [Route("test")]
        [HttpGet]
        public IHttpActionResult Test()
        {
            db = new Employeetracking_devEntities();
            return Ok(db.employees.ToList());


            //return Ok(db.track_detail.ToList());
        }
    }
}