using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class RoleRepo
    {
        public List<RoleModel> List()
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    return _db.roles.Select(m => new RoleModel() { Code = m.Code, Name = m.Name }).ToList();

                }
            }
            catch
            {
                return new List<RoleModel>();
            }
        }

    }
}
