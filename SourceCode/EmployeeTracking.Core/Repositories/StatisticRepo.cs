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
                    
                    ls5Days.Add(new StatisticNumberStoreDay()
                    {
                        Categorie = day.ToString("dd/MM/yyyy"),
                        Success = storeSuccess,
                        Fail = storeFail
                    });


                    //Set next day
                    day = day.AddDays(1);

                } while (day <= DateTime.Now);

                return ls5Days;
            }
        }

        public StoreDetailInfoViewModel getAllTrackSessionRestore(Guid id)
        {

            using (employeetracking_devEntities data = new employeetracking_devEntities())
            {
                //master_store masterStore1 = data.master_store.Where(x => x.Id == id).FirstOrDefault();
                master_store masterStore = data.Database.SqlQuery<master_store>(string.Format(@"SELECT *
                                                                                              from master_store ms
                                                                                              where ms.Id = '{0}'", id)).FirstOrDefault();
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
                var province = data.provinces.FirstOrDefault(x => x.Id == masterStore.ProvinceId);
                if (province != null)
                {
                    model.ProvinceName = province.Name;
                }
                var district = data.districts.FirstOrDefault(x => x.Id == masterStore.DistrictId);
                if (district != null)
                {
                    model.DistrictName = district.Name;
                }
                var ward = data.wards.FirstOrDefault(x => x.Id == masterStore.WardId);
                if (ward != null)
                {
                    model.WardName = ward.Name;
                }
                model.Region = masterStore.Region;
                model.StreetNames = masterStore.StreetNames;

                List<TrackSessionViewModel> lstTrackSession = data.Database.SqlQuery<TrackSessionViewModel>(string.Format(@"SELECT 
                                                                       ts.Id AS Id,
                                                                       ts.CreatedDate AS CreateDate,
                                                                       ts.Status AS Status
                                                                       from track_session ts
                                                                       JOIN track tr on ts.TrackId = tr.Id
                                                                       where tr.MasterStoreId = '{0}'", id)).ToList();

             
                List<StorePOSMViewModel> lstPosm = data.Database.SqlQuery<StorePOSMViewModel>(string.Format(@"SELECT 
                                                                       mt.Name AS PosmTypeName,
                                                                       SUM(tr_de.PosmNumber) AS CountPosmNumber
                                                                       from track_detail tr_de
                                                                       JOIN track_session tr_se on tr_de.TrackSessionId = tr_se.Id
                                                                       JOIN track tr on tr_se.TrackId = tr.Id
                                                                       JOIN media_type mt on tr_de.MediaTypeId = mt.Code
                                                                       where tr.MasterStoreId = '{0}' AND tr_de.MediaTypeId <> 'DEFAULT' AND tr_de.MediaTypeId <> 'SELFIE'
                                                                       GROUP BY tr_de.MediaTypeId ", id)).ToList();
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
