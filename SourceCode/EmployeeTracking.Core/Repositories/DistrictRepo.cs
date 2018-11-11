using EmployeeTracking.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class DistrictRepo
    {
        public IList<district> GetByProvinceId(long ProvinceId)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.districts.Where(_ => _.ProvinceId == ProvinceId)
                    .OrderBy(_ => _.Type).ThenBy(_ => _.Name).ToList();
            }
        }
    }
}
