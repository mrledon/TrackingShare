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
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Core.Utils;
using System.Web.Configuration;
using System.Web;

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
                                         where !tr_se.IsEndSession.HasValue && tr_se.IsEndSession.Value
                                         select new
                                         {
                                             Id = tr.Id,
                                             Date = tr.Date,
                                             EmployeeId = tr.EmployeeId,
                                             EmployeeName = em.Name,
                                             Manager = em.Owner,
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
                                 rs.Date,
                                 rs.EmployeeId,
                                 rs.EmployeeName,
                                 rs.Manager,
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
                                 Date = "",
                                 EmployeeId = g.Key.EmployeeId,
                                 EmployeeName = g.Key.EmployeeName,
                                 Manager = g.Key.Manager,
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

        /// <summary>
        /// Get list data using jquery datatable
        /// </summary>
        /// <param name="request">Jquery datatable request</param>
        /// <returns><string, object></returns>
        public Dictionary<string, object> List(CustomDataTableRequestHelper request)
        {
            string[] tmp = request.FromDate.Split('-');
            DateTime fromDate = new DateTime(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]), 00, 00, 00); //From date
            tmp = request.ToDate.Split('-');
            DateTime toDate = new DateTime(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]), 23, 23, 59); //To date

            Dictionary<string, object> _return = new Dictionary<string, object>();
            try
            {
                //Declare response data to json object
                DataTableResponse<ImageManagementViewModel> _itemResponse = new DataTableResponse<ImageManagementViewModel>();
                //List of data
                List<ImageManagementViewModel> _list = new List<ImageManagementViewModel>();

                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var _lData = (from tr in _db.tracks
                                  join store in _db.master_store on tr.MasterStoreId equals store.Id
                                  join em in _db.employees on tr.EmployeeId equals em.Id
                                  where tr.Date >= fromDate && tr.Date <= toDate
                                  orderby tr.CreateDate descending
                                  select new
                                  {
                                      Id = tr.Id,
                                      Date = tr.Date,
                                      EmployeeId = tr.EmployeeId,
                                      EmployeeName = em.Name,
                                      Manager = em.Owner,
                                      MasterStoreId = tr.MasterStoreId,
                                      MasterStoreCode = store.Code,
                                      MasterStoreName = tr.MaterStoreName,
                                      StoreStatus = tr.StoreStatus,
                                      Region = store.Region,
                                      QCStatus = tr.QCStatus ?? 0
                                  }).ToList();

                    //Area
                    if (request.Region.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Region.Contains(m.Region)
                                  select m).ToList();
                    }

                    //Store
                    if (request.Store.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Store.Contains(m.MasterStoreCode.ToString())
                                  select m).ToList();
                    }

                    //Employee
                    if (request.Employee.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Employee.Contains(m.EmployeeId.ToString())
                                  select m).ToList();
                    }

                    _itemResponse.draw = request.draw;
                    _itemResponse.recordsTotal = _lData.Count;
                    //Search
                    if (request.search != null && !string.IsNullOrWhiteSpace(request.search.Value))
                    {
                        string searchValue = request.search.Value.ToLower();
                        _lData = _lData.Where(m => m.Date.ToString().ToLower().Contains(searchValue) ||
                                                   m.EmployeeId.ToLower().Contains(searchValue) ||
                                                   m.EmployeeName.ToLower().Contains(searchValue) ||
                                                   m.Manager.ToLower().Contains(searchValue) ||
                                                   m.MasterStoreCode.ToLower().Contains(searchValue) ||
                                                   m.MasterStoreName.ToLower().Contains(searchValue) ||
                                                   m.Region.ToLower().Contains(searchValue)).ToList();
                    }
                    var lstRole = (List<String>)HttpContext.Current.Session["Roles"];
                    //Add to list
                    foreach (var item in _lData)
                    {
                        var _trackSession = _db.track_session.Where(m => (m.TrackId == item.Id) && (!m.IsEndSession.HasValue || m.IsEndSession.Value)).Select(m => new { m.Id, m.Date, m.Status }).ToList();

                        _list.Add(new ImageManagementViewModel()
                        {
                            Id = item.Id,
                            Date = item.Date.ToString("dd-MM-yyyy"),
                            EmployeeId = item.EmployeeId,
                            EmployeeName = item.EmployeeName,
                            Manager = item.Manager,
                            MasterStoreId = item.MasterStoreId,
                            MasterStoreCode = item.MasterStoreCode,
                            MasterStoreName = item.MasterStoreName,
                            StoreStatus = item.StoreStatus ?? false,
                            Region = item.Region,
                            QCStatus = item.QCStatus,
                            QCStatusString = QCStatus.QCStatusData().FirstOrDefault(m => m.Id == item.QCStatus).Name,
                            lstRole = lstRole,
                            TrackSessions = _trackSession.Select(m => new TrackSessionViewModel()
                            {
                                Id = m.Id,
                                CreateDate = m.Date,
                                CreateDateString = m.Date.ToString("dd-MM-yyyy"),
                                Status = m.Status ?? false
                            }).ToList()
                        });
                    }
                    _itemResponse.recordsFiltered = _list.Count;
                    _itemResponse.data = _list.Skip(request.start).Take(request.length).ToList();
                    _return.Add(DatatableCommonSetting.Response.DATA, _itemResponse);
                }
                _return.Add(DatatableCommonSetting.Response.STATUS, ResponseStatusCodeHelper.OK);
            }
            catch (Exception ex)
            {
                //throw new ServiceException(FILE_NAME, "List", userID, ex);
            }
            return _return;
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
                                                                                        tr.Date as Date,
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
                                                                                        tr.Id as DigixId,
                                                                                        tr.MaterStoreName as DigixName,
                                                                                        tr.PhoneNumber as DigixPhone,
                                                                                        dxsttype.Name as DigixType,
                                                                                        dxpr.Name as DigixProvince,
                                                                                        dxdis.Name as DigixDistrict,
                                                                                        dxwa.Name as DigixWard,
                                                                                        tr.StreetNames as DigixStreetName,
                                                                                        tr.HouseNumber as DigixHouseNumber,
                                                                                        tr.StoreIsChanged,
                                                                                        tr.Note as Note
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
                        storeInfo.Note = model.Note;
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
                        storeInfo.DigixId = model.DigixId;
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
                        storeInfo.Note = model.Note;
                        storeInfo.SbvpCode = model.SbvpCode;
                        storeInfo.SbvpName = model.SbvpName;
                        storeInfo.SbvpPhone = model.SbvpPhone;
                        storeInfo.SbvpType = model.SbvpType;
                        storeInfo.SbvpProvince = model.SbvpProvince;
                        storeInfo.SbvpDistrict = model.SbvpDistrict;
                        storeInfo.SbvpWard = model.SbvpWard;
                        storeInfo.SbvpStreetName = model.SbvpStreetName;
                        storeInfo.SbvpHouseNumber = model.SbvpHouseNumber;
                        storeInfo.DigixId = model.DigixId;
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
                //var model = (from tr in _db.tracks
                //             join tr_se in _db.track_session on tr.Id equals tr_se.TrackId
                //             where tr.Id == id 
                //             select new TrackSessionViewModel
                //             {
                //                 Id = tr_se.Id,
                //                 CreateDate = tr_se.Date,
                //                 Status = tr_se.Status
                //             }).ToList();

                var model = (from tr_se in _db.track_session
                             where tr_se.TrackId == id && (!tr_se.IsEndSession.HasValue || tr_se.IsEndSession.Value)
                             select new TrackSessionViewModel
                             {
                                 Id = tr_se.Id,
                                 CreateDate = tr_se.Date,
                                 Status = tr_se.Status
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
                                              TrackDetailId = td.Id,
                                              TrackSessionID = td.TrackSessionId,
                                              MediaTypeID = td.MediaTypeId,
                                              MediaTypeName = mt.Name,
                                              createSessionDate = tr_se.CreatedDate,
                                              posmnumber = td.PosmNumber
                                          }
                                        ).ToList();

                var model = (from tr in Tr_Session_Details
                             group tr by new { tr.MediaTypeID, tr.MediaTypeName, tr.TrackSessionID } into tmp
                             select new TrackPosmStatisticViewModel
                             {
                                 TrackSessionId = tmp.Key.TrackSessionID,
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
                var model = (from rs in (from td in _db.track_detail
                                         join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                         where td.TrackSessionId == id
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

        /// <summary>
        /// Export to excel file
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="region"></param>
        /// <param name="store"></param>
        /// <param name="employee"></param>
        /// <param name="templatePath"></param>
        /// <param name="tempFoldePath"></param>
        /// <returns></returns>
        public string GetExportTrackListImg(string fromDate, string toDate, List<string> region, List<string> store, List<string> employee, string templatePath, string tempFoldePath)
        {
            string _returnFileName = Guid.NewGuid().ToString() + ".xlsx";

            string _tempFile = Path.Combine(tempFoldePath, Guid.NewGuid().ToString() + ".xlsx");

            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                #region " Filert "

                StringBuilder whereCondition = new StringBuilder();

                string[] tmpFrom = fromDate.Split('-');
                string[] tmpTo = toDate.Split('-');

                whereCondition.AppendLine(string.Format("WHERE tr.Date BETWEEN '{0}-{1}-{2} 00:00:00' AND '{3}-{4}-{5} 23:23:59'", tmpFrom[0], tmpFrom[1], tmpFrom[2], tmpTo[0], tmpTo[1], tmpTo[2]));
                
                //Area
                if (region.Count() > 0)
                {
                    whereCondition.AppendLine("AND sbstore.Region IN ");
                    string tmp = "";
                    foreach (var item in region)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");

                }

                //Store
                if (store.Count() > 0)
                {
                    whereCondition.AppendLine("AND sbstore.CODE IN ");
                    string tmp = "";
                    foreach (var item in store)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");
                }

                //Employee
                if (employee.Count() > 0)
                {
                    whereCondition.AppendLine("AND em.Id IN ");
                    string tmp = "";
                    foreach (var item in employee)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");
                }

                #endregion


                #region POSM NUMBER
                string selectPosm = @", IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'TRANH_PEPSI_AND_7UP'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS TRANH_PEPSI_AND_7UP,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'STICKER_7UP'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS STICKER_7UP,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'STICKER_PEPSI'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS STICKER_PEPSI,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'BANNER_PEPSI'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS BANNER_PEPSI,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'BANNER_7UP_TET'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS BANNER_7UP_TET,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'BANNER_MIRINDA'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS BANNER_MIRINDA,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'BANNER_TWISTER'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS BANNER_TWISTER,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber
                                                FROM
                                                    track_detail posm_tr_de
                                                    LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                    LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id
                                                WHERE
                                                    posm_tr.Id = tr.Id
                                                    AND posm_tr_se.STATUS = 1
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                    AND posm_tr_de.MediaTypeId = 'BANNER_REVIVE'
                                                ORDER BY
                                                    posm_tr_se.CreatedDate ASC
                                                    LIMIT 1),0) AS BANNER_REVIVE,
                                                IFNULL((SELECT
                                                    posm_tr_de.PosmNumber 
                                                FROM
                                                	track_detail posm_tr_de
                                                	LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                                	LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id 
                                                WHERE
                                                	posm_tr.Id = tr.Id 
                                                	AND posm_tr_se.STATUS = 1 
                                                    AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                                	AND posm_tr_de.MediaTypeId = 'BANNER_OOLONG' 
                                                ORDER BY
                                                	posm_tr_se.CreatedDate ASC 
                                                	LIMIT 1),0) AS BANNER_OOLONG";
                #endregion POSM NUMBER


                #region Time attendance
                string attendanceTime = @", (SELECT
                                             	posm_tr_de.CreateDate 
                                             FROM
                                             	track_detail posm_tr_de
                                             	LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                             	LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id 
                                             WHERE
                                             	posm_tr.Id = tr.Id  
                                             	AND posm_tr_se.STATUS = 1
                                                AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                             	AND posm_tr_de.MediaTypeId = 'DEFAULT' 
                                             ORDER BY
                                             	posm_tr_se.CreatedDate ASC 
                                             	LIMIT 1) AS AttendanceStart,
                                             (SELECT
                                             	posm_tr_de.CreateDate 
                                             FROM
                                             	track_detail posm_tr_de
                                             	LEFT JOIN track_session AS posm_tr_se ON posm_tr_de.TrackSessionId = posm_tr_se.Id
                                             	LEFT JOIN track posm_tr ON posm_tr_se.TrackId = posm_tr.Id 
                                             WHERE
                                             	posm_tr.Id = tr.Id  
                                             	AND posm_tr_se.STATUS = 1
                                                AND (posm_tr_se.IsEndSession IS NULL OR posm_tr_se.IsEndSession = 1)
                                             	AND posm_tr_de.MediaTypeId = 'SELFIE' 
                                             ORDER BY
                                             	posm_tr_se.CreatedDate ASC 
                                             	LIMIT 1) AS AttendanceEnd";
                #endregion Time attendance

                #region model
                var model = _db.Database.SqlQuery<TrackExcelViewModel>(string.Format(@"SELECT tr.Id AS Id,
                                                                                              em.Id AS EmployeeId,
                                                                                              em.CODE AS EmployeeCode,
                                                                                              em.NAME AS EmployeeName,
                                                                                              em.Owner AS Owner,
                                                                                              sbstore.Id AS MasterStoreId,
                                                                                              sbstore.CODE AS MasterStoreCode,
                                                                                              sbstore.Region AS Region,
                                                                                              tr.Date AS Date,
                                                                                              tr.CreateDate AS CreateDate,
                                                                                              tr.Note AS Note,
                                                                                              sbstore.NAME AS SbvpName,
                                                                                              sbsttype.NAME AS SbvpType,
                                                                                              sbpr.NAME AS SbvpProvince,
                                                                                              sbdis.NAME AS SbvpDistrict,
                                                                                              sbwa.NAME AS SbvpWard,
                                                                                              sbstore.PhoneNumber AS SbvpPhoneNumber,
                                                                                              sbstore.HouseNumber AS SbvpHouseNumber,
                                                                                              sbstore.StreetNames AS SbvpStreetName,
                                                                                              tr.MaterStoreName AS DigixName,
                                                                                              dxsttype.NAME AS DigixType,
                                                                                              dxpr.NAME AS DigixProvince,
                                                                                              dxdis.NAME AS DigixDistrict,
                                                                                              dxwa.NAME AS DigixWard,
                                                                                              tr.PhoneNumber AS DigixPhoneNumber,
                                                                                              tr.HouseNumber AS DigixHouseNumber,
                                                                                              tr.StreetNames AS DigixStreetName,
                                                                                              tr.StoreIsChanged AS DigixStoreIsChange,
                                                                                              (
                                                                                                  SELECT COUNT(*) FROM track_session WHERE (TrackId = tr.Id) AND (IsEndSession IS NULL OR IsEndSession = 1) 
                                                                                              ) AS SessionCount,
                                                                                              (
                                                                                                  SELECT COUNT(*)
                                                                                                  FROM track_detail tr_de
                                                                                                      LEFT JOIN track_session tr_se
                                                                                                          ON tr_de.TrackSessionId = tr_se.Id
                                                                                                      LEFT JOIN track tr_
                                                                                                          ON tr_se.TrackId = tr_.Id
                                                                                                  WHERE (tr_.Id = tr.Id) AND (tr_se.IsEndSession IS NULL OR tr_se.IsEndSession = 1)
                                                                                              ) AS ImageCount,
                                                                                              tr.Lat AS checkInLat,
                                                                                              tr.Lng AS checkInLng,
                                                                                              tr.Lat AS checkOutLat,
                                                                                              tr.Lng AS checkOutLng, 
                                                                                              tr.StoreStatus AS StoreStatus
                                                                                              {0} {1}
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
                                                                                            {2}
                                                                                       		ORDER BY tr.Date DESC;", selectPosm, attendanceTime, whereCondition.ToString())).ToList();
                #endregion
                try
                {
                    //Copy template to temporary folder
                    File.Copy(templatePath, _tempFile, true);

                    //Read excel file
                    FileInfo file = new FileInfo(_tempFile);
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorkbook excelWorkBook = package.Workbook;
                        ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets.First();
                        if (model.Count() == 0)
                        {
                            excelWorksheet.DeleteRow(4, 1, true);
                        }
                        else
                        {
                            int rowIndex = 4;
                            string tmp = "";
                            foreach (var item in model)
                            {
                                if (rowIndex < (model.Count() + 4) - 1)
                                {
                                    excelWorksheet.Cells[rowIndex, 1, rowIndex, 42].Copy(excelWorksheet.Cells[rowIndex + 1, 1, rowIndex + 1, 42]);
                                }
                                //var firstTR_SE = (from tr in _db.tracks
                                //                  join ts in _db.track_session on tr.Id equals ts.TrackId
                                //                  where tr.Id == item.Id
                                //                  orderby ts.CreatedDate
                                //                  select ts).OrderBy(x => x.CreatedDate).FirstOrDefault();

                                //var details = new List<TrackDetailViewModel>();
                                //if (firstTR_SE != null)
                                //{
                                //    details = (from rs in (from tr in _db.tracks
                                //                           join ts in _db.track_session on tr.Id equals ts.TrackId
                                //                           join td in _db.track_detail on ts.Id equals td.TrackSessionId
                                //                           join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                //                           where ts.Id == firstTR_SE.Id
                                //                           select new
                                //                           {
                                //                               Id = td.Id,
                                //                               FileName = td.FileName,
                                //                               Url = td.Url,
                                //                               MediaTypeId = td.MediaTypeId,
                                //                               MediaTypeName = mt.Name,
                                //                               MediaTypeOrder = mt.OrderNumber,
                                //                               PosmNumber = td.PosmNumber,
                                //                               CreateDate = td.CreateDate,
                                //                               SessionId = ts.Id,
                                //                               SessionCreateDate = ts.CreatedDate,
                                //                               MediaTypeSub = td.MediaTypeSub
                                //                           })
                                //               group rs by new
                                //               {
                                //                   rs.MediaTypeId,
                                //                   rs.MediaTypeName,
                                //                   rs.MediaTypeOrder,
                                //                   rs.SessionId,
                                //                   rs.SessionCreateDate
                                //               } into g
                                //               select new TrackDetailViewModel
                                //               {
                                //                   MediaTypeId = g.Key.MediaTypeId,
                                //                   MediaTypeName = g.Key.MediaTypeName,
                                //                   MediaTypeOrder = g.Key.MediaTypeOrder,
                                //                   SessionId = g.Key.SessionId,
                                //                   TrackDetailImages = g.Select(x => new TrackDetailImageViewModel
                                //                   {
                                //                       Id = x.Id,
                                //                       FileName = x.FileName,
                                //                       Url = x.Url,
                                //                       PosmNumber = x.PosmNumber,
                                //                       CreateDate = x.CreateDate,
                                //                       MediaTypeSub = x.MediaTypeSub
                                //                   })
                                //               }).ToList();
                                //}


                                //var tmpTRANH_PEPSI_AND_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.TRANH_PEPSI_AND_7UP);
                                //var tmpSTICKER_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_7UP);
                                //var tmpSTICKER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_PEPSI);
                                //var tmpBANNER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_PEPSI);
                                //var tmpBANNER_7UP_TET = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_7UP_TET);
                                //var tmpBANNER_MIRINDA = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_MIRINDA);
                                //var tmpBANNER_TWISTER = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_TWISTER);
                                //var tmpBANNER_REVIVE = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_REVIVE);
                                //var tmpBANNER_OOLONG = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_OOLONG);

                                #region " [ Setting Value in cell ] "

                                excelWorksheet.Cells[rowIndex, 1].Value = item.CreateDate.ToString("dd-MM-yyyy");
                                excelWorksheet.Cells[rowIndex, 2].Value = item.Region;
                                excelWorksheet.Cells[rowIndex, 3].Value = item.EmployeeId;
                                excelWorksheet.Cells[rowIndex, 4].Value = item.EmployeeName;
                                excelWorksheet.Cells[rowIndex, 5].Value = item.Owner;
                                excelWorksheet.Cells[rowIndex, 6].Value = item.MasterStoreCode;
                                excelWorksheet.Cells[rowIndex, 7].Value = item.SbvpName;
                                excelWorksheet.Cells[rowIndex, 8].Value = item.SbvpPhoneNumber;
                                excelWorksheet.Cells[rowIndex, 9].Value = item.SbvpType;
                                excelWorksheet.Cells[rowIndex, 10].Value = item.SbvpProvince;
                                excelWorksheet.Cells[rowIndex, 11].Value = item.SbvpDistrict;
                                excelWorksheet.Cells[rowIndex, 12].Value = item.SbvpWard;
                                excelWorksheet.Cells[rowIndex, 13].Value = item.SbvpStreetName;
                                excelWorksheet.Cells[rowIndex, 14].Value = item.SbvpHouseNumber;

                                tmp = "";
                                tmp += item.SbvpHouseNumber + " " + item.SbvpStreetName;
                                if (item.SbvpWard != null)
                                {
                                    tmp += (tmp.Length > 0 ? ", " : "") + item.SbvpWard;
                                }
                                if (item.SbvpDistrict != null)
                                {
                                    tmp += (tmp.Length > 0 ? ", " : "") + item.SbvpDistrict;
                                }
                                if (item.SbvpProvince != null)
                                {
                                    tmp += (tmp.Length > 0 ? ", " : "") + item.SbvpProvince;
                                }
                                excelWorksheet.Cells[rowIndex, 15].Value = tmp;

                                excelWorksheet.Cells[rowIndex, 16].Value = (item.DigixStoreIsChange ?? true) ? item.DigixName : "";
                                excelWorksheet.Cells[rowIndex, 17].Value = (item.DigixStoreIsChange ?? true) ? item.DigixPhoneNumber : "";
                                excelWorksheet.Cells[rowIndex, 18].Value = (item.DigixStoreIsChange ?? true) ? item.DigixType : "";
                                excelWorksheet.Cells[rowIndex, 19].Value = (item.DigixStoreIsChange ?? true) ? item.DigixProvince : "";
                                excelWorksheet.Cells[rowIndex, 20].Value = (item.DigixStoreIsChange ?? true) ? item.DigixDistrict : "";
                                excelWorksheet.Cells[rowIndex, 21].Value = (item.DigixStoreIsChange ?? true) ? item.DigixWard : "";
                                excelWorksheet.Cells[rowIndex, 22].Value = (item.DigixStoreIsChange ?? true) ? item.DigixStreetName : "";
                                excelWorksheet.Cells[rowIndex, 23].Value = (item.DigixStoreIsChange ?? true) ? item.DigixHouseNumber : "";

                                tmp = "";
                                if (item.DigixStoreIsChange ?? true)
                                {
                                    tmp += item.DigixHouseNumber + " " + item.DigixStreetName;
                                    if (item.DigixWard != null)
                                    {
                                        tmp += (tmp.Length > 0 ? ", " : "") + item.DigixWard;
                                    }
                                    if (item.DigixDistrict != null)
                                    {
                                        tmp += (tmp.Length > 0 ? ", " : "") + item.DigixDistrict;
                                    }
                                    if (item.DigixProvince != null)
                                    {
                                        tmp += (tmp.Length > 0 ? ", " : "") + item.DigixProvince;
                                    }
                                }
                                excelWorksheet.Cells[rowIndex, 24].Value = tmp;

                                //excelWorksheet.Cells[rowIndex, 25].Value = tmpTRANH_PEPSI_AND_7UP == null ? 0 : tmpTRANH_PEPSI_AND_7UP.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 26].Value = tmpSTICKER_7UP == null ? 0 : tmpSTICKER_7UP.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 27].Value = tmpSTICKER_PEPSI == null ? 0 : tmpSTICKER_PEPSI.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 28].Value = tmpBANNER_PEPSI == null ? 0 : tmpBANNER_PEPSI.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 29].Value = tmpBANNER_7UP_TET == null ? 0 : tmpBANNER_7UP_TET.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 30].Value = tmpBANNER_MIRINDA == null ? 0 : tmpBANNER_MIRINDA.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 31].Value = tmpBANNER_TWISTER == null ? 0 : tmpBANNER_TWISTER.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 32].Value = tmpBANNER_REVIVE == null ? 0 : tmpBANNER_REVIVE.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 33].Value = tmpBANNER_OOLONG == null ? 0 : tmpBANNER_OOLONG.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().PosmNumber;
                                //excelWorksheet.Cells[rowIndex, 34].Value = item.SessionCount == null ? 0 : item.SessionCount;
                                //excelWorksheet.Cells[rowIndex, 35].Value = item.ImageCount;

                                //var start = details.FirstOrDefault(x => x.MediaTypeId == "DEFAULT");

                                //excelWorksheet.Cells[rowIndex, 36].Value = start != null ? start.TrackDetailImages.OrderBy(x => x.CreateDate).FirstOrDefault().CreateDate.ToString("dd-MM-yyyy hh:mm:ss") : "";  // Giờ chụp hình tổng quan

                                //var end = details.FirstOrDefault(x => x.MediaTypeId == "SELFIE" && x.TrackDetailImages.Any());
                                //excelWorksheet.Cells[rowIndex, 37].Value = end != null ? end.TrackDetailImages.FirstOrDefault().CreateDate.ToString("dd-MM-yyyy hh:mm:ss") : ""; // giờ chụp hình chấm công đầu ra




                                excelWorksheet.Cells[rowIndex, 25].Value = item.TRANH_PEPSI_AND_7UP;
                                excelWorksheet.Cells[rowIndex, 26].Value = item.STICKER_7UP;
                                excelWorksheet.Cells[rowIndex, 27].Value = item.STICKER_PEPSI;
                                excelWorksheet.Cells[rowIndex, 28].Value = item.BANNER_PEPSI;
                                excelWorksheet.Cells[rowIndex, 29].Value = item.BANNER_7UP_TET;
                                excelWorksheet.Cells[rowIndex, 30].Value = item.BANNER_MIRINDA;
                                excelWorksheet.Cells[rowIndex, 31].Value = item.BANNER_TWISTER;
                                excelWorksheet.Cells[rowIndex, 32].Value = item.BANNER_REVIVE;
                                excelWorksheet.Cells[rowIndex, 33].Value = item.BANNER_OOLONG;
                                excelWorksheet.Cells[rowIndex, 34].Value = item.SessionCount == null ? 0 : item.SessionCount;
                                excelWorksheet.Cells[rowIndex, 35].Value = item.ImageCount;

                                excelWorksheet.Cells[rowIndex, 36].Value = (item.AttendanceStart != null) ? item.AttendanceStart.Value.ToString("dd-MM-yyyy hh:mm:ss") : "";  // Giờ chụp hình tổng quan


                                excelWorksheet.Cells[rowIndex, 37].Value = (item.AttendanceEnd != null) ? item.AttendanceEnd.Value.ToString("dd-MM-yyyy hh:mm:ss") : ""; // giờ chụp hình chấm công đầu ra


                                excelWorksheet.Cells[rowIndex, 38].Value = (item.AttendanceEnd != null && item.AttendanceStart != null) ? (item.AttendanceEnd - item.AttendanceStart).ToString() : "";

                                excelWorksheet.Cells[rowIndex, 39].Value = $"{item.checkInLat}; {item.checkInLng}";
                                excelWorksheet.Cells[rowIndex, 40].Value = $"{item.checkOutLat}; {item.checkOutLng}";
                                excelWorksheet.Cells[rowIndex, 41].Value = item.Note;
                                excelWorksheet.Cells[rowIndex, 42].Value = (item.StoreStatus ?? true) ? "Thành công" : "Không thành công";

                                #endregion
                                rowIndex++;

                            }
                        }
                        package.SaveAs(new FileInfo(Path.Combine(tempFoldePath, _returnFileName)));
                        return _returnFileName;
                    }
                }
                catch (Exception ex)
                {
                    return "";
                }
                finally
                {
                    if (File.Exists(_tempFile))
                    {
                        File.Delete(_tempFile);
                    }
                    GC.Collect();

                }

            }
        }

        /// <summary>
        /// Export to excel file
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="region"></param>
        /// <param name="store"></param>
        /// <param name="employee"></param>
        /// <param name="userId"></param>
        /// <param name="tempFoldePath"></param>
        /// <returns></returns>
        public string ZipImage(string fromDate, string toDate, List<string> region, List<string> store, List<string> employee, string userId, string tempFoldePath)
        {
            string rootFolderImage = WebConfigurationManager.AppSettings["rootMedia"];

            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                #region " Filert "

                StringBuilder whereCondition = new StringBuilder();

                string[] tmpFrom = fromDate.Split('-');
                string[] tmpTo = toDate.Split('-');

                whereCondition.AppendLine(string.Format("WHERE tr.Date BETWEEN '{0}-{1}-{2} 00:00:00' AND '{3}-{4}-{5} 23:23:59'", tmpFrom[0], tmpFrom[1], tmpFrom[2], tmpTo[0], tmpTo[1], tmpTo[2]));

                //Area
                if (region.Count() > 0)
                {
                    whereCondition.AppendLine("AND sbstore.Region IN ");
                    string tmp = "";
                    foreach (var item in region)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");

                }

                //Store
                if (store.Count() > 0)
                {
                    whereCondition.AppendLine("AND sbstore.CODE IN ");
                    string tmp = "";
                    foreach (var item in store)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");
                }

                //Employee
                if (employee.Count() > 0)
                {
                    whereCondition.AppendLine("AND em.Id IN ");
                    string tmp = "";
                    foreach (var item in employee)
                    {
                        tmp += "'" + item + "',";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    whereCondition.Append(" (" + tmp + ") ");
                }

                #endregion
                
                #region model
                
                var model = _db.Database.SqlQuery<TrackExcelViewModel>(string.Format(@"SELECT tr.Id AS Id,
				                                                                              sbstore.CODE AS MasterStoreCode,
				                                                                              tr.Date AS Date,
                                                                                       FROM track tr
		                                                                               LEFT JOIN employee em ON tr.EmployeeId = em.Id
		                                                                               LEFT JOIN master_store sbstore ON tr.MasterStoreId = sbstore.Id
		                                                                               {0}
		                                                                               ORDER BY tr.Date DESC;", whereCondition.ToString())).ToList();
                #endregion
                try
                {
                    //Delete folder if exists
                    if (Directory.Exists(Path.Combine(tempFoldePath, userId)))
                    {
                        Directory.Delete(Path.Combine(tempFoldePath, userId));
                    }
                    //Create directory
                    Directory.CreateDirectory(Path.Combine(tempFoldePath, userId));
                    

                    foreach (var item in model)
                    {
                        var listImage = (from m in _db.track_detail
                                         join s in _db.track_session on m.TrackSessionId equals s.Id
                                         where s.TrackId == item.Id
                                         select new { m.Url, m.FileName }).ToList();
                        foreach (var f in listImage)
                        {

                        }
                    }

                    return "";
                }
                catch (Exception ex)
                {
                    return "";
                }
                finally
                {
                    GC.Collect();
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
                    FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + detail.Url + detail.FileName); // remove old file
                    FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + "/WriteText" + detail.Url + detail.FileName); // remove old file

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
                        FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + item.Url + item.FileName); // remove old file
                        FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + "/WriteText" + item.Url + item.FileName);
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


        public string WriteTextToImageCustom(string text, string rootUrl, string sourceDirPath, string sourceFileName)
        {
            //string dir = Path.GetDirectoryName(rootUrl + sourceFilePath);
            string ext = Path.GetExtension(rootUrl + sourceDirPath + sourceFileName);

            string dir_target = rootUrl + "/WriteText" + sourceDirPath;
            if (!Directory.Exists(dir_target))
                Directory.CreateDirectory(dir_target);

            if (File.Exists(Path.Combine(dir_target, sourceFileName)))
                return Path.Combine(dir_target, sourceFileName);

            string target_file = "";
            if (File.Exists(rootUrl + sourceDirPath + sourceFileName))
            {
                string newFileName = Path.GetFileNameWithoutExtension(rootUrl + sourceDirPath + sourceFileName);
                string newFilenamePath = Path.Combine(dir_target, newFileName + ext);

                var bitmap = Image.FromFile(rootUrl + sourceDirPath + sourceFileName); // set 
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

                target_file = newFilenamePath;
            }

            return target_file;

        }

        /// <summary>
        /// Lưu trạng thái qc báo cáo
        /// </summary>
        /// <param name="trackID">Track id</param>
        /// <param name="status">Status: 0: Chưa phân loại, 1: Đạt, 2: Không đạt, 3: Chưa xem xét</param>
        /// <returns></returns>
        public MessageReturnModel UpdateQCStatus(string trackID, int status)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var track = _db.tracks.FirstOrDefault(m => m.Id == trackID);
                if (track != null)
                {
                    track.QCStatus = status;

                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = track.ToString()
                    };
                }

                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy!"
                };
            }
        }

        #endregion

        public MessageReturnModel DeleteTrack(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var track = _db.tracks.Find(id);
                if (track != null)
                {
                    var track_sessions = _db.track_session.Where(x => x.TrackId == id).ToList();
                    if (track_sessions.Count > 0)
                    {
                        for (int i = 0; i < track_sessions.Count; i++)
                        {
                            string trackSessionsId = track_sessions[i].Id;
                            var details = _db.track_detail.Where(x => x.TrackSessionId == trackSessionsId).ToList();
                            if (details.Count > 0)
                            {
                                foreach (var item in details)
                                {
                                    FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + item.Url + item.FileName); // remove old file
                                    FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + "/WriteText" + item.Url + item.FileName);
                                }
                            }
                        }
                    }
                    _db.tracks.Remove(track);
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
    }
}
