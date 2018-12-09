using EmployeeTracking.Admin.Filters;
using EmployeeTracking.Core;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Core.Utils;
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class ImageManagementController : Controller
    {
        private string rootMedia = @"" + WebConfigurationManager.AppSettings["rootMedia"];
        private string tempMedia = @"" + WebConfigurationManager.AppSettings["WebServerTempFolder"];
        //private string rootMedia = @"D:\test image";

        private ImageManagementRepo _imageManagementRepo;
        private MediaTypeRepo _mediaTypeRepo;
        private StoreRepo _StoreRepo;
        private TrackDetailRepo _trackDetailRepo;
        private EmployeeRepo _employeeRepo;
        private TrackRepo _tr;

        public ImageManagementController()
        {
            _imageManagementRepo = new ImageManagementRepo();
            _mediaTypeRepo = new MediaTypeRepo();
            _StoreRepo = new StoreRepo();
            _trackDetailRepo = new TrackDetailRepo();
            _employeeRepo = new EmployeeRepo();
            _tr = new TrackRepo();
        }


        // GET: ImageManagement
        [CheckLoginFilter]
        public ActionResult Index()
        {

            ViewBag.employee = _employeeRepo.GetListToShowOnCombobox();
            //ViewBag.store = _StoreRepo.GetListStoreToShowOnCombobox();
            ViewBag.region = _StoreRepo.GetListRegionToShowOnCombobox();


            return View();
        }



        /// <summary>
        /// Group form
        /// Post method
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>DataTableResponse<GroupModel></returns>
        [HttpPost]
        [CheckLoginFilter]
        public JsonResult Index(CustomDataTableRequestHelper requestData)
        {
            //try
            //{
            #region " [ Declaration ] "

            #endregion

            #region " [ Main processing ] "

            if (requestData.FromDate == null) // From date
            {
                requestData.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (requestData.ToDate == null) // To date
            {
                requestData.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (requestData.Region == null) // Region
            {
                requestData.Region = new List<string>();
            }
            if (requestData.Store == null) // Store
            {
                requestData.Store = new List<string>();
            }
            if (requestData.Employee == null) // Employee
            {
                requestData.Employee = new List<string>();
            }
            // Process sorting column
            //requestData = requestData.SetOrderingColumnName();

            #endregion

            //Call to service
            Dictionary<string, object> _return = _imageManagementRepo.List(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<ImageManagementViewModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<ImageManagementViewModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<ImageManagementViewModel>(), JsonRequestBehavior.AllowGet);
            //}
            //catch (ServiceException serviceEx)
            //{
            //    throw serviceEx;
            //}
            //catch (DataAccessException accessEx)
            //{
            //    throw accessEx;
            //}
            //catch (Exception ex)
            //{
            //    throw new ControllerException(FILE_NAME, MethodInfo.GetCurrentMethod().Name, UserID, ex);
            //}
        }

        [CheckLoginFilter]
        public ActionResult AddNew(string trackId, string employeeId, string masterStoreId)
        {
            try
            {
                AddImageModel model = new AddImageModel();
                model.DateUpdate = DateTime.Now;
                model.EmployeeId = employeeId;
                model.MasterStoreId = masterStoreId;
                model.TrackId = trackId;
                model.FileUploads = new List<FileUploadModel>();
                var allMediaType = _mediaTypeRepo.GetAll().Where(x => x.Code != "DEFAULT" && x.Code != "STORE_FAILED" && x.Code != "SELFIE");
                foreach (var item in allMediaType)
                {
                    FileUploadModel file = new FileUploadModel();
                    file.PosmNumber = 0;
                    file.TypeId = item.Code;
                    file.TypeName = item.Name;
                    model.FileUploads.Add(file);
                }
                @ViewBag.DateUpdate = DateTime.Now.ToString("yyyy-MM-dd");
                return PartialView("~/Views/ImageManagement/_AddNew.cshtml", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CheckLoginFilter]
        public ActionResult AddNew_Submit(AddImageModel DocumentModel)
        {
            AddImageModel modelSubmit = new AddImageModel();
            modelSubmit.DateUpdate = DocumentModel.DateUpdate;
            modelSubmit.CreateDate = DateTime.Now;
            modelSubmit.CreateBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
            modelSubmit.EmployeeId = DocumentModel.EmployeeId;
            modelSubmit.MasterStoreId = DocumentModel.MasterStoreId;
            modelSubmit.TrackId = DocumentModel.TrackId;
            modelSubmit.FileUploads = new List<FileUploadModel>();
            string urlFile = String.Format("/{0}/{1}/{2}/{3}/", modelSubmit.DateUpdate.Year, modelSubmit.DateUpdate.Month, modelSubmit.DateUpdate.Day, modelSubmit.MasterStoreId);
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase fileData = Request.Files[file];
                if (fileData != null && fileData.ContentLength > 0)
                {
                    // Get Mediatype and subtype
                    string type = "";
                    string subType = "";
                    var stringSplit = file.Split('-');
                    type = stringSplit[0];
                    if (stringSplit.Length > 1)
                    {
                        subType = stringSplit[1];
                    }
                    var url = urlFile + type + "/";
                    //get posm Number
                    int posmNumber = 0;
                    try
                    {
                        posmNumber = DocumentModel.FileUploads.Where(x => x.TypeId == type).FirstOrDefault().PosmNumber;
                    }
                    catch { }
                    // Get file info
                    var fileName = Path.GetFileName(fileData.FileName);
                    string fguid = Guid.NewGuid().ToString();
                    var newFileName = fileName.Replace(Path.GetFileNameWithoutExtension(fileData.FileName), DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid);
                    var path = Path.Combine(rootMedia + url, newFileName);
                    if (!Directory.Exists(rootMedia + url))
                        Directory.CreateDirectory(rootMedia + url);
                    // create model file
                    FileUploadModel fileModel = new FileUploadModel();
                    fileModel.FileName = newFileName;
                    fileModel.FilePath = url;
                    fileModel.TypeId = type;
                    if (!string.IsNullOrEmpty(subType))
                    {
                        if (subType.Equals("PXN"))
                            fileModel.SubType = "HINH_KY_PXN";
                        if (subType.Equals("PXNFULL"))
                            fileModel.SubType = "HINH_PXN_FULL";
                        if (subType.Equals("SPVB"))
                            fileModel.SubType = "HINH_SPVB";
                        if (subType.Equals("GENERAL"))
                            fileModel.SubType = "HINH_TONG_QUAT";
                        if (subType.Equals("ADDRESS"))
                            fileModel.SubType = "HINH_DIA_CHI";
                    }
                    fileModel.PosmNumber = posmNumber;
                    modelSubmit.FileUploads.Add(fileModel);
                    // Save file
                    fileData.SaveAs(path);
                }
            }
            _trackDetailRepo.InsertImageAdmin(modelSubmit);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        [CheckLoginFilter]
        //[RoleFilter(ActionName = "Edit.A")]
        public ActionResult EditTrackSession(string id)
        {
            ViewBag.StoreInfo = _imageManagementRepo.GetStoreInfoByTrackSessionId(id);

            //ViewBag.StoreInfo = _imageManagementRepo.GetStoreInfoByTrackId(id);

            //ViewBag.POSM
            ViewBag.PosmType = _mediaTypeRepo.GetOnlyPOSM();

            var model = _imageManagementRepo.GetTrackDetailListByTrackSessionId(id);

            model.ForEach(f =>
            {
                f.TrackDetailImages.ToList().ForEach(_ =>
                {
                    _.Url = WebConfigurationManager.AppSettings["rootMediaURl"] + _.Url;
                });
            });
            ViewBag.TrackSessionsId = id;
            return PartialView("_EditTrackSession", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackId</param>
        /// <returns></returns>
        [CheckLoginFilter]
        public ActionResult ExpandTrack(string id)
        {
            var model = _imageManagementRepo.GetStoreInfoByTrackId(id);
            ViewBag.TrackSessionList = _imageManagementRepo.GetTrackSessionListByTrackId(id);
            ViewBag.PosmInfo = _imageManagementRepo.GetPOSMStatisticByTrackID(id);
            ViewBag.TrackId = id;
            return PartialView("_ExpandTrack", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        [CheckLoginFilter]
        public ActionResult TrackSessionCarousel(string id, string TrackIdForCarousel)
        {
            var model = _imageManagementRepo.GetTrackDetailListByTrackSessionId(id);
            model.ForEach(f =>
            {
                f.TrackDetailImages.ToList().ForEach(_ =>
                {
                    _.Url = WebConfigurationManager.AppSettings["rootMediaURl"] + _.Url;
                });
            });
            Response.Headers["trackSessionIdForCarousel"] = TrackIdForCarousel;
            ViewBag.TrackIdForCarousel = TrackIdForCarousel;
            return PartialView("_TrackSessionCarousel", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        [CheckLoginFilter]
        public ActionResult DownloadImagePackage(string id)
        {
            var sessionList = _imageManagementRepo.GetTrackDetailListByTrackSessionId(id);

            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {

                    foreach (var item in sessionList)
                    {
                        foreach (var image in item.TrackDetailImages)
                        {
                            try
                            {
                                var path = rootMedia + Path.Combine(image.Url, image.FileName);
                                byte[] file = System.IO.File.ReadAllBytes(path);

                                var zipArchiveEntry = archive.CreateEntry($"{item.MediaTypeName}\\{image.FileName}", CompressionLevel.Fastest);
                                using (var zipStream = zipArchiveEntry.Open()) zipStream.Write(file, 0, file.Length);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                // TODO: đặt tên file
                return File(ms.ToArray(), "application/zip", "Archive.zip");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckLoginFilter]
        public ActionResult ChangeDetailImage(ChangeDetailImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy id! Vui lòng tải lại trang."
                });
            }

            var detail = _imageManagementRepo.GetSessionDetailById(model.SessionDetailId);
            if (detail == null)
            {
                return Json(new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy id! Vui lòng tải lại trang."
                });
            }

            if (Request.Files.Count > 0)
            {
                var file = Request.Files["SessionDetailImage"];
                if (file != null && file.ContentLength > 0)
                {
                    var typeMappings = ExtensionClass._imagesMappingDictionary.Where(val => val.Value.Contains(file.ContentType));
                    if (typeMappings.Any())
                    {
                        var typeMapping = typeMappings.First();

                        string fguid = Guid.NewGuid().ToString();
                        var fileName = DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid + typeMapping.Key;

                        var store = _StoreRepo.getstoreByTrackSSId(detail.TrackSessionId);
                        string storeId = (store == null) ? Guid.NewGuid().ToString() : store.Id.ToString();

                        var directoryGen = $"/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}/{storeId}/{detail.MediaTypeId}/";
                        var directory = rootMedia + directoryGen;

                        Directory.CreateDirectory(directory);
                        var path = Path.Combine(directory, fileName);

                        file.SaveAs(path);
                        // upload done

                        FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + detail.Url + detail.FileName); // remove old file
                        FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + "/WriteText" + detail.Url + detail.FileName);
                        detail.Url = directoryGen;
                        detail.FileName = fileName;

                        var rs = _imageManagementRepo.UpdateSessionDetail(detail);

                        return Json(rs);
                    }
                }
            }

            return Json(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">track detail id</param>
        /// <returns></returns>
        [HttpPost]
        [CheckLoginFilter]
        public ActionResult DeleteTrackDetail(string id)
        {
            var rs = _imageManagementRepo.DeleteTrackDetail(id);
            return Json(rs);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">track session id</param>
        /// <returns></returns>
        [HttpPost]
        [CheckLoginFilter]
        public ActionResult DeleteTrackSession(string id)
        {
            var rs = _imageManagementRepo.DeleteTrackSession(id);
            return Json(rs);
        }

        /// <summary>
        /// Save excel file
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Excel file</returns>
        [HttpGet]
        [CheckLoginFilter]
        [DeleteFileAttribute]
        public FileResult ExportExcelTrack(string FromDate, string ToDate, string Region, string Store, string Employee)
        {
            string templatePath = Server.MapPath("~/ExcelTemplate/ReportTemplate.xlsx");
            string tempFolderPath = Server.MapPath("~/temp");

            List<string> _region = new List<string>();
            List<string> _store = new List<string>();
            List<string> _employee = new List<string>();

            if (FromDate.Length == 0) // From date
            {
                FromDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (ToDate.Length == 0) // To date
            {
                ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (Region.Length == 0) // Region
            {
                _region = new List<string>();
            }
            else
            {
                string[] tmp = Region.Split(',');
                foreach (var item in tmp)
                {
                    if (item.Length == 0)
                        continue;
                    _region.Add(item);
                }
            }
            if (Store.Length == 0) // Store
            {
                _store = new List<string>();
            }
            else
            {
                string[] tmp = Store.Split(',');
                foreach (var item in tmp)
                {
                    if (item.Length == 0)
                        continue;
                    _store.Add(item);
                }
            }
            if (Employee.Length == 0) // Employee
            {
                _employee = new List<string>();
            }
            else
            {
                string[] tmp = Employee.Split(',');
                foreach (var item in tmp)
                {
                    if (item.Length == 0)
                        continue;
                    _employee.Add(item);
                }
            }
            string fileName = _imageManagementRepo.GetExportTrackListImg(FromDate, ToDate, _region, _store, _employee, templatePath, tempFolderPath);

            //save the file to server temp folder
            //string fullPath = Path.Combine(Server.MapPath("~/temp"), fileName);

            //System.IO.File.WriteAllBytes(fullPath, bin);

            return File(Server.MapPath("~/temp/" + fileName), "application/vnd.ms-excel", fileName);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


        /*Hieu.pt Show popup Update store in track*/
        [CheckLoginFilter]
        public ActionResult GetDetail(string id)
        {
            TrackViewModel obj = new TrackViewModel();
            try
            {
                obj = _tr.GetTrackById(id);
                return PartialView("~/Views/ImageManagement/PopupDetail.cshtml", obj);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        /*Hieu.pt Update store in track*/
        [AcceptVerbs(HttpVerbs.Post)]
        [CheckLoginFilter]
        public JsonResult PostDetail(TrackViewModel param)
        {
            try
            {
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    result = _tr.Update(param);
                    return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Thiếu hoặc sai thông tin cửa hàng", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        /*Hieu.pt Show popup update posm in track_detail*/
        [CheckLoginFilter]
        public ActionResult GetPosm(TrackPosmStatisticViewModel param)
        {
            try
            {
                return PartialView("~/Views/ImageManagement/PopupUpdatePosm.cshtml", param);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }


        /*Hieu.pt Update posm in track_detail*/
        [CheckLoginFilter]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PosmUpdate(TrackPosmStatisticViewModel param)
        {
            try
            {
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    result = _trackDetailRepo.Update(param);
                    return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Thiếu hoặc sai thông tin cửa hàng", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SavePosmType(string trackSessionsId, string mediaTypeId, int ValuePosmOfMediaType, string OldMediaTypeId)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _trackDetailRepo.SavePosmType(trackSessionsId, mediaTypeId, ValuePosmOfMediaType, OldMediaTypeId);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        /// <summary>
        /// Lưu trạng thái QC cập nhật
        /// </summary>
        /// <param name="trackId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [CheckLoginFilter]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateQCStatus(string trackId, int status)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _imageManagementRepo.UpdateQCStatus(trackId, status);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckLoginFilter]
        public ActionResult getPopupUpdateStoreStatus(string id)
        {
            var model = _tr.GetTrackById(id);
            return PartialView("PopupUpdateStoreStatus", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStoreStatus(TrackViewModel model)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _tr.UpdateStoreStatus(model);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [CheckLoginFilter]
        public ActionResult getPopupUpdateEmployee(string id)
        {
            var model = _tr.GetTrackById(id);
            return PartialView("PopupUpdateEmployee", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateEmployee(TrackViewModel model)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _tr.UpdateEmployee(model);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">track session id</param>
        /// <returns></returns>
        [HttpPost]
        [CheckLoginFilter]
        public ActionResult DeleteTrack(string id)
        {
            var rs = _imageManagementRepo.DeleteTrack(id);
            return Json(rs);
        }

        [HttpPost]
        [CheckLoginFilter]
        public JsonResult AddImage(HttpPostedFileBase inputFile, string TrackSessionsId, string MediaTypeId, int PosmNumber, string TypeSub)
        {
            
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                var track_sessions = _trackDetailRepo.GetTrackSessionById(TrackSessionsId);
                AddImageModel modelSubmit = new AddImageModel();
                modelSubmit.DateUpdate = DateTime.Now;
                modelSubmit.CreateDate = DateTime.Now;
                modelSubmit.CreateBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                modelSubmit.EmployeeId = track_sessions.EmployeeId;
                modelSubmit.MasterStoreId = track_sessions.MasterStoreId;
                modelSubmit.TrackId = track_sessions.TrackId;
                modelSubmit.FileUploads = new List<FileUploadModel>();
                string urlFile = String.Format("/{0}/{1}/{2}/{3}/", modelSubmit.DateUpdate.Year, modelSubmit.DateUpdate.Month, modelSubmit.DateUpdate.Day, modelSubmit.MasterStoreId);
                HttpPostedFileBase fileData = inputFile;
                if (fileData != null && fileData.ContentLength > 0)
                {
                    // Get Mediatype and subtype
                    var url = urlFile + MediaTypeId + "/";
                    //get posm Number
                    //try
                    //{
                    //    if (MediaTypeId == "DEFAULT" || MediaTypeId == "SELFIE")
                    //    {
                    //        posmNumber = 0;
                    //    }
                    //    //posmNumber = DocumentModel.FileUploads.Where(x => x.TypeId == type).FirstOrDefault().PosmNumber;
                    //}
                    //catch { }
                    // Get file info
                    var fileName = Path.GetFileName(fileData.FileName);
                    string fguid = Guid.NewGuid().ToString();
                    var newFileName = fileName.Replace(Path.GetFileNameWithoutExtension(fileData.FileName), DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid);
                    var path = Path.Combine(rootMedia + url, newFileName);
                    if (!Directory.Exists(rootMedia + url))
                        Directory.CreateDirectory(rootMedia + url);
                    // create model file
                    FileUploadModel fileModel = new FileUploadModel();
                    fileModel.FileName = newFileName;
                    fileModel.FilePath = url;
                    fileModel.TypeId = MediaTypeId;
                    fileModel.PosmNumber = PosmNumber;
                    modelSubmit.FileUploads.Add(fileModel);
                    modelSubmit.TrackSessionId = track_sessions.TrackSessionId;
                    if (TypeSub!=null)
                    {
                        modelSubmit.TypeSub = TypeSub;
                    }
                    // Save file
                    fileData.SaveAs(path);
                }
                result = _trackDetailRepo.InsertImageForTrackDetail(modelSubmit);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Id = track_sessions.TrackSessionId });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackSessionId"></param>
        /// <param name="employeeId"></param>
        /// <param name="masterStoreId"></param>
        /// <returns></returns>
        [HttpGet]
        [CheckLoginFilter]
        public ActionResult UnSubmitSession(string trackSessionId, string employeeId)
        {
            try
            {

                var fileImageModel = _imageManagementRepo.GetTrackDetailListByTrackSessionId(trackSessionId);

                fileImageModel.ForEach(f =>
                {
                    f.TrackDetailImages.ToList().ForEach(_ =>
                    {
                        _.Url = WebConfigurationManager.AppSettings["rootMediaURl"] + _.Url;
                    });
                });

                ViewBag.TrackSessionsId = trackSessionId;
                //
                AddImageModel model = new AddImageModel();
                model.DateUpdate = DateTime.Now;
                model.EmployeeId = employeeId;
                //model.MasterStoreId = masterStoreId;
                model.TrackId = trackSessionId;
                model.FileUploads = new List<FileUploadModel>();
                var allMediaType = _mediaTypeRepo.GetAllWithDefault();

                FileUploadModel file = new FileUploadModel();
                string tmp = "";

                foreach (var item in allMediaType)
                {
                    //Lấy thông tin fileupload theo mediaTypeId
                    var fileUploaded = fileImageModel.Where(m => m.MediaTypeId == item.Code).FirstOrDefault();

                    switch (item.Code)
                    {
                        case "DEFAULT":
                            #region " Default "

                            //trường hợp không tồn tại trackdetailimage thì tạo 2 file upload mới cho hình tổng quát và hình địa chỉ
                            if (fileUploaded == null || fileUploaded.TrackDetailImages.Count() == 0)
                            {

                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "HINH_TONG_QUAT";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);

                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "HINH_DIA_CHI";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }
                            else
                            {
                                //Trường hơp default chỉ có 1 hình được up lên từ mobile, gán phấn tử được chọn vào danh sách, phần tử thứ 2 sẽ gán tự động với subtype khác phần tử được lấy lên từ db
                                if (fileUploaded.TrackDetailImages.Count() == 1)
                                {
                                    var fDefault = fileUploaded.TrackDetailImages.First();
                                    file = new FileUploadModel();
                                    file.FileId = fDefault.Id;
                                    file.PosmNumber = fDefault.PosmNumber;
                                    file.FileName = fDefault.FileName;
                                    file.FilePath = fDefault.Url;
                                    file.TypeId = item.Code;
                                    file.TypeName = item.Name;
                                    file.SubType = fDefault.MediaTypeSub;
                                    model.FileUploads.Add(file);
                                    //
                                    tmp = "HINH_TONG_QUAT";
                                    if (fDefault.MediaTypeSub == "HINH_TONG_QUAT")
                                    {
                                        tmp = "HINH_DIA_CHI";
                                    }
                                    file = new FileUploadModel();
                                    file.TypeId = item.Code;
                                    file.TypeName = item.Name;
                                    file.SubType = tmp;
                                    file.FileName = "noimage.png";
                                    file.FilePath = "/Content/images/";
                                    model.FileUploads.Add(file);
                                }
                                else //Trường hợp từ mobile up lên đầy đủ 2 hình
                                {
                                    foreach (var fUpload in fileUploaded.TrackDetailImages)
                                    {
                                        file = new FileUploadModel();
                                        file.FileId = fUpload.Id;
                                        file.PosmNumber = fUpload.PosmNumber;
                                        file.FileName = fUpload.FileName;
                                        file.FilePath = fUpload.Url;
                                        file.TypeId = item.Code;
                                        file.TypeName = item.Name;
                                        file.SubType = fUpload.MediaTypeSub;
                                        model.FileUploads.Add(file);
                                    }
                                }
                            }

                            #endregion
                            break;
                        case "SELFIE":
                        case "STORE_FAILED":
                            //trường hợp mobile chưa up ảnh chấm công
                            if (fileUploaded == null || fileUploaded.TrackDetailImages.Count() == 0)
                            {
                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }
                            else //Trường hợp mobile đã up hình lên thì thêm vào danh sách
                            {
                                var fSelfie = fileUploaded.TrackDetailImages.First();
                                file = new FileUploadModel();
                                file.FileId = fSelfie.Id;
                                file.PosmNumber = fSelfie.PosmNumber;
                                file.FileName = fSelfie.FileName;
                                file.FilePath = fSelfie.Url;
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = fSelfie.MediaTypeSub;
                                model.FileUploads.Add(file);
                            }
                            break;
                        default:
                            #region " POSM "

                            //Kiểm tra ảnh PXN
                            var fPXN = fileUploaded == null ? null : fileUploaded.TrackDetailImages.FirstOrDefault(m => m.MediaTypeSub == "HINH_KY_PXN");
                            if(fPXN != null)
                            {
                                file = new FileUploadModel();
                                file.FileId = fPXN.Id;
                                file.PosmNumber = fPXN.PosmNumber;
                                file.FileName = fPXN.FileName;
                                file.FilePath = fPXN.Url;
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = fPXN.MediaTypeSub;
                                model.FileUploads.Add(file);
                            }
                            else
                            {
                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "HINH_KY_PXN";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }

                            //Kiểm tra ảnh HINH_PXN_FULL
                            var fPXNFull = fileUploaded == null ? null : fileUploaded.TrackDetailImages.FirstOrDefault(m => m.MediaTypeSub == "HINH_PXN_FULL");
                            if (fPXNFull != null)
                            {
                                file = new FileUploadModel();
                                file.FileId = fPXNFull.Id;
                                file.PosmNumber = fPXNFull.PosmNumber;
                                file.FileName = fPXNFull.FileName;
                                file.FilePath = fPXNFull.Url;
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = fPXNFull.MediaTypeSub;
                                model.FileUploads.Add(file);
                            }
                            else
                            {
                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "HINH_PXN_FULL";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }

                            //Kiểm tra ảnh HINH_SPVB
                            var fSpvb = fileUploaded == null ? null : fileUploaded.TrackDetailImages.FirstOrDefault(m => m.MediaTypeSub == "HINH_SPVB");
                            if (fSpvb != null)
                            {
                                file = new FileUploadModel();
                                file.FileId = fSpvb.Id;
                                file.PosmNumber = fSpvb.PosmNumber;
                                file.FileName = fSpvb.FileName;
                                file.FilePath = fSpvb.Url;
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = fSpvb.MediaTypeSub;
                                model.FileUploads.Add(file);
                            }
                            else
                            {
                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "HINH_SPVB";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }

                            //Kiểm tra ảnh Others
                            var fOthers = fileUploaded == null ? null : fileUploaded.TrackDetailImages.FirstOrDefault(m => m.MediaTypeSub == "");
                            if (fOthers != null)
                            {
                                file = new FileUploadModel();
                                file.FileId = fOthers.Id;
                                file.PosmNumber = fOthers.PosmNumber;
                                file.FileName = fOthers.FileName;
                                file.FilePath = fOthers.Url;
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = fOthers.MediaTypeSub;
                                model.FileUploads.Add(file);
                            }
                            else
                            {
                                file = new FileUploadModel();
                                file.TypeId = item.Code;
                                file.TypeName = item.Name;
                                file.SubType = "";
                                file.FileName = "noimage.png";
                                file.FilePath = "/Content/images/";
                                model.FileUploads.Add(file);
                            }

                            #endregion
                            break;

                    }

                }
                ViewBag.DateUpdate = DateTime.Now.ToString("yyyy-MM-dd");
                return PartialView("~/Views/ImageManagement/_UnSubmitSession.cshtml", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CheckLoginFilter]
        public ActionResult UpdateUnSubmitSession(AddImageModel DocumentModel, FormCollection fc)
        {
            AddImageModel modelSubmit = new AddImageModel();
            modelSubmit.DateUpdate = DateTime.Now;
            modelSubmit.CreateDate = DateTime.Now;
            modelSubmit.EmployeeId = DocumentModel.EmployeeId;
            modelSubmit.CreateBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
            modelSubmit.TrackId = DocumentModel.TrackId;
            modelSubmit.FileUploads = new List<FileUploadModel>();
            string urlFile = String.Format("/{0}/{1}/{2}/{3}/", modelSubmit.DateUpdate.Year, modelSubmit.DateUpdate.Month, modelSubmit.DateUpdate.Day, modelSubmit.MasterStoreId);
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase fileData = Request.Files[file];
                if (fileData != null && fileData.ContentLength > 0)
                {
                    // Get Mediatype and subtype
                    string type = "";
                    string subType = "";
                    var stringSplit = file.Split('-');
                    type = stringSplit[0];

                    if (stringSplit.Length > 1)
                    {
                        subType = stringSplit[1];
                    }
                    var url = urlFile + type + "/";
                    //get posm Number
                    int posmNumber = 0;
                    try
                    {
                        posmNumber = DocumentModel.FileUploads.Where(x => x.TypeId == type).FirstOrDefault().PosmNumber;
                    }
                    catch { }
                    // Get file info
                    var fileName = Path.GetFileName(fileData.FileName);
                    string fguid = Guid.NewGuid().ToString();
                    var newFileName = fileName.Replace(Path.GetFileNameWithoutExtension(fileData.FileName), DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid);
                    var path = Path.Combine(rootMedia + url, newFileName);
                    if (!Directory.Exists(rootMedia + url))
                        Directory.CreateDirectory(rootMedia + url);
                    // create model file
                    FileUploadModel fileModel = new FileUploadModel();
                    fileModel.FileName = newFileName;
                    fileModel.FilePath = url;
                    fileModel.TypeId = type;

                    switch (type)
                    {
                        case "DEFAULT":
                            switch (subType)
                            {
                                case "GENERAL":
                                    fileModel.SubType = "HINH_TONG_QUAT";
                                    fileModel.FileId = fc["_idDEFAULT-GENERAL"] == null ? "" : fc["_idDEFAULT-GENERAL"].ToString();
                                    break;
                                case "ADDRESS":
                                    fileModel.SubType = "HINH_DIA_CHI";
                                    fileModel.FileId = fc["_idDEFAULT-ADDRESS"] == null ? fc["_idDEFAULT-ADDRESS"] : fc["_idDEFAULT-ADDRESS"].ToString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "SELFIE":
                            fileModel.FileId = fc["_idSELFIE"] == null ? "" : fc["_idSELFIE"].ToString();
                            break;
                        case "STORE":
                            fileModel.FileId = fc["_idSTORE_FAIL"] == null ? "" : fc["_idSTORE_FAIL"].ToString();
                            break;
                        default:

                            try
                            {
                                posmNumber = int.Parse(fc["number_" + type].ToString());
                            }
                            catch { }

                            switch (subType)
                            {
                                case "PXN":
                                    fileModel.SubType = "HINH_KY_PXN";
                                    fileModel.FileId = fc[type + "_PXN-fileid"] == null ? "" : fc[type + "_PXN-fileid"].ToString();
                                    break;
                                case "PXNFULL":
                                    fileModel.SubType = "HINH_PXN_FULL";
                                    fileModel.FileId = fc[type + "_PXN_FULL-fileid"] == null ? "" : fc[type + "_PXN_FULL-fileid"].ToString();
                                    break;
                                case "SPVB":
                                    fileModel.SubType = "HINH_SPVB";
                                    fileModel.FileId = fc[type + "_SPVB-fileid"] ==  null ? "" : fc[type + "_SPVB-fileid"].ToString();
                                    break;
                                default:
                                    fileModel.SubType = "";
                                    fileModel.FileId = fc[file + "-Other-fileid"] == null ? "" : fc[file + "-Other-fileid"].ToString();
                                    break;
                            }
                            break;
                    }

                    fileModel.PosmNumber = posmNumber;
                    modelSubmit.FileUploads.Add(fileModel);
                    // Save file
                    fileData.SaveAs(path);
                }
            }
            _trackDetailRepo.UpdateUnSubmitTrackSession(modelSubmit.TrackId, modelSubmit);
            return RedirectToAction("Index");
        }

    }
}