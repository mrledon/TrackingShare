using EmployeeTracking.Data.Database;
using System.Linq;

namespace EmployeeTracking.Core.Repositories
{
    public class StoreRepo
    {
        public master_store GetByCode(string code)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    return _db.master_store.Where(_ => _.Code == code).FirstOrDefault();
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }
    }
}
