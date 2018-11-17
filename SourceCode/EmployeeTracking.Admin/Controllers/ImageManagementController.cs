using EmployeeTracking.Core;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class ImageManagementController : Controller
    {
        private string rootMedia = @"" + WebConfigurationManager.AppSettings["rootMedia"];
        private string tempMedia = @"" + WebConfigurationManager.AppSettings["WebServerTempFolder"];

        private ImageManagementRepo _imageManagementRepo;
        private MediaTypeRepo _mediaTypeRepo;
        private StoreRepo _StoreRepo;

        public ImageManagementController()
        {
            _imageManagementRepo = new ImageManagementRepo();
            _mediaTypeRepo = new MediaTypeRepo();
            _StoreRepo = new StoreRepo();
        }


        // GET: ImageManagement
        public ActionResult Index(int? page = 1)
        {
            const int pageSize = 10;
            var pageNumber = page > 0 ? page.Value : 1;

            var model = _imageManagementRepo.GetTrackList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

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
                return PartialView("~/Views/ImageManagement/_AddNew.cshtml", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [HttpPost]
        public ActionResult AddNew_Submit(AddImageModel DocumentModel)
        {
            //var DocumentUpload = DocumentModel.DocumentList;
            //foreach (var Doc in DocumentUpload)
            //{
            //    string strFileUpload = "file_" + Convert.ToString(Doc.DocumentID);
            //    HttpPostedFileBase file = Request.Files;

            //    //if (file != null && file.ContentLength > 0)
            //    //{
            //    //    // if you want to save in folder use this
            //    //    var fileName = Path.GetFileName(file.FileName);
            //    //    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);           
            //    //    file.SaveAs(path); 

            //    //    // if you want to store in Bytes in Database use this
            //    //    byte[] image = new byte[file.ContentLength];
            //    //    file.InputStream.Read(image, 0, image.Length); 

            //    //}
            //}
            return Json(new MessageReturnModel
            {
                IsSuccess = true,
                Message = ""
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        public ActionResult EditTrackSession(string id)
        {
            ViewBag.StoreInfo = _imageManagementRepo.GetStoreInfoByTrackSessionId(id);

            var model = _imageManagementRepo.GetTrackDetailListByTrackSessionId(id);

            return PartialView("_EditTrackSession", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackId</param>
        /// <returns></returns>
        public ActionResult ExpandTrack(string id)
        {
            var model = _imageManagementRepo.GetStoreInfoByTrackId(id);
            ViewBag.TrackSessionList = _imageManagementRepo.GetTrackSessionListByTrackId(id);

            return PartialView("_ExpandTrack", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
        public ActionResult TrackSessionCarousel(string id)
        {
            var model = _imageManagementRepo.GetTrackDetailListByTrackSessionId(id);

            return PartialView("_TrackSessionCarousel", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">TrackSessionId</param>
        /// <returns></returns>
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

                        FileHelper.RemoveFileFromServer(detail.Url + detail.FileName); // remove old file

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
        public ActionResult DeleteTrackSession(string id)
        {
            var rs = _imageManagementRepo.DeleteTrackSession(id);
            return Json(rs);
        }

        public ActionResult ExportExcelTrack()
        {
            var bin = _imageManagementRepo.GetExportTrackList();

            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}