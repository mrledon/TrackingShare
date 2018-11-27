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
                var i = 0;
                var ls5Days = new List<StatisticNumberStoreDay>();
                for (var day = DateTime.Now.AddDays(-4); day.Date <= DateTime.Now; day = day.AddDays(1))
                {
                    var storeFail = _data.master_store.Where(x=>x.CreatedDate.ToString("MM/dd/yyyy") == day.ToString("MM/dd/yyyy")).Count();
                    var storeSuccess = (from ms in _data.master_store join tr in _data.tracks on ms.Id equals tr.MasterStoreId where _data.track_session.Where(x=>x.TrackId == tr.Id).Count() > 0 select ms).Count() ;
                    var storeUnSubmit = (from ms in _data.master_store join tr in _data.tracks on ms.Id equals tr.MasterStoreId select ms).Count();
                    ls5Days[i++] = new StatisticNumberStoreDay() {
                            storeFail = storeFail
                        , storeSuccesss = storeSuccess
                        , storeUnSubmit = storeUnSubmit
                    };
                }

                return ls5Days;
            }
        }
    }

    public class StatisticNumberStoreDay
    {
        public int storeSuccesss { get; set; }
        public int storeFail { get; set; }
        public int storeUnSubmit { get; set; }
    }
}
