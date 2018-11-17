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
using System.Web.Hosting;
using System.Web.Mvc;

namespace EmployeeTracking.Controllers
{
    public class ImageManagementController : Controller
    {
        private ImageManagementRepo _imageManagementRepo;
        private MediaTypeRepo _mediaTypeRepo;

        public ImageManagementController()
        {
            _imageManagementRepo = new ImageManagementRepo();
            _mediaTypeRepo = new MediaTypeRepo();
        }


        // GET: ImageManagement
        public ActionResult Index(int? page = 1)
        {
            const int pageSize = 10;
            var pageNumber = page > 0 ? page.Value : 1;

            var model = _imageManagementRepo.GetTrackList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AddNew()
        {
            try
            {
                ViewBag.ListMediaType = _mediaTypeRepo.GetAll();
                return PartialView("~/Views/ImageManagement/_AddNew.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
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
                                var path = Server.MapPath("~" + Path.Combine(image.Url, image.FileName));
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
                    if (FileHelper.IsValidImage(file))
                    {
                        var fileName = Guid.NewGuid().ToString("n") + Path.GetExtension(file.FileName);

                        // TODO: sửa lại link lưu thêm id trước media type
                        var directoryGen = $"/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}/" + detail.MediaTypeId + "/";
                        var directory = Server.MapPath("~" + directoryGen);

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