using EmployeeTracking.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class WardRepo
    {
        public IList<ward> GetByDistrictId(long DistrictId)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.wards.Where(_ => _.DistrictId == DistrictId)
                    .OrderBy(_ => _.Type).ThenBy(_ => _.Name).ToList();
            }
        }
    }
}
