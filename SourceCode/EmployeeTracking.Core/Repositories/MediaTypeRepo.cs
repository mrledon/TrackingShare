using EmployeeTracking.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class MediaTypeRepo
    {
        public media_type GetByCode(string Code)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.media_type.FirstOrDefault(_ => _.Code == Code);
            }
        }

        public List<media_type> GetAll()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.media_type.Where(_ => _.IsActive == true && _.Code != "DEFAULT").ToList();
            }
        }
    }
}
