using EmployeeTracking.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class ProvinceRepo
    {
        public IList<province> GetAll()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.provinces
                    .OrderBy(_ => _.Type).ThenBy(_ => _.Name).ToList();
            }
        }
    }
}
