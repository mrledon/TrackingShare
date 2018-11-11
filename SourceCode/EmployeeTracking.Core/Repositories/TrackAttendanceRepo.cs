using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class TrackAttendanceRepo
    {
        public void AttendanceStart(track_attendance model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackAttend = _db.track_attendance.FirstOrDefault(
                    _ =>
                    _.Date.Year == model.Date.Year &&
                    _.Date.Month == model.Date.Month &&
                    _.Date.Day == model.Date.Day
                );
                if (trackAttend == null)
                {
                    var newtrackAttend = new track_attendance()
                    {
                        CreatedBy = model.EmployeeId,
                        Date = model.Date,
                        EmployeeId = model.EmployeeId,
                        End = null,
                        Id = Guid.NewGuid(),
                        IsActive = true,
                        Start = model.Start
                    };
                    _db.track_attendance.Add(newtrackAttend);
                    _db.SaveChanges();
                }
                //else
                //    throw new Exception("");
            }
        }
        public void AttendanceEnd(track_attendance model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackAttend = _db.track_attendance.FirstOrDefault(
                    _ =>
                    _.Date.Year == model.Date.Year &&
                    _.Date.Month == model.Date.Month &&
                    _.Date.Day == model.Date.Day
                );
                if (trackAttend == null)
                {
                    throw new Exception("please call Attendance Start before");
                }
                else
                {
                    if (trackAttend.End.HasValue)
                    {
                        //throw new Exception("Attendance End");
                    }
                    else
                    {
                        trackAttend.End = model.End;

                        trackAttend.ModifiedBy = model.EmployeeId;
                        trackAttend.ModifiedDate = DateTime.Now;

                        _db.SaveChanges();
                    }
                }
            }
        }
    }
}
