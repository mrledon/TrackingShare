using EmployeeTracking.Core.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class AttendanceManagementController : Controller
    {
        private TrackAttendanceRepo _trackAttendanceRepo;
        public AttendanceManagementController()
        {
            _trackAttendanceRepo = new TrackAttendanceRepo();
        }

        // GET: AttendanceManagement
        public ActionResult Index(int? page)
        {
            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            var data = _trackAttendanceRepo.GetAllAttendance();
            return View(data.ToPagedList(pageNumber, pageSize));
        }
    }
}