using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;


namespace EmployeeTracking.Core.Repositories
{
    public class EmployeeRepo
    {
        public List<employee> GetAllEmployee()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    return _data.employee.OrderBy(f => f.Name).ToList();
                }
            }
            catch (Exception)
            {
                return new List<employee>();
            }
        }
    }
}
