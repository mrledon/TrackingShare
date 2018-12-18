using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using EmployeeTracking.Core;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EmployeeTracking.API.Controllers
{
    public class MobileController : ApiController
    {
        private string rootMedia = @"" + WebConfigurationManager.AppSettings["rootMedia"];
        private string tempMedia = @"" + WebConfigurationManager.AppSettings["WebServerTempFolder"];
        private MediaTypeRepo _MediaTypeRepo;
        private DistrictRepo _DistrictRepo;
        private ProvinceRepo _ProvinceRepo;
        private WardRepo _WardRepo;
        private EmployeeRepo _EmployeeRepo;
        private TrackAttendanceRepo _TrackAttendanceRepo;
        private TrackRepo _TrackRepo;
        private TrackSessionRepo _TrackSessionRepo;
        private TrackDetailRepo _TrackDetailRepo;
        private StoreRepo _StoreRepo;
        private StoreTypeRepo _StoreTypeRepo;
        public MobileController()
        {
            _EmployeeRepo = new EmployeeRepo();
            _TrackAttendanceRepo = new TrackAttendanceRepo();
            _DistrictRepo = new DistrictRepo();
            _ProvinceRepo = new ProvinceRepo();
            _WardRepo = new WardRepo();
            _TrackRepo = new TrackRepo();
            _TrackSessionRepo = new TrackSessionRepo();
            _MediaTypeRepo = new MediaTypeRepo();
            _TrackDetailRepo = new TrackDetailRepo();
            _StoreTypeRepo = new StoreTypeRepo();
            _StoreRepo = new StoreRepo();
        }

        #region LOGIN

        [HttpPost]
        public object Login(EmployeeApiLoginModel model)
        {
            try
            {
                //Check version Mobile
                if ((model.Version ?? "").Length <= 0 || (model.Version ?? "") != WebConfigurationManager.AppSettings["mobileVersion"])
                    throw new Exception("Vui lòng cài đúng phiên bản App. Version " + WebConfigurationManager.AppSettings["mobileVersion"]);

                var obj = _EmployeeRepo.LoginAPI(model);
                _EmployeeRepo.DeleteEmpTokenExpire(model.Id);
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
                    throw new Exception("Không tìm thấy thời gian bắt đầu và thời gian kết thúc.");
                else if (!string.IsNullOrEmpty(model.AttendanceStart) && string.IsNullOrEmpty(model.AttendanceEnd))
                {
                    TimeSpan time = TimeSpan.Parse(model.AttendanceStart); //HH:mm:ss
                    if (_TrackAttendanceRepo.CheckAttendanceStart(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        Start = time
                    }))
                        throw new Exception("Hôm nay bạn đã thực hiện điểm danh bắt đầu.");

                    _TrackAttendanceRepo.AttendanceStart(new track_attendance()
                    {

                        Date = date,
                        EmployeeId = emp.Id,
                        Start = time,
                        StartCoordinates = model.StartCoordinates
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
                    if (_TrackAttendanceRepo.CheckAttendanceEnd(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        Start = time
                    }))
                        throw new Exception("Hôm nay bạn đã thực hiện điểm danh kết thúc.");

                    _TrackAttendanceRepo.AttendanceEnd(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        End = time,
                        EndCoordinates = model.EndCoordinates
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
            //string ErrorInput = "";

            try
            {

                int posmNumber = 0;
                Guid trackSessionId = Guid.Empty;

                try
                {
                    posmNumber = int.Parse(HttpContext.Current.Request.Params["PosmNumber"]);
                }
                catch { }
                try
                {
                    trackSessionId = new Guid(HttpContext.Current.Request.Params["TrackSessionId"]);
                }
                catch
                {
                     trackSessionId = Guid.Empty;
                }


                var model = new TrackingDataFileModel()
                {
                    Id = (HttpContext.Current.Request.Params["Id"] ?? ""),
                    Code = HttpContext.Current.Request.Params["Code"],
                    Code2 = HttpContext.Current.Request.Params["Code2"],
                    Date = HttpContext.Current.Request.Params["Date"],
                    //MasterStoreId = new Guid(HttpContext.Current.Request.Params["MasterStoreId"]),
                    Token = HttpContext.Current.Request.Params["Token"],
                    TrackSessionId = trackSessionId,
                    PosmNumber = posmNumber
                    //OriginalFileName = HttpContext.Current.Request.Params["OriginalFileName"]
                };

                if (model == null)
                    throw new Exception("Request data false !");

                var dnow = DateTime.Now;
                //var d = DateTime.ParseExact(model.Date, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime d;
                if (!DateTime.TryParseExact(model.Date, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                {
                    d = dnow;
                }


                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Kiểm tra xác thực nhân viên không đúng. vui lòng đăng nhập lại !");

                var mediaType = _MediaTypeRepo.GetByCode(model.Code);
                if (mediaType == null)
                    throw new Exception("Không tìm thấy loại hình ảnh !");

                //Kiem tra model.TrackSessionId = null || _blank => hardcode model.TrackSessionId = "xxxx"
                if (model.TrackSessionId == Guid.Empty)
                {
                    model.TrackSessionId = new Guid("3ac8cb31-f61c-4b4e-a191-15aa4c883315");
                }
                var tracksession = _TrackSessionRepo.getById(model.TrackSessionId.ToString());
                if (tracksession == null)
                    throw new Exception("Vui lòng nhập thông tin Cửa hàng !");

                if (HttpContext.Current.Request.Files.Count != 1)
                    throw new Exception("Vui lòng up 1 hình !");


                var store = _StoreRepo.getstoreByTrackSSId(tracksession.Id);
                string storeId = (store == null) ? Guid.NewGuid().ToString() : store.Id.ToString();
                //List<Task<InputUploadFile>> tasksInput = new List<Task<InputUploadFile>>();
                //for (int f = 0; f < HttpContext.Current.Request.Files.Count; f++)
                //{
                //    var file = HttpContext.Current.Request.Files[f];
                //    tasksInput.Add(SaveImageFromClient(emp, file, rootMedia, string.Format("/{0}/{1}/{2}/{3}/{4}/", d.Year, d.Month, d.Day, storeId, model.Code)));
                //}
                //Task.WhenAll(tasksInput);


                var rssaveimage = SaveImageFromClient(emp, HttpContext.Current.Request.Files[0], rootMedia, string.Format("/{0}/{1}/{2}/{3}/{4}/", d.Year, d.Month, d.Day, storeId, model.Code));


                if (rssaveimage == null)
                    throw new Exception("Không lưu được hình ảnh. At :[" + (model.Id ?? "") + "][" + (model.Code ?? "") + "][" + (model.Code2 ?? "") + "][" + model.TrackSessionId + "][" + HttpContext.Current.Request.Files[0].FileName + "]");


                _TrackDetailRepo.Insert(new track_detail()
                {
                    CreateBy = model.Id,
                    CreateDate = d,
                    EmployeeId = model.Id,
                    FileName = rssaveimage.FileName,
                    Url = rssaveimage.FileUrl,
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    MediaTypeId = model.Code,
                    TrackSessionId = tracksession.Id,
                    PosmNumber = model.PosmNumber,
                    MediaTypeSub = model.Code2,
                    OriginalFileName = rssaveimage.OriginalFileName,
                    OriginalFileSize = rssaveimage.ContentLength
                });




                //for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                //{
                //    var file = HttpContext.Current.Request.Files[i];

                //    if (file != null && file.ContentLength > 0)
                //    {
                //        //var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                //        var url = string.Format("/{0}/{1}/{2}/{3}/{4}/", d.Year, d.Month, d.Day, model.MasterStoreId, model.Code);
                //        if (!Directory.Exists((rootMedia + url)))
                //            Directory.CreateDirectory(rootMedia + url);

                //        string fguid = Guid.NewGuid().ToString();
                //        var newFileName = emp.Id + dnow.ToString("yyyyMMddHHmmss" + "-") + fguid + Path.GetExtension(file.FileName);
                //        var path = rootMedia + url + newFileName;

                //        file.SaveAs(path);
                //        _TrackDetailRepo.Insert(new track_detail()
                //        {
                //            CreateBy = model.Id,
                //            CreateDate = DateTime.Now,
                //            EmployeeId = model.Id,
                //            FileName = newFileName,
                //            Url = url,
                //            Id = Guid.NewGuid().ToString(),
                //            IsActive = true,
                //            MediaTypeId = model.Code,
                //            TrackSessionId = tracksession.Id,
                //            PosmNumber = model.PosmNumber
                //        });

                //    }
                //}



                if (!_TrackSessionRepo.updateStatus(tracksession.Id, true))
                    throw new Exception("Lỗi cập nhât!");


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
                        //Message = ErrorInput,
                        Data = null
                    });
            }
        }
        #endregion

        #region TRACKING SESSION
        [HttpPost]
        public object TrackingSession(TrackingDataModel model)
        {
            try
            {


                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");

                var d = DateTime.ParseExact(model.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Guid StoreId = new Guid();
                string newtrackId = "";
                track_session trackss = new track_session();
                //1. Insert New Store
                if (model.MasterStoreId == Guid.Empty)
                {
                    //2. Create Store
                    StoreManagerModel storeManagerModel = new StoreManagerModel
                    {
                        Id = Guid.NewGuid(),
                        Code = Guid.NewGuid().ToString(),
                        CreatedBy = model.Id,
                        CreatedDate = DateTime.UtcNow,
                        DistrictId = model.DistrictId,
                        HouseNumber = model.HouseNumber,
                        Name = model.MaterStoreName,
                        ProvinceId = model.ProvinceId,
                        Region = model.Region,
                        StoreType = model.StoreType,
                        StreetNames = model.StreetNames,
                        WardId = model.WardId,
                        PhoneNumber = model.PhoneNumber,
                        LAT = ((model.Lat ?? "").Length == 0) ? (double?)null : Convert.ToDouble(model.Lat),
                        LNG = ((model.Lng ?? "").Length == 0) ? (double?)null : Convert.ToDouble(model.Lng)
                    };
                    MessageReturnModel rt = _StoreRepo.Insert(storeManagerModel);
                    if (rt.IsSuccess)
                    {
                        StoreId = new Guid(rt.Id);
                    }
                    else
                    {
                        throw new Exception(rt.Message);
                    }

                    string track_id = _TrackRepo.Insert(new track()
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
                        MasterStoreId = StoreId,
                        Date = d,
                        StoreStatus = model.StoreStatus,
                        PhoneNumber = model.PhoneNumber,
                        StoreIsChanged = false,
                        StoreType = model.StoreType,
                        QCNote = ""
                    });
                    if (track_id != "")
                    {
                        newtrackId = track_id;
                    }
                    else
                    {
                        throw new Exception("Lỗi tạo báo cáo. Vui lòng thực hiện lại!");
                    }
                }
                else
                {
                    //3. 
                    StoreId = model.MasterStoreId;
                    //Update Coordinates
                    if ((model.Lat ?? "").Length > 0 && (model.Lng ?? "").Length > 0)
                    {
                        if (!_StoreRepo.UpdateCoordinates(StoreId, Convert.ToDouble(model.Lat), Convert.ToDouble(model.Lng)).IsSuccess)
                            throw new Exception("Cập nhật tọa độ cửa hàng không thành công. Vui lòng thử lại.");
                    }
                    var trackModel = _TrackRepo.GetTrackByDate(model.MasterStoreId, model.Id, d);
                    if (trackModel == null)
                    {
                        string track_id = _TrackRepo.Insert(new track()
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
                            MasterStoreId = StoreId,
                            Date = d,
                            StoreStatus = model.StoreStatus,
                            PhoneNumber = model.PhoneNumber,
                            //StoreIsChanged = false,
                            StoreIsChanged = model.StoreIsChanged,
                            StoreType = model.StoreType
                        });
                        if (track_id != "")
                        {
                            newtrackId = track_id;
                        }
                        else
                        {
                            throw new Exception("Lỗi tạo báo cáo. Vui lòng thực hiện lại!");
                        }
                    }
                    else
                    {
                        newtrackId = trackModel.Id;
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
                        trackModel.StoreStatus = model.StoreStatus;
                        trackModel.PhoneNumber = model.PhoneNumber;
                        //trackModel.StoreIsChanged = true;
                        trackModel.StoreIsChanged = model.StoreIsChanged;
                        trackModel.StoreType = model.StoreType;
                        _TrackRepo.UpdateFromMobile(trackModel);
                    }


                }

                //if (newtrackId == "")
                //    throw new Exception("Lỗi tạo báo cáo. Vui lòng thực hiện lại!");

                var obj = _TrackSessionRepo.Insert(new track_session
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedBy = emp.Id,
                    CreatedDate = DateTime.UtcNow,
                    TrackId = newtrackId,
                    Date = d,
                    Status = false,
                    IsEndSession = false
                });
                return Json(
                    new JsonResultModel<object>()
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

        [HttpPost]
        public object TrackingSessionEnd(string trackSessionId)
        {
            try
            {
                if (_TrackSessionRepo.getById(trackSessionId) == null)
                    throw new Exception("Lỗi không tìm thấy phiên làm việc !");
                if (!_TrackSessionRepo.EndOfSession(trackSessionId))
                    throw new Exception("Lỗi lưu kết thúc phiên làm việc !");
                return Json(
                    new JsonResultModel<object>()
                    {
                        HasError = false,
                        Message = "",
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
        #endregion TRACKING SESSION

        #region TRACKING DETAIL
        [HttpPost]
        public object Tracking_Detail(TrackingInforModel model)
        {
            try
            {
                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");


                var obj = _TrackRepo.GetTrackDoneByEmployeeId(model.Id);
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

        #region Store

        [HttpPost]
        public object GetStoreByCode(StoreApiModel model)
        {
            try
            {
                var obj = _StoreRepo.GetByCode(model.Code);


                if (obj != null)
                {
                    return Json(new JsonResultModel<master_store>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = obj
                    });
                }
                else
                {
                    return Json(new JsonResultModel<master_store>()
                    {
                        HasError = true,
                        Message = "Chưa có mã cửa hàng này!",
                        Data = null
                    });
                }

            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<master_store>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = null
                    });
            }
        }



        [HttpPost]
        public object StoreType()
        {
            try
            {
                var obj = _StoreTypeRepo.GetAll();
                return Json(
                   new JsonResultModel<IList<master_store_type>>()
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


        #endregion Store

        #region InputFile
        public InputUploadFile SaveImageFromClient(employee emp, HttpPostedFile file, string LocationFolder, string url)
        {
            try
            {
                string conteType = file.ContentType;
                var typeMapping = ExtensionClass._imagesMappingDictionary.Where(val => val.Value.Contains(conteType)).First();

                if (!Directory.Exists((LocationFolder + url)))
                    Directory.CreateDirectory(LocationFolder + url);

                string fguid = Guid.NewGuid().ToString();
                var newFileName = emp.Id + DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid + typeMapping.Key;
                var path = LocationFolder + url + newFileName;
                //file.SaveAs(path);
                bool _GenerateThumbnailRs = false;
                int FileLen = 0;
                using (System.IO.Stream MyStream = file.InputStream)
                {
                    FileLen = file.ContentLength;
                    byte[] input = new byte[FileLen];
                    MyStream.Read(input, 0, FileLen);
                    _GenerateThumbnailRs = GenerateThumbnails(1, MyStream, path);
                }
                if (!_GenerateThumbnailRs)
                    throw new Exception("False to GenerateThumbnails.");

                return new InputUploadFile()
                {
                    FileName = newFileName,
                    OriginalFileName = file.FileName,
                    ContentType = conteType,
                    ContentLength = FileLen,
                    FileUrl = url
                };
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        private bool GenerateThumbnails(double scaleFactor, Stream sourcePath, string targetPath)
        {
            try
            {
                using (var image = Image.FromStream(sourcePath))
                {
                    var newWidth = (int)(image.Width * scaleFactor);
                    var newHeight = (int)(image.Height * scaleFactor);
                    var thumbnailImg = new Bitmap(newWidth, newHeight);
                    var thumbGraph = Graphics.FromImage(thumbnailImg);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    thumbGraph.DrawImage(image, imageRectangle);
                    thumbnailImg.Save(targetPath, image.RawFormat);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion InputFile
        [HttpGet]
        public string ping()
        {
            return DateTime.UtcNow.ToString();
        }
    }
}