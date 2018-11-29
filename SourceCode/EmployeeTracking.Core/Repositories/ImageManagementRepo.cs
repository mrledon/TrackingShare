using EmployeeTracking.Data.CommonData;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class ImageManagementRepo
    {
        public List<ImageManagementViewModel> GetTrackList()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from rs in (from tr in _db.tracks
                                         join store in _db.master_store on tr.MasterStoreId equals store.Id
                                         join em in _db.employees on tr.EmployeeId equals em.Id
                                         join tr_se in _db.track_session.DefaultIfEmpty() on tr.Id equals tr_se.TrackId
                                         select new
                                         {
                                             Id = tr.Id,
                                             EmployeeId = tr.EmployeeId,
                                             EmployeeName = em.Name,
                                             MasterStoreId = tr.MasterStoreId,
                                             MasterStoreCode = store.Code,
                                             MasterStoreName = tr.MaterStoreName,
                                             CreateDate = tr.Date,
                                             StoreStatus = tr.StoreStatus,
                                             Region = tr.Region,
                                             TrackSessionId = tr_se.Id,
                                             TrackCreateDate = tr_se.Date,
                                             SessionStatus = tr_se.Status
                                         })
                             group rs by new
                             {
                                 rs.Id,
                                 rs.EmployeeId,
                                 rs.EmployeeName,
                                 rs.MasterStoreId,
                                 rs.MasterStoreCode,
                                 rs.MasterStoreName,
                                 rs.CreateDate,
                                 rs.StoreStatus,
                                 rs.Region,
                                 rs.SessionStatus
                             } into g
                             select new ImageManagementViewModel()
                             {
                                 Id = g.Key.Id,
                                 EmployeeId = g.Key.EmployeeId,
                                 EmployeeName = g.Key.EmployeeName,
                                 MasterStoreId = g.Key.MasterStoreId,
                                 MasterStoreCode = g.Key.MasterStoreCode,
                                 MasterStoreName = g.Key.MasterStoreName,
                                 CreateDate = g.Key.CreateDate,
                                 StoreStatus = g.Key.StoreStatus,
                                 Region = g.Key.Region,
                                 TrackSessions = g.Select(x => new TrackSessionViewModel
                                 {
                                     Id = x.TrackSessionId,
                                     CreateDate = x.TrackCreateDate,
                                     Status = x.SessionStatus
                                 })
                             }).OrderByDescending(x => x.CreateDate).ToList();

                return model;
            }
        }

        public StoreInfoViewModel GetStoreInfoByTrackSessionId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    StoreInfoViewModel storeInfo = new StoreInfoViewModel();
                    var model = _db.Database.SqlQuery<StoreInfoViewModel>(string.Format(@"SELECT 
                                                                                        sbstore.Id as Id,
                                                                                        sbstore.Code as SbvpCode,
                                                                                        sbstore.Name as SbvpName,
                                                                                        sbstore.PhoneNumber as SbvpPhone,
                                                                                        sbsttype.Name as SbvpType,
                                                                                        sbpr.Name as SbvpProvince,
                                                                                        sbdis.Name as SbvpDistrict,
                                                                                        sbwa.Name as SbvpWard,
                                                                                        sbstore.StreetNames as SbvpStreetName,
                                                                                        sbstore.HouseNumber as SbvpHouseNumber,
                                                                                        sbstore.Code as DigixCode,
                                                                                        tr.MaterStoreName as DigixName,
                                                                                        tr.PhoneNumber as DigixPhone,
                                                                                        dxsttype.Name as DigixType,
                                                                                        dxpr.Name as DigixProvince,
                                                                                        dxdis.Name as DigixDistrict,
                                                                                        dxwa.Name as DigixWard,
                                                                                        tr.StreetNames as DigixStreetName,
                                                                                        tr.HouseNumber as DigixHouseNumber,
                                                                                        tr.StoreIsChanged
	                                                                                        from track_session trse 
	                                                                                        LEFT JOIN track tr on trse.TrackId = tr.Id
	                                                                                        LEFT JOIN master_store sbstore on tr.MasterStoreId = sbstore.Id
	                                                                                        LEFT JOIN master_store_type sbsttype on sbstore.StoreType = sbsttype.Id
	                                                                                        LEFT JOIN province sbpr on sbstore.ProvinceId = sbpr.Id
	                                                                                        LEFT JOIN district sbdis on sbstore.DistrictId = sbdis.Id
	                                                                                        LEFT JOIN ward sbwa on sbstore.WardId = sbwa.Id
	                                                                                        LEFT JOIN master_store_type dxsttype on tr.StoreType = dxsttype.Id
	                                                                                        LEFT JOIN province dxpr on tr.ProvinceId = dxpr.Id
	                                                                                        LEFT JOIN district dxdis on tr.DistrictId = dxdis.Id
	                                                                                        LEFT JOIN ward dxwa on tr.WardId = dxwa.Id
	                                                                                        where trse.Id = '{0}'", id)).FirstOrDefault();


                    if (model.StoreIsChanged ?? true)
                    {
                        storeInfo.Id = model.Id;
                        storeInfo.SbvpCode = model.SbvpCode;
                        storeInfo.SbvpName = model.SbvpName;
                        storeInfo.SbvpPhone = model.SbvpPhone;
                        storeInfo.SbvpType = model.SbvpType;
                        storeInfo.SbvpProvince = model.SbvpProvince;
                        storeInfo.SbvpDistrict = model.SbvpDistrict;
                        storeInfo.SbvpWard = model.SbvpWard;
                        storeInfo.SbvpStreetName = model.SbvpStreetName;
                        storeInfo.SbvpHouseNumber = model.SbvpHouseNumber;

                        storeInfo.DigixCode = model.DigixCode;
                        storeInfo.DigixName = model.DigixName;
                        storeInfo.DigixPhone = model.DigixPhone;
                        storeInfo.DigixType = model.DigixType;
                        storeInfo.DigixProvince = model.DigixProvince;
                        storeInfo.DigixDistrict = model.DigixDistrict;
                        storeInfo.DigixWard = model.DigixWard;
                        storeInfo.DigixStreetName = model.DigixStreetName;
                        storeInfo.DigixHouseNumber = model.DigixHouseNumber;
                        storeInfo.StoreIsChanged = model.StoreIsChanged;
                    }
                    else
                    {
                        storeInfo.Id = model.Id;
                        storeInfo.SbvpCode = model.SbvpCode;
                        storeInfo.SbvpName = model.SbvpName;
                        storeInfo.SbvpPhone = model.SbvpPhone;
                        storeInfo.SbvpType = model.SbvpType;
                        storeInfo.SbvpProvince = model.SbvpProvince;
                        storeInfo.SbvpDistrict = model.SbvpDistrict;
                        storeInfo.SbvpWard = model.SbvpWard;
                        storeInfo.SbvpStreetName = model.SbvpStreetName;
                        storeInfo.SbvpHouseNumber = model.SbvpHouseNumber;
                        storeInfo.DigixCode = "";
                        storeInfo.DigixName = "";
                        storeInfo.DigixPhone = "";
                        storeInfo.DigixType = "";
                        storeInfo.DigixProvince = "";
                        storeInfo.DigixDistrict = "";
                        storeInfo.DigixWard = "";
                        storeInfo.DigixStreetName = "";
                        storeInfo.DigixHouseNumber = "";
                        storeInfo.StoreIsChanged = model.StoreIsChanged;
                    }

                    //var model = (from tr in _db.tracks
                    //             join ts in _db.track_session on tr.Id equals ts.TrackId
                    //             // sbvp
                    //             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                    //             join type in _db.master_store_type.DefaultIfEmpty() on st.StoreType equals type.Id
                    //             join s_p in _db.provinces.DefaultIfEmpty() on st.ProvinceId equals s_p.Id into rs_p
                    //             from s_p in rs_p.DefaultIfEmpty()
                    //             join s_d in _db.districts.DefaultIfEmpty() on st.DistrictId equals s_d.Id into rs_d
                    //             from s_d in rs_p.DefaultIfEmpty()
                    //             join s_w in _db.wards.DefaultIfEmpty() on st.WardId equals s_w.Id into rs_w
                    //             from s_w in rs_w.DefaultIfEmpty()
                    //                 // digix
                    //             join d_p in _db.provinces.DefaultIfEmpty() on tr.ProvinceId equals d_p.Id into rs_p1
                    //             from d_p in rs_p1.DefaultIfEmpty()
                    //             join d_d in _db.districts.DefaultIfEmpty() on tr.DistrictId equals d_d.Id into rs_d1
                    //             from d_d in rs_d1.DefaultIfEmpty()
                    //             join d_w in _db.wards.DefaultIfEmpty() on tr.WardId equals d_w.Id into rs_w1
                    //             from d_w in rs_w1.DefaultIfEmpty()

                    //             where ts.Id == id
                    //             select new StoreInfoViewModel
                    //             {
                    //                 Id = tr.Id,
                    //                 SbvpName = st.Name,
                    //                 SbvpType = type.Name,
                    //                 SbvpProvince = s_p.Name,
                    //                 SbvpDistrict = s_d.Type + " " + s_d.Name,
                    //                 SbvpWard = s_w.Name,
                    //                 SbvpStreetName = st.StreetNames,
                    //                 SbvpHouseNumber = st.HouseNumber,
                    //                 DigixName = tr.MaterStoreName,
                    //                 DigixType = type.Name,
                    //                 DigixProvince = d_p.Name,
                    //                 DigixDistrict = d_d.Type + " " + d_d.Name,
                    //                 DigixWard = d_w.Name,
                    //                 DigixStreetName = tr.StreetNames,
                    //                 DigixHouseNumber = tr.HouseNumber
                    //             }).FirstOrDefault();
                    return storeInfo;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }

        public StoreInfoViewModel GetStoreInfoByTrackId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    StoreInfoViewModel storeInfo = new StoreInfoViewModel();
                    var model = _db.Database.SqlQuery<StoreInfoViewModel>(string.Format(@"SELECT 
                                                                                        sbstore.Id as Id,
                                                                                        sbstore.Code as SbvpCode,
                                                                                        sbstore.Name as SbvpName,
                                                                                        sbstore.PhoneNumber as SbvpPhone,
                                                                                        sbsttype.Name as SbvpType,
                                                                                        sbpr.Name as SbvpProvince,
                                                                                        sbdis.Name as SbvpDistrict,
                                                                                        sbwa.Name as SbvpWard,
                                                                                        sbstore.StreetNames as SbvpStreetName,
                                                                                        sbstore.HouseNumber as SbvpHouseNumber,
                                                                                        sbstore.Code as DigixCode,
                                                                                        tr.MaterStoreName as DigixName,
                                                                                        tr.PhoneNumber as DigixPhone,
                                                                                        dxsttype.Name as DigixType,
                                                                                        dxpr.Name as DigixProvince,
                                                                                        dxdis.Name as DigixDistrict,
                                                                                        dxwa.Name as DigixWard,
                                                                                        tr.StreetNames as DigixStreetName,
                                                                                        tr.HouseNumber as DigixHouseNumber,
                                                                                        tr.StoreIsChanged
	                                                                                        from track tr
	                                                                                        LEFT JOIN master_store sbstore on tr.MasterStoreId = sbstore.Id
	                                                                                        LEFT JOIN master_store_type sbsttype on sbstore.StoreType = sbsttype.Id
	                                                                                        LEFT JOIN province sbpr on sbstore.ProvinceId = sbpr.Id
	                                                                                        LEFT JOIN district sbdis on sbstore.DistrictId = sbdis.Id
	                                                                                        LEFT JOIN ward sbwa on sbstore.WardId = sbwa.Id
	                                                                                        LEFT JOIN master_store_type dxsttype on tr.StoreType = dxsttype.Id
	                                                                                        LEFT JOIN province dxpr on tr.ProvinceId = dxpr.Id
	                                                                                        LEFT JOIN district dxdis on tr.DistrictId = dxdis.Id
	                                                                                        LEFT JOIN ward dxwa on tr.WardId = dxwa.Id
	                                                                                        where tr.Id = '{0}'", id)).FirstOrDefault();


                    if (model.StoreIsChanged ?? true)
                    {
                        storeInfo.Id = model.Id;
                        storeInfo.SbvpCode = model.SbvpCode;
                        storeInfo.SbvpName = model.SbvpName;
                        storeInfo.SbvpPhone = model.SbvpPhone;
                        storeInfo.SbvpType = model.SbvpType;
                        storeInfo.SbvpProvince = model.SbvpProvince;
                        storeInfo.SbvpDistrict = model.SbvpDistrict;
                        storeInfo.SbvpWard = model.SbvpWard;
                        storeInfo.SbvpStreetName = model.SbvpStreetName;
                        storeInfo.SbvpHouseNumber = model.SbvpHouseNumber;

                        storeInfo.DigixCode = model.DigixCode;
                        storeInfo.DigixName = model.DigixName;
                        storeInfo.DigixPhone = model.DigixPhone;
                        storeInfo.DigixType = model.DigixType;
                        storeInfo.DigixProvince = model.DigixProvince;
                        storeInfo.DigixDistrict = model.DigixDistrict;
                        storeInfo.DigixWard = model.DigixWard;
                        storeInfo.DigixStreetName = model.DigixStreetName;
                        storeInfo.DigixHouseNumber = model.DigixHouseNumber;
                        storeInfo.StoreIsChanged = model.StoreIsChanged;
                    }
                    else
                    {
                        storeInfo.Id = model.Id;
                        storeInfo.SbvpCode = model.SbvpCode;
                        storeInfo.SbvpName = model.SbvpName;
                        storeInfo.SbvpPhone = model.SbvpPhone;
                        storeInfo.SbvpType = model.SbvpType;
                        storeInfo.SbvpProvince = model.SbvpProvince;
                        storeInfo.SbvpDistrict = model.SbvpDistrict;
                        storeInfo.SbvpWard = model.SbvpWard;
                        storeInfo.SbvpStreetName = model.SbvpStreetName;
                        storeInfo.SbvpHouseNumber = model.SbvpHouseNumber;
                        storeInfo.DigixCode = "";
                        storeInfo.DigixName = "";
                        storeInfo.DigixPhone = "";
                        storeInfo.DigixType = "";
                        storeInfo.DigixProvince = "";
                        storeInfo.DigixDistrict = "";
                        storeInfo.DigixWard = "";
                        storeInfo.DigixStreetName = "";
                        storeInfo.DigixHouseNumber = "";
                        storeInfo.StoreIsChanged = model.StoreIsChanged;
                    }
                    //var model = (from tr in _db.tracks
                    //                 // sbvp
                    //             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                    //             join type in _db.master_store_type.DefaultIfEmpty() on st.StoreType equals type.Id
                    //             join s_p in _db.provinces.DefaultIfEmpty() on st.ProvinceId equals s_p.Id
                    //             join s_d in _db.districts.DefaultIfEmpty() on st.DistrictId equals s_d.Id
                    //             join s_w in _db.wards.DefaultIfEmpty() on st.WardId equals s_w.Id into rs1
                    //             from s_w in rs1.DefaultIfEmpty()
                    //                 // digix
                    //             join d_p in _db.provinces.DefaultIfEmpty() on tr.ProvinceId equals d_p.Id
                    //             join d_d in _db.districts.DefaultIfEmpty() on tr.DistrictId equals d_d.Id
                    //             join d_w in _db.wards.DefaultIfEmpty() on tr.WardId equals d_w.Id into rs2
                    //             from d_w in rs2.DefaultIfEmpty()
                    //             where tr.Id == id
                    //             select new StoreInfoViewModel
                    //             {
                    //                 Id = tr.Id,
                    //                 SbvpName = st.Name,
                    //                 SbvpType = type.Name,
                    //                 SbvpProvince = s_p.Name,
                    //                 SbvpDistrict = s_d.Type + " " + s_d.Name,
                    //                 SbvpWard = s_w.Name,
                    //                 SbvpStreetName = st.StreetNames,
                    //                 SbvpHouseNumber = st.HouseNumber,
                    //                 DigixName = tr.MaterStoreName,
                    //                 DigixType = type.Name,
                    //                 DigixProvince = d_p.Name,
                    //                 DigixDistrict = d_d.Type + " " + d_d.Name,
                    //                 DigixWard = d_w.Name,
                    //                 DigixStreetName = tr.StreetNames,
                    //                 DigixHouseNumber = tr.HouseNumber
                    //             }).FirstOrDefault();

                    return storeInfo;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }

        public List<TrackSessionViewModel> GetTrackSessionListByTrackId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr in _db.tracks
                             join tr_se in _db.track_session on tr.Id equals tr_se.TrackId
                             where tr.Id == id
                             select new TrackSessionViewModel
                             {
                                 Id = tr_se.Id,
                                 CreateDate = tr_se.Date
                             }).ToList();

                return model;
            }
        }

        public List<TrackPosmStatisticViewModel> GetPOSMStatisticByTrackID(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var Tr_Session_Details = (from tr in _db.tracks
                                          join tr_se in _db.track_session.DefaultIfEmpty() on tr.Id equals tr_se.TrackId
                                          join td in _db.track_detail.DefaultIfEmpty() on tr_se.Id equals td.TrackSessionId
                                          join mt in _db.media_type.DefaultIfEmpty() on td.MediaTypeId equals mt.Code
                                          where tr.Id == id && MEDIA_TYPE.POSM.Contains(td.MediaTypeId)
                                          select new
                                          {
                                              TrackSessionID = td.TrackSessionId,
                                              MediaTypeID = td.MediaTypeId,
                                              MediaTypeName = mt.Name,
                                              createSessionDate = tr_se.CreatedDate,
                                              posmnumber = td.PosmNumber
                                          }
                                        ).ToList();

                var model = (from tr in Tr_Session_Details
                             group tr by new { tr.MediaTypeID, tr.MediaTypeName } into tmp
                             select new TrackPosmStatisticViewModel
                             {
                                 MediaTypeId = tmp.Key.MediaTypeID,
                                 MediaTypeName = tmp.Key.MediaTypeName,
                                 PosmNumber = tmp.OrderBy(x => x.TrackSessionID).FirstOrDefault().posmnumber
                             }).ToList();

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

        public track_detail GetSessionDetailById(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = _db.track_detail.Find(id);
                return model;
            }
        }

        public Byte[] GetExportTrackListImg()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                #region rem
                //var model = (from tr in _db.tracks
                //             join em in _db.employees on tr.EmployeeId equals em.Id
                //             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                //             select new TrackExcelViewModel
                //             {
                //                 Id = tr.Id,
                //                 EmployeeId = tr.EmployeeId,
                //                 EmployeeCode = em.Code,
                //                 EmployeeName = em.Name,
                //                 CreateDate = tr.Date,
                //                 StoreStatus = tr.StoreStatus,
                //                 Region = st.Region,
                //                 MasterStoreId = st.Code,
                //                 Note = tr.Note,

                //                 SbvpName = tr.MaterStoreName,
                //                 SbvpType = (_db.master_store_type.FirstOrDefault(x => x.Id == st.StoreType) != null)? _db.master_store_type.FirstOrDefault(x => x.Id == st.StoreType).Name : "",
                //                 SbvpProvince = _db.provinces.FirstOrDefault(x=> st.ProvinceId == x.Id) != null? _db.provinces.FirstOrDefault(x => st.ProvinceId == x.Id).Name : "",
                //                 SbvpDistrict = _db.districts.FirstOrDefault(x => st.DistrictId == x.Id) != null ? _db.districts.FirstOrDefault(x => st.DistrictId == x.Id).Type + " " +_db.districts.FirstOrDefault(x => st.DistrictId == x.Id).Name : "",
                //                 SbvpWard = _db.wards.FirstOrDefault(x => st.WardId == x.Id) != null ? _db.wards.FirstOrDefault(x => st.WardId == x.Id).Name : "",
                //                 SbvpStreetName = st.StreetNames,
                //                 SbvpHouseNumber = st.HouseNumber,

                //                 DigixName = tr.MaterStoreName,
                //                 DigixType = (_db.master_store_type.FirstOrDefault(x => x.Id == st.StoreType) != null) ? _db.master_store_type.FirstOrDefault(x => x.Id == st.StoreType).Name : "",
                //                 DigixProvince = _db.provinces.FirstOrDefault(x => tr.ProvinceId == x.Id) != null ? _db.provinces.FirstOrDefault(x => tr.ProvinceId == x.Id).Name : "",
                //                 DigixDistrict = _db.districts.FirstOrDefault(x => tr.DistrictId == x.Id) != null ? _db.districts.FirstOrDefault(x => tr.DistrictId == x.Id).Type + " " + _db.districts.FirstOrDefault(x => tr.DistrictId == x.Id).Name : "",
                //                 DigixWard = _db.wards.FirstOrDefault(x => tr.WardId == x.Id) != null ? _db.wards.FirstOrDefault(x => tr.WardId == x.Id).Name : "",
                //                 DigixStreetName = tr.StreetNames,
                //                 DigixHouseNumber = tr.HouseNumber,
                //                 DigixStoreIsChange = tr.StoreIsChanged != null ? (bool)tr.StoreIsChanged : false,

                //                 SessionCount = _db.track_session.Where(x=>x.TrackId == tr.Id).Count(),
                //                 ImageCount = (from tr_se in _db.track_session join td in _db.track_detail on tr_se.Id equals td.TrackSessionId where tr_se.TrackId == tr.Id select new { td.Id }).Count(),
                //                 checkInLat = tr.Lat,
                //                 checkInLng = tr.Lng,
                //                 checkOutLat = tr.Lat,
                //                 checkOutLng = tr.Lng

                //             }).OrderByDescending(x => x.CreateDate).ToList();
                #endregion
                #region model
                var model = _db.Database.SqlQuery<TrackExcelViewModel>(string.Format(@"SELECT tr.Id AS Id,
                                                                                              em.Id AS EmployeeId,
                                                                                              em.CODE AS EmployeeCode,
                                                                                              em.NAME AS EmployeeName,
                                                                                              sbstore.Id AS MasterStoreId,
                                                                                              sbstore.CODE AS MasterStoreCode,
                                                                                              sbstore.Region AS Region,
                                                                                              tr.Date AS Date,
                                                                                              tr.CreateDate AS CreateDate,
                                                                                              tr.Note AS Note,
                                                                                              tr.StoreStatus AS StoreStatus,
                                                                                              sbstore.NAME AS SbvpName,
                                                                                              sbsttype.NAME AS SbvpType,
                                                                                              sbpr.NAME AS SbvpProvince,
                                                                                              sbdis.NAME AS SbvpDistrict,
                                                                                              sbwa.NAME AS SbvpWard,
                                                                                              sbstore.HouseNumber AS SbvpHouseNumber,
                                                                                              sbstore.StreetNames AS SbvpStreetName,
                                                                                              tr.MaterStoreName AS DigixName,
                                                                                              dxsttype.NAME AS DigixType,
                                                                                              dxpr.NAME AS DigixProvince,
                                                                                              dxdis.NAME AS DigixDistrict,
                                                                                              dxwa.NAME AS DigixWard,
                                                                                              tr.HouseNumber AS DigixHouseNumber,
                                                                                              tr.StreetNames AS DigixStreetName,
                                                                                              tr.StoreIsChanged AS DigixStoreIsChange,
                                                                                              (
                                                                                                  SELECT COUNT(*) FROM track_session WHERE TrackId = tr.Id
                                                                                              ) AS SessionCount,
                                                                                              (
                                                                                                  SELECT COUNT(*)
                                                                                                  FROM track_detail tr_de
                                                                                                      LEFT JOIN track_session tr_se
                                                                                                          ON tr_de.TrackSessionId = tr_se.Id
                                                                                                      LEFT JOIN track tr_
                                                                                                          ON tr_se.TrackId = tr_.Id
                                                                                                  WHERE tr_.Id = tr.Id
                                                                                              ) AS ImageCount,
                                                                                              tr.Lat AS checkInLat,
                                                                                              tr.Lng AS checkInLng,
                                                                                              tr.Lat AS checkOutLat,
                                                                                              tr.Lng AS checkOutLng
                                                                                       FROM track tr
                                                                                           LEFT JOIN employee em
                                                                                               ON tr.EmployeeId = em.Id
                                                                                           LEFT JOIN master_store sbstore
                                                                                               ON tr.MasterStoreId = sbstore.Id
                                                                                           LEFT JOIN master_store_type sbsttype
                                                                                               ON sbstore.StoreType = sbsttype.Id
                                                                                           LEFT JOIN province sbpr
                                                                                               ON sbstore.ProvinceId = sbpr.Id
                                                                                           LEFT JOIN district sbdis
                                                                                               ON sbstore.DistrictId = sbdis.Id
                                                                                           LEFT JOIN ward sbwa
                                                                                               ON sbstore.WardId = sbwa.Id
                                                                                           LEFT JOIN master_store_type dxsttype
                                                                                               ON tr.StoreType = dxsttype.Id
                                                                                           LEFT JOIN province dxpr
                                                                                               ON tr.ProvinceId = dxpr.Id
                                                                                           LEFT JOIN district dxdis
                                                                                               ON tr.DistrictId = dxdis.Id
                                                                                           LEFT JOIN ward dxwa
                                                                                               ON tr.WardId = dxwa.Id
                                                                                       		ORDER BY tr.Date DESC;")).ToList();
                #endregion

                /////////////////////////////////////////////////
                // Write data to excel

                using (ExcelPackage p = new ExcelPackage())
                {
                    //Here setting some document properties
                    p.Workbook.Properties.Author = "Công ty";
                    p.Workbook.Properties.Title = "Báo cáo";

                    //Create a sheet
                    p.Workbook.Worksheets.Add("Báo cáo");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Báo cáo"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Times New Roman"; //Default Font name for whole sheet

                    // Create header column
                    string[] arrColumnHeader = {
                                                "Ngày thực hiện",
                                                "Khu vực",
                                                "User NV",
                                                "Nhân viên",
                                                "Mã CH",
                                                "Tên CH",
                                                "Loại hình cửa hàng",
                                                "Tỉnh",
                                                "Quận/Huyện",
                                                "Phường/Xã",
                                                "Đường",
                                                "Số nhà",
                                                "Địa chỉ",
                                                "Tên CH",
                                                "Loại hình cửa hàng",
                                                "Tỉnh",
                                                "Quận/Huyện",
                                                "Phường/Xã",
                                                "Đường",
                                                "Số nhà",
                                                "Địa chỉ",
                                                "Tranh Pepsi & 7Up",
                                                "Sticker 7Up",
                                                "Sticker Pepsi",
                                                "Banner Pepsi",
                                                "Banner 7Up Tết",
                                                "Banner Mirinda",
                                                "Banner Twister",
                                                "Banner Revive",
                                                "Banner Oolong",
                                                "Số lần viếng thăm",
                                                "Số lượng ảnh",
                                                "Check in",
                                                "Check out",
                                                "Thời gian làm việc tại CH",
                                                "Tọa độ check in",
                                                "Tọa độ check out",
                                                "Ghi chú NV"
                    };
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 2;

                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;

                        colIndex++;
                    }

                    ws.Cells[rowIndex, 6, rowIndex + 1, 6 + 7].Merge = false;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Merge = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[rowIndex, 6 + 8, rowIndex + 1, 6 + 8 + 7].Merge = false;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Merge = true;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex + 1, 6 + 8 + 7 + 9].Merge = false;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Merge = true;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    colIndex = 1;
                    //Creating Headings
                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.Font.Bold = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        var cell = ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex];

                        //Setting the background color of header cells to Gray
                        var fill3 = cell.Style.Fill;
                        fill3.PatternType = ExcelFillStyle.Solid;
                        fill3.BackgroundColor.SetColor(1, 0, 176, 240);

                        //Setting Top/left,right/bottom borders.
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        //Setting Value in cell
                        cell.Value = item;

                        colIndex++;
                    }

                    ws.Cells[rowIndex, 6].Value = "Thông tin cửa hàng SPVB cung cấp";
                    //Setting the background color of header cells to Gray
                    var fill = ws.Cells[rowIndex, 6].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 244, 176, 132);

                    ws.Cells[rowIndex, 6 + 8].Value = "Thông tin cửa hàng Digix cập nhật";
                    //Setting the background color of header cells to Gray
                    fill = ws.Cells[rowIndex, 6 + 8].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 244, 176, 132);

                    ws.Cells[rowIndex, 6 + 8 + 8].Value = "Loại POSM";
                    //Setting the background color of header cells to Gray
                    fill = ws.Cells[rowIndex, 6 + 8 + 8].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 255, 217, 102);

                    rowIndex++;
                    // Adding Data into rows
                    foreach (var item in model)
                    {
                        colIndex = 1;
                        rowIndex++;

                        var firstTR_SE = (from tr in _db.tracks join ts in _db.track_session on tr.Id equals ts.TrackId where tr.Id == item.Id orderby ts.CreatedDate select ts).OrderBy(x => x.CreatedDate).FirstOrDefault();

                        var details = new List<TrackDetailViewModel>();
                        if (firstTR_SE != null)
                        {
                            details = (from rs in (from tr in _db.tracks
                                                   join ts in _db.track_session on tr.Id equals ts.TrackId
                                                   join td in _db.track_detail on ts.Id equals td.TrackSessionId
                                                   join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                                   where ts.Id == firstTR_SE.Id
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
                                                       SessionId = ts.Id,
                                                       SessionCreateDate = ts.CreatedDate,
                                                       MediaTypeSub = td.MediaTypeSub
                                                   })
                                       group rs by new
                                       {
                                           rs.MediaTypeId,
                                           rs.MediaTypeName,
                                           rs.MediaTypeOrder,
                                           rs.SessionId,
                                           rs.SessionCreateDate
                                       } into g
                                       select new TrackDetailViewModel
                                       {
                                           MediaTypeId = g.Key.MediaTypeId,
                                           MediaTypeName = g.Key.MediaTypeName,
                                           MediaTypeOrder = g.Key.MediaTypeOrder,
                                           SessionId = g.Key.SessionId,
                                           TrackDetailImages = g.Select(x => new TrackDetailImageViewModel
                                           {
                                               Id = x.Id,
                                               FileName = x.FileName,
                                               Url = x.Url,
                                               PosmNumber = x.PosmNumber,
                                               CreateDate = x.CreateDate,
                                               MediaTypeSub = x.MediaTypeSub
                                           })
                                       }).ToList();
                        }


                        var tmpTRANH_PEPSI_AND_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.TRANH_PEPSI_AND_7UP);
                        var tmpSTICKER_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_7UP);
                        var tmpSTICKER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_PEPSI);
                        var tmpBANNER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_PEPSI);
                        var tmpBANNER_7UP_TET = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_7UP_TET);
                        var tmpBANNER_MIRINDA = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_MIRINDA);
                        var tmpBANNER_TWISTER = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_TWISTER);
                        var tmpBANNER_REVIVE = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_REVIVE);
                        var tmpBANNER_OOLONG = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_OOLONG);

                        //var sessions = _db.track_session.Where(x => x.TrackId == item.Id).ToList();

                        //Setting Value in cell
                        ws.Cells[rowIndex, colIndex].Value = item.CreateDate.ToString("dd-MM-yyyy");
                        //Setting Top/left,right/bottom borders.
                        var border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Region;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.EmployeeId;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.EmployeeName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.MasterStoreCode;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpType;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpProvince;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpDistrict;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpWard;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpStreetName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpHouseNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = $"{item.SbvpHouseNumber} {item.SbvpStreetName}, {item.SbvpWard}, {item.SbvpDistrict}, {item.SbvpProvince}";//Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixName : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixType : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixProvince : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixDistrict : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =

                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixWard : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixStreetName : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =

                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? item.DigixHouseNumber : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = (item.DigixStoreIsChange ?? true) ? $"{item.DigixHouseNumber} {item.DigixStreetName}, {item.DigixWard}, {item.DigixDistrict}, {item.DigixProvince}" : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpTRANH_PEPSI_AND_7UP == null ? 0 : tmpTRANH_PEPSI_AND_7UP.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpSTICKER_7UP == null ? 0 : tmpSTICKER_7UP.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpSTICKER_PEPSI == null ? 0 : tmpSTICKER_PEPSI.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_PEPSI == null ? 0 : tmpBANNER_PEPSI.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_7UP_TET == null ? 0 : tmpBANNER_7UP_TET.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_MIRINDA == null ? 0 : tmpBANNER_MIRINDA.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_TWISTER == null ? 0 : tmpBANNER_TWISTER.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_REVIVE == null ? 0 : tmpBANNER_REVIVE.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_OOLONG == null ? 0 : tmpBANNER_OOLONG.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SessionCount;//sessions.Count;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;


                        ws.Cells[rowIndex, colIndex].Value = item.ImageCount;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        var start = details.FirstOrDefault(x => x.MediaTypeId == "DEFAULT");

                        ws.Cells[rowIndex, colIndex].Value = start != null ? start.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().CreateDate.ToString("dd-MM-yyyy hh:mm:ss") : "";  // Giờ chụp hình tổng quan
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        var end = details.FirstOrDefault(x => x.MediaTypeId == "SELFIE" && x.TrackDetailImages.Any());

                        ws.Cells[rowIndex, colIndex].Value = end != null ? end.TrackDetailImages.FirstOrDefault().CreateDate.ToString("dd-MM-yyyy hh:mm:ss") : ""; // giờ chụp hình chấm công đầu ra
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = ((start != null && end != null) ? (start.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().CreateDate - end.TrackDetailImages.FirstOrDefault().CreateDate).ToString() : ""); // giờ chụp hình chấm công đầu ra
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = $"{item.checkInLat}; {item.checkInLng}";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = $"{item.checkOutLat}; {item.checkOutLng}";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Note;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                    }

                    //Generate A File with name
                    Byte[] bin = p.GetAsByteArray();

                    return bin;
                }
            }
        }


        public MessageReturnModel DeleteTrackDetail(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var detail = _db.track_detail.Find(id);
                if (detail != null)
                {
                    FileHelper.RemoveFileFromServer(detail.Url + detail.FileName); // remove old file

                    _db.track_detail.Remove(detail);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh!"
                    };
                }
            }
        }

        public MessageReturnModel DeleteTrackSession(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var session = _db.track_session.Find(id);
                if (session != null)
                {
                    var details = _db.track_detail.Where(x => x.TrackSessionId == id).ToList();
                    foreach (var item in details)
                    {
                        FileHelper.RemoveFileFromServer(item.Url + item.FileName); // remove old file
                        _db.track_detail.Remove(item);
                    }

                    _db.track_session.Remove(session);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy gói!"
                    };
                }
            }
        }

        public MessageReturnModel UpdateSessionDetail(track_detail model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var detail = _db.track_detail.Find(model.Id);
                if (detail != null)
                {
                    detail.FileName = model.FileName;
                    detail.Url = model.Url;
                    detail.MediaTypeId = model.MediaTypeId;
                    detail.EmployeeId = model.EmployeeId;
                    detail.IsActive = model.IsActive;
                    detail.TrackSessionId = model.TrackSessionId;
                    detail.PosmNumber = model.PosmNumber;

                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = detail.Id
                    };
                }

                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy!"
                };
            }
        }

        #region Processing Image Helper
        public void WriteTextToImage(string text, string serverFilePath)
        {
            string dir = Path.GetDirectoryName(serverFilePath);
            string ext = Path.GetExtension(serverFilePath);
            string newFileName = Path.GetFileNameWithoutExtension(serverFilePath) + "_rewrite";
            string newFilenamePath = Path.Combine(dir, newFileName + ext);

            var bitmap = Image.FromFile(serverFilePath); // set 
            //draw the image object using a Graphics object
            Graphics graphicsImage = Graphics.FromImage(bitmap);
            int fontsize = (bitmap.Width + bitmap.Height) / 90;

            StringFormat stringformat = new StringFormat();
            stringformat.Alignment = StringAlignment.Near;
            stringformat.LineAlignment = StringAlignment.Near;
            Color StringColor = Color.Red;
            graphicsImage.DrawString(text, new Font("Arial", fontsize,
            FontStyle.Bold), new SolidBrush(StringColor), new Point(0, 0),
            stringformat);

            bitmap.Save(newFilenamePath);
            bitmap.Dispose();
            graphicsImage.Dispose();     
        }


        public string WriteTextToImageCustom(string text, string sourceFilePath)
        {
            string dir = Path.GetDirectoryName(sourceFilePath);
            string ext = Path.GetExtension(sourceFilePath);

            //string dirtarget =  sourceFilePath


            string newFileName = Path.GetFileNameWithoutExtension(sourceFilePath) + "_rewrite";
            string newFilenamePath = Path.Combine(dir, newFileName + ext);

            var bitmap = Image.FromFile(sourceFilePath); // set 
            //draw the image object using a Graphics object
            Graphics graphicsImage = Graphics.FromImage(bitmap);
            int fontsize = (bitmap.Width + bitmap.Height) / 90;

            StringFormat stringformat = new StringFormat();
            stringformat.Alignment = StringAlignment.Near;
            stringformat.LineAlignment = StringAlignment.Near;
            Color StringColor = Color.Red;
            graphicsImage.DrawString(text, new Font("Arial", fontsize,
            FontStyle.Bold), new SolidBrush(StringColor), new Point(0, 0),
            stringformat);

            bitmap.Save(newFilenamePath);
            bitmap.Dispose();
            graphicsImage.Dispose();


            return newFilenamePath;

        }


        #endregion
    }
}
