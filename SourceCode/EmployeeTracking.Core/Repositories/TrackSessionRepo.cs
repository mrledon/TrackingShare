using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;

namespace EmployeeTracking.Core.Repositories
{
    public class TrackSessionRepo
    {
        public string Insert(track_session model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                _db.track_session.Add(model);
                _db.SaveChanges();
                return model.Id;
            }
        }
    }
}
