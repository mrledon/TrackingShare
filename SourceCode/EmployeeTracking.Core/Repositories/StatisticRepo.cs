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
                    ls5Days.Add(new StatisticNumberStoreDay()
                    {
                        categorie = day,
                        values = lsRateValue
                    });
                }

                return ls5Days;
            }
        }

        public StoreDetailInfoViewModel getAllTrackSessionRestore(Guid id)
        {

            using (employeetracking_devEntities data = new employeetracking_devEntities())
            {
                //    var model = (from ms in data.master_store where ms.Id == id select new StoreDetailInfoViewModel() {
                //        Code = ms.Code,
                //        Name = ms.Name,
                //        StoreType = data.master_store_type.Where(x=>x.Id == ms.StoreType).FirstOrDefault().Name,
                //        HouseNumber = ms.HouseNumber, ProvinceName = data.provinces.FirstOrDefault(x=>x.Id == ms.ProvinceId).Name,
                //        DistrictName = data.districts.FirstOrDefault(x => x.Id == ms.DistrictId).Name,
                //        WardName = data.wards.FirstOrDefault(x => x.Id == ms.WardId).Name,
                //        Region = ms.Region,
                //        StreetNames = ms.StreetNames,
                //        trackSessions = (from ts in data.track_session join tr in data.tracks on ts.TrackId equals tr.Id where tr.MasterStoreId == ms.Id select new TrackSessionViewModel() { Id = ts.Id, CreateDate = ts.CreatedDate, Status = ts.Status }).ToList()
                //}).ToList();
                master_store masterStore = data.master_store.Where(x => x.Id == id).FirstOrDefault();
                StoreDetailInfoViewModel model = new StoreDetailInfoViewModel();

                if (masterStore.Code != null)
                {
                    model.Code = masterStore.Code;
                }

                model.Name = masterStore.Name;
                if (data.master_store_type.Where(x => x.Id == masterStore.StoreType).FirstOrDefault() != null)
                {
                    model.StoreType = data.master_store_type.Where(x => x.Id == masterStore.StoreType).FirstOrDefault().Name;
                }
                model.PhoneNumber = masterStore.PhoneNumber;
                model.HouseNumber = masterStore.HouseNumber;
                if (data.provinces.FirstOrDefault(x => x.Id == masterStore.ProvinceId) != null)
                {
                    model.ProvinceName = data.provinces.FirstOrDefault(x => x.Id == masterStore.ProvinceId).Name;
                }
                    if(data.districts.FirstOrDefault(x => x.Id == masterStore.DistrictId)!=null)
                {
                    model.DistrictName = data.districts.FirstOrDefault(x => x.Id == masterStore.DistrictId).Name;
                }
                if (data.wards.FirstOrDefault(x => x.Id == masterStore.WardId)!=null)
                {
                    model.WardName = data.wards.FirstOrDefault(x => x.Id == masterStore.WardId).Name;
                }
                model.Region = masterStore.Region;
                model.StreetNames = masterStore.StreetNames;
                List<TrackSessionViewModel> lstTrackSession = (from ts in data.track_session join tr in data.tracks on ts.TrackId equals tr.Id where tr.MasterStoreId == id select new TrackSessionViewModel() { Id = ts.Id, CreateDate = ts.CreatedDate, Status = ts.Status }).ToList();

                List<StorePOSMViewModel> lstPosm = (from tr_de in data.track_detail
                                                    join tr_se in data.track_session on tr_de.TrackSessionId equals tr_se.Id
                                                    join tr in data.tracks on tr_se.TrackId equals tr.Id
                                                    join mt in data.media_type on tr_de.MediaTypeId equals mt.Code
                                                    where tr.MasterStoreId == id && tr_de.MediaTypeId != "DEFAULT" && tr_de.MediaTypeId != "SELFIE"
                                                    group new { mt, tr_de } by new { tr_de.MediaTypeId } into pg
                                                    select new StorePOSMViewModel() { PosmTypeName = pg.FirstOrDefault().mt.Name, CountPosmNumber = pg.Sum(tr_de => tr_de.tr_de.PosmNumber) }).ToList();
                model.trackSessions = lstTrackSession;
                model.listPosm = lstPosm;
                return model;
            }
        }

        public List<TrackDetailViewModel> GetTrackDetailListByTrackSessionId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from rs in (from ts in _db.track_session
                                         join td in _db.track_detail on ts.Id equals td.TrackSessionId
                                         join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                         where ts.Id == id
                                         select new
                                         {
                                             Id = td.Id,
                                             FileName = td.FileName,
                                             Url = td.Url,
                                             MediaTypeId = td.MediaTypeId,
                                             MediaTypeName = mt.Name,
                                             MediaTypeOrder = mt.OrderNumber,
                                             PosmNumber = td.PosmNumber,
                                             CreateDate = td.CreateDate,
                                             MediaTypeSub = td.MediaTypeSub
                                         })
                             group rs by new
                             {
                                 rs.MediaTypeId,
                                 rs.MediaTypeName,
                                 rs.MediaTypeOrder
                             } into g
                             select new TrackDetailViewModel
                             {
                                 MediaTypeId = g.Key.MediaTypeId,
                                 MediaTypeName = g.Key.MediaTypeName,
                                 MediaTypeOrder = g.Key.MediaTypeOrder,
                                 TrackDetailImages = g.Select(x => new TrackDetailImageViewModel
                                 {
                                     Id = x.Id,
                                     FileName = x.FileName,
                                     Url = x.Url,
                                     PosmNumber = x.PosmNumber,
                                     CreateDate = x.CreateDate,
                                     MediaTypeSub = x.MediaTypeSub
                                 })
                             }).OrderBy(x => x.MediaTypeOrder).ToList();

                return model;
            }
        }
    }
}
