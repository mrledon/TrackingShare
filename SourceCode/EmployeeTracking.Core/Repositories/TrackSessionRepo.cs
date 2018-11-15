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
        public track_session Insert(track_session model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    _db.track_session.Add(model);
                    _db.SaveChanges();
                    return model;
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
        public track_session getById(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    return _db.track_session.Where(x=>x.Id==id).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }

        public bool updateStatus(string id, bool status)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    var q= _db.track_session.Where(x => x.Id == id).FirstOrDefault();
                    q.Status = status;
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }
    }
}
