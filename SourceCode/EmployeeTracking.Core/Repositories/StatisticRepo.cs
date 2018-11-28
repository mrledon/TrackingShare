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
            using (employeetracking_devEntities db = new employeetracking_devEntities())
            {
                var ls5Days = new List<StatisticNumberStoreDay>();

                //set 4 day before
                var day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-4);

                do
                {
                    //Get end of day
                    var endofday = day.AddDays(1).AddSeconds(-1);

                    //Get data curent
                    var Todaytrack = db.tracks.Where(x => x.CreateDate >= day && x.CreateDate <= endofday);
                    
                    var storeFail = (from tr in Todaytrack where tr.StoreStatus == false select tr).Count();

                    var storeSuccess = (from ms in (from subms in db.master_store join tr in Todaytrack on subms.Id equals tr.MasterStoreId where tr.StoreStatus == true select subms) select ms).Count();

                    var UnSubmitTrack = db.tracks.Where(x => db.track_session
                                                        .Join(db.track_detail,
                                                        ts => ts.Id, td => td.TrackSessionId,
                                                        (ts, td) => new { TS = ts, TD = td.Id })
                                                        .Where(y => y.TS.TrackId == x.Id).Count() == 0);

                    var storeUnSubmit = (from ms in (from subms in db.master_store
                                                     join tr in UnSubmitTrack on subms.Id equals tr.MasterStoreId
                                                     select subms)
                                         select ms).Count();

                    ls5Days.Add(new StatisticNumberStoreDay()
                    {
                        Categorie = day.ToString("dd/MM/yyyy"),
                        Success = storeSuccess,
                        UnSubmit = storeUnSubmit,
                        Fail = storeFail
                    });


                    //Set next day
                    day = day.AddDays(1);

                } while (day <= DateTime.Now);

                //for (day; day.Date <= DateTime.Now; day = day.AddDays(1))
                //{
                //    var endofday = day.AddDays(1);
                //    var Todaytrack = db.tracks.Where(x => x.CreateDate >= day && x.CreateDate < endofday);


                //    var storeFail = (from tr in Todaytrack where tr.StoreStatus == false select tr).Count();

                //    var storeSuccess = (from ms in (from subms in db.master_store join tr in Todaytrack on subms.Id equals tr.MasterStoreId where tr.StoreStatus == true select subms) select ms).Count();

                //    var UnSubmitTrack = db.tracks.Where(x => db.track_session
                //                                        .Join(db.track_detail, 
                //                                        ts => ts.Id, td => td.TrackSessionId, 
                //                                        (ts, td) => new { TS = ts, TD = td.Id })
                //                                        .Where(y => y.TS.TrackId == x.Id).Count() == 0);

                //    var storeUnSubmit = (from ms in (from subms in db.master_store
                //                                     join tr in UnSubmitTrack on subms.Id equals tr.MasterStoreId
                //                                     select subms)
                //                         select ms).Count();
                //    var lsRateValue = new List<RateValues>();

                //    lsRateValue.Add(new RateValues() { value = storeSuccess, rate = "Thành công", });
                //    lsRateValue.Add(new RateValues() { value = storeSuccess, rate = "Không thành công" });
                //    lsRateValue.Add(new RateValues() { value = storeSuccess, rate = "Chưa submit" });


                //    ls5Days.Add(new StatisticNumberStoreDay() {
                //        categorie = day,
                //        categorieString = day.ToString("dd/MM/yyyy"),
                //        values = lsRateValue
                //    });
                //}

                return ls5Days;
            }
        }

        public List<StoreDetailInfoViewModel> getAllTrackSessionRestore(Guid id)
        {
            using (employeetracking_devEntities data = new employeetracking_devEntities())
            {
                var model = (from ms in data.master_store where ms.Id == id select new StoreDetailInfoViewModel() {
                    Code = ms.Code,
                    Name = ms.Name,
                    StoreType = data.master_store_type.Where(x=>x.Id == ms.StoreType).FirstOrDefault().Name,
                    HouseNumber = ms.HouseNumber, ProvinceName = data.provinces.FirstOrDefault(x=>x.Id == ms.ProvinceId).Name,
                    DistrictName = data.districts.FirstOrDefault(x => x.Id == ms.DistrictId).Name,
                    WardName = data.wards.FirstOrDefault(x => x.Id == ms.WardId).Name,
                    Region = ms.Region,
                    StreetNames = ms.StreetNames,
                    trackSessions = (from ts in data.track_session join tr in data.tracks on ts.TrackId equals tr.Id where tr.MasterStoreId == ms.Id select new TrackSessionViewModel() { Id = ts.Id, CreateDate = ts.CreatedDate, Status = ts.Status }).ToList()
                }).ToList();

                return model;
            }
        }
    }
}
