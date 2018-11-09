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
    public class StoreController : ApiController
    {
        private employeetracking_devEntities db = new employeetracking_devEntities();
        
        // GET: api/store/5
        [ResponseType(typeof(master_store))]
        public IHttpActionResult GetStore(string id)
        {
            master_store store = db.master_store.Find(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

    }
}