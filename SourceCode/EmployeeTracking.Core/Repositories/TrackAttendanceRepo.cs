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
            }
        }
    }
}
