using EmployeeTracking.Data.CommonData;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    class StatisticRepo
    {
        public List<StatisticNumberStoreDay> getStoreNumber5Days()
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var ls5Days = new List<StatisticNumberStoreDay>();
                for (var day = DateTime.Now.AddDays(-4); day.Date <= DateTime.Now; day = day.AddDays(1))
                {
                    ls5Days.Add(new StatisticNumberStoreDay() { storeFail = 9, storeSucesss = 99, storeUnSubmit = 50 });
                }

                return ls5Days;
            }
        }
    }

    public class StatisticNumberStoreDay
    {
        public int storeSucesss { get; set; }
        public int storeFail { get; set; }
        public int storeUnSubmit { get; set; }
    }
}
