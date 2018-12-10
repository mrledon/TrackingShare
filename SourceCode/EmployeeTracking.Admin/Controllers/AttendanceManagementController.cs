using EmployeeTracking.Core.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Admin.Filters;

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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "Attendance")]
        public ActionResult Index(int? page)
        {
            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            var data = _trackAttendanceRepo.GetAllAttendance();
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        [CheckLoginFilter]
        public ActionResult ExportExcel()
        {
            var bin = _trackAttendanceRepo.GetExportTrackList();

            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}