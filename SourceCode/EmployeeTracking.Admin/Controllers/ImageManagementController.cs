﻿using EmployeeTracking.Admin.Filters;
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
        public JsonResult AddImage(HttpPostedFileBase inputFile, string TrackSessionsId, string MediaTypeId, int PosmNumber)
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
    }
}