using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
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
        public List<AttendanceManagerModel> GetAllAttendance()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    var query = (from ta in _data.track_attendance
                                 join e in _data.employees
                                      on ta.EmployeeId equals e.Id
                                 select new AttendanceManagerModel
                                 {
                                     Id = ta.Id,
                                     Date = ta.Date,
                                     EmployeeId = ta.EmployeeId,
                                     EmployeeCode = e.Code,
                                     EmployeeName = e.Name,
                                     End = ta.End,
                                     Start = ta.Start
                                 }).ToList();

                    int index = 1;
                    foreach (var item in query)
                    {
                        item.DateString = item.Date.ToString("dd/MM/yyyy");
                        item.Index = index;
                        index++;
                    }
                    return query;
                }
            }
            catch (Exception ex)
            {
                return new List<AttendanceManagerModel>();
            }
        }

        public void AttendanceStart(track_attendance model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackAttend = _db.track_attendance.FirstOrDefault(
                    _ =>
                    _.EmployeeId == model.EmployeeId &&
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
                        Start = model.Start,
                        StartCoordinates = model.StartCoordinates
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
                    _.EmployeeId == model.EmployeeId &&
                    _.Date.Year == model.Date.Year &&
                    _.Date.Month == model.Date.Month &&
                    _.Date.Day == model.Date.Day
                );
                if (trackAttend == null)
                {
                    throw new Exception("Bạn chưa thực hiện điểm danh bắt đầu.");
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
                        trackAttend.EndCoordinates = model.EndCoordinates;
                        _db.SaveChanges();
                    }
                }
            }
        }



        public bool CheckAttendanceStart(track_attendance model)
        {
            try
            {
                bool rs = false;
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    if (_db.track_attendance.FirstOrDefault(
                        _ =>
                        _.EmployeeId == model.EmployeeId &&
                        _.Date.Year == model.Date.Year &&
                        _.Date.Month == model.Date.Month &&
                        _.Date.Day == model.Date.Day &&
                        _.Start != null
                    ) != null)
                    {
                        rs =  true;
                    }
                    return rs;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckAttendanceEnd(track_attendance model)
        {
            try
            {
                bool rs = false;
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    if (_db.track_attendance.FirstOrDefault(
                        _ =>
                        _.EmployeeId == model.EmployeeId &&
                        _.Date.Year == model.Date.Year &&
                        _.Date.Month == model.Date.Month &&
                        _.Date.Day == model.Date.Day &&
                        _.End != null
                    ) != null)
                    {
                        rs = true;
                    }
                    return rs;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
