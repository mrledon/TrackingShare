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
        [Route("test")]
        [HttpGet]
        public IHttpActionResult Test()
        {
            string result = "Okiew";
            return Ok(result);
        }
    }
}