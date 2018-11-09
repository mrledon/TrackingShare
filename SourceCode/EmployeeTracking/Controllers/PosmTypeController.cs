using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EmployeeTracking.Models;

namespace EmployeeTracking.Controllers
{
    public class PosmTypeController : ApiController
    {
        private employeetracking_devEntities db = new employeetracking_devEntities();

        // GET: api/users
        public IHttpActionResult GetPosmTypes()
        {
            List<string> posmTypes = new List<string>();
            string posmTypesString = ConfigurationManager.AppSettings["PosmTypesString"];

            if (!string.IsNullOrEmpty(posmTypesString))
            {
                posmTypes = posmTypesString.Split(',').ToList();
            }
            return Ok(posmTypes);
        }
        
    }
}