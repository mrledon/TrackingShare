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
using EmployeeTracking.Admin.Controllers;
using System.IO.Compression;

namespace EmployeeTracking.Controllers
{
    public class ImageManagementController : BasicController
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
        [RoleFilter(ActionName = "ImageManager")]
        public ActionResult Index()
        {
            var account = (Data.Database.user)Session["Account"];
            string userId = account.Id.ToString();
            string tempFolderPath = Server.MapPath("~/temp/" + userId);

            if (Directory.Exists(tempFolderPath))
            {
                try
                {
                    Directory.Delete(tempFolderPath, true);
                }
                catch { }
            }

            ViewBag.employee = _employeeRepo.ListEmployeeByUserToShowCombobox(account.Id);
            //ViewBag.store = _StoreRepo.GetListStoreToShowOnCombobox();
            ViewBag.region = _StoreRepo.GetListRegionToShowOnCombobox();
            ViewBag.province = _StoreRepo.GetListProvinceToShowOnCombobox();

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
        //[RoleFilter(ActionName = "ImageManager_Search")]
        public JsonResult Index(CustomDataTableRequestHelper requestData)
        {
            var account = (Data.Database.user)Session["Account"];
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

            requestData.UserId = account.Id;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        [CheckLoginFilter]
        [RoleFilter(ActionName = "ImageManager_UpdateTrackSession")]
        public ActionResult EditTrackSession(string id)
        {
            var account = (Data.Database.user)Session["Account"];
            string userId = account.Id.ToString();
            string tempFolderPath = Server.MapPath("~/temp/" + userId);

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }

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
        [RoleFilter(ActionName = "ImageManager_DeleteTrackSession")]
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
        [RoleFilter(ActionName = "ImageManager_ExportExcel")]
        public FileResult ExportExcelTrack(string FromDate, string ToDate, string Region, string Store, string Employee)
        {
            var account = (Data.Database.user)Session["Account"];

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
            string fileName = _imageManagementRepo.GetExportTrackListImg(FromDate, ToDate, _region, _store, _employee, templatePath, tempFolderPath, account.Id);

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
        public JsonResult UpdateQCStatus(string trackId, int status, string qcNote)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _imageManagementRepo.UpdateQCStatus(trackId, status, qcNote);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckLoginFilter]
        [RoleFilter(ActionName = "ImageManager_ChangeStatus")]
        public ActionResult getPopupUpdateStoreStatus(string id)
        {
            var model = _tr.GetTrackById(id);
            return PartialView("PopupUpdateStoreStatus", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [RoleFilter(ActionName = "ImageManager_ChangeStatus")]
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
        [RoleFilter(ActionName = "ImageManager_ChangeEmployee")]
        public ActionResult getPopupUpdateEmployee(string id)
        {
            var model = _tr.GetTrackById(id);
            return PartialView("PopupUpdateEmployee", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [RoleFilter(ActionName = "ImageManager_ChangeEmployee")]
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

        [CheckLoginFilter]
        public ActionResult getPopupUpdateDate(string id)
        {
            var model = _tr.GetTrackById(id);
            return PartialView("PopupUpdateDate", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateDate(TrackViewModel model)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _tr.UpdateDate(model);
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
        [RoleFilter(ActionName = "ImageManager_Delete")]
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
                    if (TypeSub != null)
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
        [RoleFilter(ActionName = "ImageManager_UpdateImageNotSubmit")]
        public ActionResult UnSubmitSession(string trackSessionId, string employeeId)
        {
            try
            {
                var account = (Data.Database.user)Session["Account"];
                string userId = account.Id.ToString();
                string tempFolderPath = Server.MapPath("~/temp/" + userId);

                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }

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
                            if (fPXN != null)
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
                            var fOthers = fileUploaded == null ? null : fileUploaded.TrackDetailImages.Where(m => m.MediaTypeSub.Length == 0).ToList();
                            if (fOthers != null)
                            {
                                foreach (var f in fOthers)
                                {
                                    file = new FileUploadModel();
                                    file.FileId = f.Id;
                                    file.PosmNumber = f.PosmNumber;
                                    file.FileName = f.FileName;
                                    file.FilePath = f.Url;
                                    file.TypeId = item.Code;
                                    file.TypeName = item.Name;
                                    file.SubType = f.MediaTypeSub;
                                    model.FileUploads.Add(file);
                                }
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
            var account = (Data.Database.user)Session["Account"];
            string userId = account.Id.ToString();
            string tempFolderPath = Server.MapPath("~/temp/" + userId);
            //
            var allMediaType = _mediaTypeRepo.GetAll().Where(x => x.Code != "DEFAULT" && x.Code != "STORE_FAILED" && x.Code != "SELFIE");
            //
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
                    if (allMediaType.Count(m => m.Code == type) > 0 && subType.Length == 0)
                    {
                        continue;
                    }
                    var url = urlFile + type + "/";
                    //get posm Number
                    int posmNumber = 0;
                    try
                    {
                        posmNumber = int.Parse(fc["number_" + type].ToString());
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
                                    fileModel.FileId = fc[type + "_SPVB-fileid"] == null ? "" : fc[type + "_SPVB-fileid"].ToString();
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

            //Other files
            foreach (var item in allMediaType)
            {
                if (!Directory.Exists(Path.Combine(tempFolderPath, item.Code)))
                {
                    continue;
                }
                DirectoryInfo dir = new DirectoryInfo(Path.Combine(tempFolderPath, item.Code));
                foreach (var f in dir.GetFiles())
                {
                    // Get Mediatype and subtype
                    string type = item.Code;
                    var url = urlFile + type + "/";
                    //get posm Number
                    int posmNumber = 0;
                    try
                    {
                        posmNumber = int.Parse(fc["number_" + type].ToString());
                    }
                    catch { }
                    // Get file info
                    var fileName = Path.GetFileName(f.Name);
                    if (fileName.Contains("new_"))
                    {
                        var path = Path.Combine(rootMedia + url, fileName.Replace("new_", ""));
                        if (!Directory.Exists(rootMedia + url))
                            Directory.CreateDirectory(rootMedia + url);
                        // create model file
                        FileUploadModel fileModel = new FileUploadModel();
                        fileModel.FileName = fileName.Replace("new_", "");
                        fileModel.FilePath = url;
                        fileModel.TypeId = type;
                        fileModel.PosmNumber = posmNumber;
                        modelSubmit.FileUploads.Add(fileModel);
                        System.IO.File.Move(f.FullName, path);
                    }
                }
            }
            _trackDetailRepo.UpdateUnSubmitTrackSession(modelSubmit.TrackId, modelSubmit);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Save excel file
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Excel file</returns>
        [HttpGet]
        [CheckLoginFilter]
        [DeleteFileAttribute]
        //[RoleFilter(ActionName = "ImageManager_ExportExcel")]
        public ActionResult DownloadReport(string FromDate, string ToDate, string Region, string Store, string Employee)
        {
            var account = (Data.Database.user)Session["Account"];
            string userId = account.Id.ToString();
            string tempFolderPath = Server.MapPath("~/temp/" + userId);

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }

            tempFolderPath = Server.MapPath("~/temp");

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
            string folder = _imageManagementRepo.ZipImage(FromDate, ToDate, _region, _store, _employee, userId, tempFolderPath);
            string zipFile = Server.MapPath("~/temp/" + userId + ".zip");
            DirectoryInfo from = new DirectoryInfo(folder);
            using (FileStream zipToOpen = new FileStream(zipFile, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (FileInfo file in from.AllFilesAndFolders().Where(o => o is FileInfo).Cast<FileInfo>())
                    {
                        var relPath = file.FullName.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(file.FullName, relPath);
                    }
                }
            }
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "filename=" + Guid.NewGuid().ToString() + ".zip");

            Response.TransmitFile(zipFile);
            return View("Index");
            // return File(zipFile, "application/zip", Guid.NewGuid().ToString() + ".zip");

        }

        [HttpPost]
        public JsonResult UpdateLocation(string digixId, double lat = 0, double lng = 0)
        {
            try
            {
                return this.Json(_StoreRepo.UpdateLocation(digixId, lat, lng), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult MediaType()
        {
            return this.Json(_mediaTypeRepo.GetAll().Where(x => x.Code != "DEFAULT" && x.Code != "STORE_FAILED" && x.Code != "SELFIE"), JsonRequestBehavior.AllowGet);
        }

        #region " Other image "

        [HttpPost]
        public JsonResult UploadOtherImage(HttpPostedFileBase file, FormCollection fc)
        {
            try
            {

                var account = (Data.Database.user)Session["Account"];
                string userId = account.Id.ToString();
                string folder = fc["folder"].ToString();
                string tempFolderPath = Server.MapPath("~/temp/" + userId + "/" + folder);

                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                if (file != null && file.ContentLength > 0)
                {
                    string name = file.FileName;
                    var fileName = Path.GetFileName(file.FileName);
                    string fguid = Guid.NewGuid().ToString().Substring(0, 8);
                    var newFileName = "new_" + fileName.Replace(Path.GetFileNameWithoutExtension(file.FileName), DateTime.Now.ToString("yyyyMMddHHmmss" + "-") + fguid);
                    file.SaveAs(Path.Combine(tempFolderPath, newFileName));
                }
                return this.Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region " [ Add New ] "

        [CheckLoginFilter]
        [RoleFilter(ActionName = "ImageManager_AddNewTrackSesion")]
        public ActionResult AddNew(string trackId, string employeeId, string masterStoreId)
        {
            try
            {

                var _account = (Data.Database.user)Session["Account"];
                string _userId = _account.Id.ToString();
                //1. Get temporary folder by user id
                string tempFolderPath = Path.Combine(tempMedia, _userId);
                //2. Delete temporary foler if exists
                if (Directory.Exists(tempFolderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(tempFolderPath);
                    foreach (var d in di.GetDirectories())
                    {
                        d.Delete(true);
                    }
                    foreach (var f in di.GetFiles())
                    {
                        f.Delete();
                    }
                }

                AddImageModel model = new AddImageModel();
                model.DateUpdate = DateTime.Now;
                model.EmployeeId = employeeId;
                model.MasterStoreId = masterStoreId;
                model.TrackId = trackId;
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
        public ActionResult AddNew(AddImageModel model, FormCollection fc)
        {
            //Get create date
            try
            {
                var tmp = fc["Date"].ToString().Split(' ');
                var date = tmp[0].Split('/');
                model.DateUpdate = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]), 0, 0, 0);
            }
            catch { }
            //
            string _fileName = "";
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            //
            AddImageModel modelSubmit = new AddImageModel();
            modelSubmit.DateUpdate = model.DateUpdate;
            modelSubmit.CreateDate = DateTime.Now;
            modelSubmit.CreateBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
            modelSubmit.EmployeeId = model.EmployeeId;
            modelSubmit.MasterStoreId = model.MasterStoreId;
            modelSubmit.TrackId = model.TrackId;
            modelSubmit.FileUploads = new List<FileUploadModel>();
            //2. Get Mediatype
            var _mediaTypeList = _mediaTypeRepo.GetAllWithDefault();
            //2. Check temporary foler if exists
            if (Directory.Exists(_tempFolderPath))
            {
                //Url file path
                string _urlFile = String.Format("/{0}/{1}/{2}/{3}/", model.DateUpdate.Year, model.DateUpdate.Month, model.DateUpdate.Day, model.MasterStoreId);
                //
                foreach (var media in _mediaTypeList)
                {
                    //2.1 Check folder if exists, if not, continue
                    if (!Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                    {
                        continue;
                    }
                    switch (media.Code)
                    {
                        case "DEFAULT":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT")))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT"));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/HINH_TONG_QUAT/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "HINH_TONG_QUAT";
                                    modelSubmit.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT"), true);
                            }
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI")))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI"));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/HINH_DIA_CHI/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "HINH_DIA_CHI";
                                    modelSubmit.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI"), true);
                            }
                            break;
                        case "SELFIE":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "";
                                    modelSubmit.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                        case "STORE_FAILED":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "";
                                    modelSubmit.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                        default:
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                int posmNumber = 0;
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get number of POSM
                                foreach (var d in dir.GetDirectories())
                                {
                                    if (d.Name.Contains("number_"))
                                    {
                                        posmNumber = int.Parse(d.Name.Replace("number_", ""));
                                        d.Delete(true);
                                        break;
                                    }
                                }
                                //Get file in folder
                                foreach (var d in dir.GetDirectories())
                                {
                                    DirectoryInfo _df = new DirectoryInfo(d.FullName);
                                    foreach (var f in _df.GetFiles())
                                    {
                                        var url = _urlFile + media.Code + "/" + d.Name + "/";
                                        // Get file info
                                        _fileName = f.Name;
                                        var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                        var path = Path.Combine(rootMedia + url, newFileName);
                                        if (!Directory.Exists(rootMedia + url))
                                            Directory.CreateDirectory(rootMedia + url);
                                        // create model file
                                        FileUploadModel fileModel = new FileUploadModel();
                                        fileModel.FileName = newFileName;
                                        fileModel.FilePath = url;
                                        fileModel.TypeId = media.Code;
                                        fileModel.PosmNumber = posmNumber;
                                        fileModel.SubType = d.Name;
                                        modelSubmit.FileUploads.Add(fileModel);
                                        // Save file
                                        f.MoveTo(path);
                                    }
                                    d.Delete(true);
                                }
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = posmNumber;
                                    fileModel.SubType = "";
                                    modelSubmit.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                    }
                }

            }
            _trackDetailRepo.InsertImageAdmin(modelSubmit);
            return RedirectToAction("Index");
        }

        #endregion

        #region " [ Image ] "

        [HttpPost]
        [CheckLoginFilter]
        public JsonResult UploadImage(HttpPostedFileBase file, FormCollection fc)
        {
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            //1. Get temporary folder by user id
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            //2. Delete temporary foler if exists
            if (!Directory.Exists(_tempFolderPath))
            {
                //3. Create temporary folder by user id
                Directory.CreateDirectory(_tempFolderPath);
            }
            //4. Get form infor
            string _type = fc["type"].ToString();
            string _subType = fc["subType"].ToString();
            string _currentfile = fc["fileName"].ToString();
            string _newFileName = fc["newFileName"].ToString();
            //5. Check file exists
            if (file != null && file.ContentLength > 0)
            {
                //5.1 Create folder based on type
                _tempFolderPath = Path.Combine(_tempFolderPath, _type);
                if (!Directory.Exists(_tempFolderPath))
                {
                    Directory.CreateDirectory(_tempFolderPath);
                }
                //5.2 
                switch (_type)
                {
                    case "DEFAULT":
                        switch (_subType)
                        {
                            case "HINH_TONG_QUAT":
                            case "HINH_DIA_CHI":
                                //5.2.1 Set temporary folder path
                                _tempFolderPath = Path.Combine(_tempFolderPath, _subType);
                                //5.2.2 Check current folder is exists
                                if (Directory.Exists((_tempFolderPath)))
                                {
                                    //5.2.2.1 Remove current file if exists
                                    if (_currentfile.Length > 0)
                                    {
                                        if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                        {
                                            System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(_tempFolderPath);
                                }
                                //5.2.3
                                //file.SaveAs(Path.Combine(_tempFolderPath, _newFileName));
                                using (System.IO.Stream MyStream = file.InputStream)
                                {
                                    byte[] input = new byte[file.ContentLength];
                                    MyStream.Read(input, 0, file.ContentLength);
                                    GenerateThumbnails(1, MyStream, Path.Combine(_tempFolderPath, _newFileName));
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "SELFIE":
                        //5.2.1 Check current folder is exists
                        if (Directory.Exists((_tempFolderPath)))
                        {
                            //5.2.1.1 Remove current file if exists
                            if (_currentfile.Length > 0)
                            {
                                if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                {
                                    System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                }
                            }
                        }
                        else
                        {
                            //5.2.1.2 Create temprary folder
                            Directory.CreateDirectory(_tempFolderPath);
                        }
                        //5.2.2
                        //file.SaveAs(Path.Combine(_tempFolderPath, _newFileName));
                        using (System.IO.Stream MyStream = file.InputStream)
                        {
                            byte[] input = new byte[file.ContentLength];
                            MyStream.Read(input, 0, file.ContentLength);
                            GenerateThumbnails(1, MyStream, Path.Combine(_tempFolderPath, _newFileName));
                        }
                        break;
                    case "STORE_FAILED":
                        //5.2.1 Check current folder is exists
                        if (!Directory.Exists((_tempFolderPath)))
                        {
                            //5.2.1.1 Create temprary folder
                            Directory.CreateDirectory(_tempFolderPath);
                        }
                        //5.2.2
                        //file.SaveAs(Path.Combine(_tempFolderPath, _newFileName));
                        using (System.IO.Stream MyStream = file.InputStream)
                        {
                            byte[] input = new byte[file.ContentLength];
                            MyStream.Read(input, 0, file.ContentLength);
                            GenerateThumbnails(1, MyStream, Path.Combine(_tempFolderPath, _newFileName));
                        }
                        break;
                    default:
                        if (_subType.Length > 0)
                        {
                            //5.2.1 Set temporary folder path
                            _tempFolderPath = Path.Combine(_tempFolderPath, _subType);
                            //5.2.2 Check current folder is exists
                            if (Directory.Exists((_tempFolderPath)))
                            {
                                //5.2.2.1 Remove current file if exists
                                if (_currentfile.Length > 0)
                                {
                                    if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                    {
                                        System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                    }
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(_tempFolderPath);
                            }
                            //5.2.3
                            //file.SaveAs(Path.Combine(_tempFolderPath, _newFileName));
                            using (System.IO.Stream MyStream = file.InputStream)
                            {
                                byte[] input = new byte[file.ContentLength];
                                MyStream.Read(input, 0, file.ContentLength);
                                GenerateThumbnails(1, MyStream, Path.Combine(_tempFolderPath, _newFileName));
                            }
                        }
                        else //Other image
                        {
                            //5.2.1 Check current folder is exists
                            if (!Directory.Exists((_tempFolderPath)))
                            {
                                //5.2.1.1 Create temprary folder
                                Directory.CreateDirectory(_tempFolderPath);
                            }
                            //5.2.2
                            //file.SaveAs(Path.Combine(_tempFolderPath, _newFileName));
                            using (System.IO.Stream MyStream = file.InputStream)
                            {
                                byte[] input = new byte[file.ContentLength];
                                MyStream.Read(input, 0, file.ContentLength);
                                GenerateThumbnails(1, MyStream, Path.Combine(_tempFolderPath, _newFileName));
                            }
                        }
                        break;
                }
            }
            //6.
            return this.Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckLoginFilter]
        public JsonResult DeleteImage(FormCollection fc)
        {
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            //1. Get temporary folder by user id
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            //2. Delete temporary foler if exists
            if (!Directory.Exists(_tempFolderPath))
            {
                return this.Json("", JsonRequestBehavior.AllowGet);
            }
            //4. Get form infor
            string _type = fc["type"].ToString();
            string _subType = fc["subType"].ToString();
            string _currentfile = fc["fileName"].ToString();
            //5. Check file exists
            //5.1 Create folder based on type
            _tempFolderPath = Path.Combine(_tempFolderPath, _type);
            if (!Directory.Exists(_tempFolderPath))
            {
                return this.Json("", JsonRequestBehavior.AllowGet);
            }
            //5.2 
            switch (_type)
            {
                case "DEFAULT":
                    switch (_subType)
                    {
                        case "HINH_TONG_QUAT":
                        case "HINH_DIA_CHI":
                            //5.2.1 Set temporary folder path
                            _tempFolderPath = Path.Combine(_tempFolderPath, _subType);
                            //5.2.2 Check current folder is exists
                            if (Directory.Exists((_tempFolderPath)))
                            {
                                //5.2.2.1 Remove current file if exists
                                if (_currentfile.Length > 0)
                                {
                                    if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                    {
                                        System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                    }
                                }
                            }
                            else
                            {
                                return this.Json("", JsonRequestBehavior.AllowGet);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "SELFIE":
                    //5.2.1 Check current folder is exists
                    if (Directory.Exists((_tempFolderPath)))
                    {
                        //5.2.1.1 Remove current file if exists
                        if (_currentfile.Length > 0)
                        {
                            if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                            {
                                System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                            }
                        }
                    }
                    break;
                case "STORE_FAILED":
                    //5.2.1 Check current folder is exists
                    if (Directory.Exists((_tempFolderPath)))
                    {
                        //5.2.1.1 Remove current file if exists
                        if (_currentfile.Length > 0)
                        {
                            if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                            {
                                System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                            }
                        }
                    }
                    break;
                default:
                    if (_subType.Length > 0)
                    {
                        //5.2.1 Set temporary folder path
                        _tempFolderPath = Path.Combine(_tempFolderPath, _subType);
                        //5.2.2 Check current folder is exists
                        if (Directory.Exists((_tempFolderPath)))
                        {
                            //5.2.2.1 Remove current file if exists
                            if (_currentfile.Length > 0)
                            {
                                if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                {
                                    System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                }
                            }
                        }
                        else
                        {
                            return this.Json("", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else //Other images
                    {
                        //5.2.1 Check current folder is exists
                        if (Directory.Exists((_tempFolderPath)))
                        {
                            //5.2.1.1 Remove current file if exists
                            if (_currentfile.Length > 0)
                            {
                                if (System.IO.File.Exists(Path.Combine(_tempFolderPath, _currentfile)))
                                {
                                    System.IO.File.Delete(Path.Combine(_tempFolderPath, _currentfile));
                                }
                            }
                        }
                    }
                    break;
            }
            //6.
            return this.Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region " [ POSM Admin ] "

        [CheckLoginFilter]
        public ActionResult POSMInstallationReport()
        {
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            //1. Get temporary folder by user id
            string tempFolderPath = Path.Combine(tempMedia, _userId);
            //2. Delete temporary foler if exists
            if (Directory.Exists(tempFolderPath))
            {
                DirectoryInfo di = new DirectoryInfo(tempFolderPath);
                foreach (var d in di.GetDirectories())
                {
                    d.Delete(true);
                }
                foreach (var f in di.GetFiles())
                {
                    f.Delete();
                }
            }

            ViewBag.employee = _employeeRepo.ListEmployeeByUserToShowCombobox(_account.Id);

            return View(new POSMTrackModel());
        }

        [HttpPost]
        [CheckLoginFilter]
        public ActionResult SavePOSM(POSMTrackModel model, FormCollection fc)
        {
            //Get create date
            try
            {
                var tmp = fc["Date"].ToString().Split(' ');
                var date = tmp[0].Split('/');
                var time = tmp[1].Split(':');
                model.Date = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]), int.Parse(time[0]), int.Parse(time[1]), 0);
            }
            catch { }
            //
            string _fileName = "";
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            model.CreateBy = _account.Id.ToString();
            //1. Get temporary folder by user id
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            //2. Get Mediatype
            var _mediaTypeList = _mediaTypeRepo.GetAllWithDefault();
            //2. Check temporary foler if exists
            if (Directory.Exists(_tempFolderPath))
            {
                if (model.Id == null || model.Id.Length == 0)
                {
                    model.Id = Guid.NewGuid().ToString();
                }
                //Url file path
                string _urlFile = String.Format("/{0}/{1}/{2}/{3}/", model.Date.Year, model.Date.Month, model.Date.Day, model.Id);
                //
                foreach (var media in _mediaTypeList)
                {
                    //2.1 Check folder if exists, if not, continue
                    if (!Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                    {
                        continue;
                    }
                    switch (media.Code)
                    {
                        case "DEFAULT":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT")))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT"));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/HINH_TONG_QUAT/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "HINH_TONG_QUAT";
                                    model.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code, "HINH_TONG_QUAT"), true);
                            }
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI")))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI"));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/HINH_DIA_CHI/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "HINH_DIA_CHI";
                                    model.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code, "HINH_DIA_CHI"), true);
                            }
                            break;
                        case "SELFIE":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "";
                                    model.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                    break;
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                        case "STORE_FAILED":
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = 0;
                                    fileModel.SubType = "";
                                    model.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                        default:
                            if (Directory.Exists(Path.Combine(_tempFolderPath, media.Code)))
                            {
                                int posmNumber = 0;
                                //Get directory
                                DirectoryInfo dir = new DirectoryInfo(Path.Combine(_tempFolderPath, media.Code));
                                //Get number of POSM
                                foreach (var d in dir.GetDirectories())
                                {
                                    if (d.Name.Contains("number_"))
                                    {
                                        posmNumber = int.Parse(d.Name.Replace("number_", ""));
                                        d.Delete(true);
                                        break;
                                    }
                                }
                                //Get file in folder
                                foreach (var d in dir.GetDirectories())
                                {
                                    DirectoryInfo _df = new DirectoryInfo(d.FullName);
                                    foreach (var f in _df.GetFiles())
                                    {
                                        var url = _urlFile + media.Code + "/" + d.Name + "/";
                                        // Get file info
                                        _fileName = f.Name;
                                        var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                        var path = Path.Combine(rootMedia + url, newFileName);
                                        if (!Directory.Exists(rootMedia + url))
                                            Directory.CreateDirectory(rootMedia + url);
                                        // create model file
                                        FileUploadModel fileModel = new FileUploadModel();
                                        fileModel.FileName = newFileName;
                                        fileModel.FilePath = url;
                                        fileModel.TypeId = media.Code;
                                        fileModel.PosmNumber = posmNumber;
                                        fileModel.SubType = d.Name;
                                        model.FileUploads.Add(fileModel);
                                        // Save file
                                        f.MoveTo(path);
                                    }
                                    d.Delete(true);
                                }
                                //Get file
                                foreach (var f in dir.GetFiles())
                                {
                                    var url = _urlFile + media.Code + "/";
                                    // Get file info
                                    _fileName = f.Name;
                                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + f.Name;
                                    var path = Path.Combine(rootMedia + url, newFileName);
                                    if (!Directory.Exists(rootMedia + url))
                                        Directory.CreateDirectory(rootMedia + url);
                                    // create model file
                                    FileUploadModel fileModel = new FileUploadModel();
                                    fileModel.FileName = newFileName;
                                    fileModel.FilePath = url;
                                    fileModel.TypeId = media.Code;
                                    fileModel.PosmNumber = posmNumber;
                                    fileModel.SubType = "";
                                    model.FileUploads.Add(fileModel);
                                    // Save file
                                    f.MoveTo(path);
                                }
                                //Remove folder
                                Directory.Delete(Path.Combine(_tempFolderPath, media.Code), true);
                            }
                            break;
                    }
                }
                //Remove temporary folder
                Directory.Delete(_tempFolderPath, true);
            }
            _trackDetailRepo.SavePOSM(model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CheckLoginFilter]
        public JsonResult POSMNumber(FormCollection fc)
        {
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            //1. Get temporary folder by user id
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            if (!Directory.Exists(_tempFolderPath))
            {
                Directory.CreateDirectory(_tempFolderPath);
            }
            //3. Get form infor
            string _type = fc["type"].ToString();
            string _number = fc["number"].ToString();
            //4. Create folder based on type
            _tempFolderPath = Path.Combine(_tempFolderPath, _type);
            if (!Directory.Exists(_tempFolderPath))
            {
                Directory.CreateDirectory(_tempFolderPath);
            }
            //5. Remove current folder represent for number
            DirectoryInfo _df = new DirectoryInfo(_tempFolderPath);
            foreach (var d in _df.GetDirectories())
            {
                if (d.Name.Contains("number_"))
                {
                    d.Delete(true);
                }
            }
            //6. Create folder save number of item
            _tempFolderPath = Path.Combine(_tempFolderPath, "number_" + _number);
            Directory.CreateDirectory(_tempFolderPath);
            //7.
            return this.Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CheckLoginFilter]
        public JsonResult POSMImageByType(string type)
        {
            var _account = (Data.Database.user)Session["Account"];
            string _userId = _account.Id.ToString();
            //1. Get temporary folder by user id
            string _tempFolderPath = Path.Combine(tempMedia, _userId);
            //2. Delete temporary foler if exists
            if (!Directory.Exists(_tempFolderPath))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            //3. Get form infor
            _tempFolderPath = Path.Combine(_tempFolderPath, type);
            if (!Directory.Exists(_tempFolderPath))
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
            List<FileUploadModel> _return = new List<FileUploadModel>();
            //
            DirectoryInfo _dir = new DirectoryInfo(_tempFolderPath);
            //get post number
            foreach (var d in _dir.GetDirectories())
            {
                if (d.Name.Contains("number_"))
                {
                    _return.Add(new FileUploadModel() { TypeId = "number", TypeName = d.Name.Replace("number_", "") });
                }
                else
                {
                    DirectoryInfo _dif = new DirectoryInfo(d.FullName);
                    foreach (var f in _dif.GetFiles())
                    {
                        _return.Add(new FileUploadModel() { TypeId = d.Name, FileName = f.Name, FilePath = "/temp/" + _userId + "/" + type + "/" + d.Name + "/" + f.Name });
                    }
                }
            }
            //Get file
            foreach (var f in _dir.GetFiles())
            {
                _return.Add(new FileUploadModel() { TypeId = "Other_" + f.DirectoryName, FileName = f.Name, FilePath = "/temp/" + _userId + "/" + type + "/" + f.Name });
            }


            return this.Json(_return, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}