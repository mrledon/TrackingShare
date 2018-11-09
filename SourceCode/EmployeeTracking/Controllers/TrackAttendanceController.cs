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
    public class TrackAttendanceController : ApiController
    {
        private employeetracking_devEntities db = new employeetracking_devEntities();

        // POST: api/users
        [ResponseType(typeof(track_attendance))]
        public IHttpActionResult PostTrackAttendance(track_attendance trackAttendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.track_attendance.Add(trackAttendance);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
            }

            return StatusCode(HttpStatusCode.OK);
        }

    }
}