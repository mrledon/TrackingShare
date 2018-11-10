using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployeeTracking.API.Controllers
{
    public class MobileController : ApiController
    {
        private EmployeeTracking.Data.Database.employeetracking_devEntities _db;
        public MobileController()
        {
            _db = new Data.Database.employeetracking_devEntities();
        }
        // GET api/<controller>
        public object Get()
        {
            //return _db.employee.ToList();

            return _db.track.ToList();
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}