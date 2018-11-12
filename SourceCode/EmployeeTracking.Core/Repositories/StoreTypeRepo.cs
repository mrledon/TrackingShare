using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;

namespace EmployeeTracking.Core.Repositories
{
    public class StoreTypeRepo
    {
        public IList<master_store_type> GetAll()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.master_store_type
                    .OrderBy(_ => _.Order).ToList();
            }
        }
    }
}
