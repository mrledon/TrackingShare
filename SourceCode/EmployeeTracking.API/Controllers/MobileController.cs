using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EmployeeTracking.API.Controllers
{
    public class MobileController : ApiController
    {
        private string rootMedia = @"E:/Image";
        private MediaTypeRepo _MediaTypeRepo;
        private DistrictRepo _DistrictRepo;
        private ProvinceRepo _ProvinceRepo;
        private WardRepo _WardRepo;
        private EmployeeRepo _EmployeeRepo;
        private TrackAttendanceRepo _TrackAttendanceRepo;
        private TrackRepo _TrackRepo;
        private TrackDetailRepo _TrackDetailRepo;
        public MobileController()
        {
            _EmployeeRepo = new EmployeeRepo();
            _TrackAttendanceRepo = new TrackAttendanceRepo();
            _DistrictRepo = new DistrictRepo();
            _ProvinceRepo = new ProvinceRepo();
            _WardRepo = new WardRepo();
            _TrackRepo = new TrackRepo();
            _MediaTypeRepo = new MediaTypeRepo();
            _TrackDetailRepo = new TrackDetailRepo();
        }

        #region LOGIN

        [HttpPost]
        public object Login(employee model)
        {
            try
            {
                var obj = _EmployeeRepo.LoginAPI(model);
                return Json(
                   new JsonResultModel<EmployeeApiModel>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
            }
        }
        #endregion


        #region ATTENDANCE
        [HttpPost]
        public object Attendance(AttendanceApiModel model)
        {
            try
            {
                var date = DateTime.Now;

                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");

                if (string.IsNullOrEmpty(model.AttendanceStart) && string.IsNullOrEmpty(model.AttendanceEnd))
                    throw new Exception("Please input Start or End value");
                else if (!string.IsNullOrEmpty(model.AttendanceStart) && string.IsNullOrEmpty(model.AttendanceEnd))
                {
                    TimeSpan time = TimeSpan.Parse(model.AttendanceStart); //HH:mm:ss
                    _TrackAttendanceRepo.AttendanceStart(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        Start = time
                    });

                    return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
                }
                else if (!string.IsNullOrEmpty(model.AttendanceEnd) && string.IsNullOrEmpty(model.AttendanceStart))
                {
                    TimeSpan time = TimeSpan.Parse(model.AttendanceEnd); //HH:mm:ss
                    _TrackAttendanceRepo.AttendanceEnd(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        End = time
                    });

                    return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
                }
                else
                    throw new Exception("");
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
            }
        }
        #endregion



        #region TRACKING
        [HttpPost]
        public object Provinces()
        {
            try
            {
                var obj = _ProvinceRepo.GetAll();
                return Json(
                   new JsonResultModel<IList<province>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<province>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<province>()
                    });
            }
        }
        [HttpPost]
        public object Districts(DistrictApiModel model)
        {
            try
            {
                var obj = _DistrictRepo.GetByProvinceId(model.ProvinceId);
                return Json(
                   new JsonResultModel<IList<district>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<district>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<district>()
                    });
            }
        }
        [HttpPost]
        public object Wards(WardApiModel model)
        {
            try
            {
                var obj = _WardRepo.GetByDistrictId(model.DistrictId);
                return Json(
                   new JsonResultModel<IList<ward>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<ward>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<ward>()
                    });
            }
        }


        [HttpPost]
        public object TrackingData(TrackingDataModel model)
        {
            try
            {
                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");

                var d = DateTime.ParseExact(model.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var trackModel = _TrackRepo.GetTrackByDate(model.MasterStoreId, model.Id, d);

                if (trackModel == null)
                {
                    _TrackRepo.Insert(new track()
                    {
                        DistrictId = model.DistrictId,
                        HouseNumber = model.HouseNumber,
                        Lat = model.Lat,
                        Lng = model.Lng,
                        MaterStoreName = model.MaterStoreName,
                        Note = model.Note,
                        ProvinceId = model.ProvinceId,
                        Region = model.Region,
                        StreetNames = model.StreetNames,
                        WardId = model.WardId,
                        CreateDate = DateTime.Now,
                        EmployeeId = model.Id,
                        Id = Guid.NewGuid().ToString(),
                        MasterStoreId = model.MasterStoreId,
                        Date = d
                    });
                }
                else
                {
                    trackModel.DistrictId = model.DistrictId;
                    trackModel.HouseNumber = model.HouseNumber;
                    trackModel.Lat = model.Lat;
                    trackModel.Lng = model.Lng;
                    trackModel.MaterStoreName = model.MaterStoreName;
                    trackModel.Note = model.Note;
                    trackModel.ProvinceId = model.ProvinceId;
                    trackModel.Region = model.Region;
                    trackModel.StreetNames = model.StreetNames;
                    trackModel.WardId = model.WardId;
                    _TrackRepo.UpdateFromMobile(trackModel);
                }
                return Json(
                    new JsonResultModel<object>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = null
                    });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<object>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = null
                    });
            }
        }


        [HttpPost]
        public object TrackingDataMedia()
        {
            try
            {
                var model = new TrackingDataFileModel()
                {
                    Id = HttpContext.Current.Request.Params["Id"],
                    Code = HttpContext.Current.Request.Params["Code"],
                    Date = HttpContext.Current.Request.Params["Date"],
                    MasterStoreId = new Guid(HttpContext.Current.Request.Params["MasterStoreId"]),
                    Token = HttpContext.Current.Request.Params["Token"]
                };

                var d = DateTime.ParseExact(model.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var dnow = DateTime.Now;
                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");

                var mediaType = _MediaTypeRepo.GetByCode(model.Code);
                if (mediaType == null)
                    throw new Exception("MediaType not found.");


                var track = _TrackRepo.GetTrackByDate(model.MasterStoreId, model.Id, d);
                if (track == null)
                    throw new Exception("Please update basic information.");

                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var file = HttpContext.Current.Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var url = string.Format("/{0}/{1}/{2}/{3}/{4}/", d.Year, d.Month, d.Day, model.MasterStoreId, model.Code);
                        if (!Directory.Exists((rootMedia + url)))
                            Directory.CreateDirectory(rootMedia + url);

                        var newFileName = dnow.ToString("yyyyMMddHHmmss" + "-") + fileName;
                        var path = rootMedia + url + newFileName;

                        file.SaveAs(path);


                        _TrackDetailRepo.Insert(new track_detail()
                        {
                            CreateBy = model.Id,
                            CreateDate = DateTime.Now,
                            EmployeeId = model.Id,
                            FileName = newFileName,
                            Url = url,
                            Id = Guid.NewGuid().ToString(),
                            IsActive = true,
                            MediaTypeId = model.Code,
                            TrackId = track.Id
                        });

                    }
                }

                return Json(
                    new JsonResultModel<object>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = null
                    });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<object>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = null
                    });
            }
        }
        #endregion




        #region TRACKING DETAIL
        [HttpPost]
        public object Tracking_Detail(TrackingInforModel model)
        {
            try
            {
                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");


                var obj = _TrackRepo.GetTrackByEmployeeId(model.Id);
                return Json(
                   new JsonResultModel<IList<TrackMinModel>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<TrackMinModel>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<TrackMinModel>()
                    });
            }
        }
        #endregion

    }
}