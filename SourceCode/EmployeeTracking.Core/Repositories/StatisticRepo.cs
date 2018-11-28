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
    public class StatisticRepo
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

        public List<StoreDetailInfoViewModel> getAllTrackSessionRestore(Guid id)
        {
            using (employeetracking_devEntities data = new employeetracking_devEntities())
            {
                var model = (from ms in data.master_store where ms.Id == id select new StoreDetailInfoViewModel() {
                    Code = ms.Code, Name = ms.Name, StoreType = data.master_store_type.Where(x=>x.Id == ms.StoreType).FirstOrDefault().Name
                    , HouseNumber = ms.HouseNumber, ProvinceName = data.provinces.FirstOrDefault(x=>x.Id == ms.ProvinceId).Name, DistrictName = data.districts.FirstOrDefault(x => x.Id == ms.DistrictId).Name
                    , WardName = data.wards.FirstOrDefault(x => x.Id == ms.WardId).Name, Region = ms.Region, StreetNames = ms.StreetNames, trackSessions = (from ts in data.track_session join tr in data.tracks on ts.TrackId equals tr.Id where tr.MasterStoreId == ms.Id select new TrackSessionViewModel() { Id = ts.Id, CreateDate = ts.CreatedDate, Status = ts.Status }).ToList()
                }).ToList();
                return model;
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
