using EmployeeTracking.Admin.App_Helper;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Core;
using System.Web.Configuration;
using EmployeeTracking.Admin.Filters;

namespace EmployeeTracking.Admin.Controllers
{
    public class StatisticController : Controller
    {
        private string rootMedia = @"" + WebConfigurationManager.AppSettings["rootMedia"];
        private string tempMedia = @"" + WebConfigurationManager.AppSettings["WebServerTempFolder"];
        private ImageManagementRepo _imageManagementRepo;
        private StatisticRepo _statisticRepo;
        private EmployeeRepo _employeeRepo;
        private MediaTypeRepo _mediaTypeRepo;
        private StoreRepo _StoreRepo;
        public StatisticController()
        {
            _statisticRepo = new StatisticRepo();
            _imageManagementRepo = new ImageManagementRepo();
            _employeeRepo = new EmployeeRepo();
            _StoreRepo = new StoreRepo();
            _mediaTypeRepo = new MediaTypeRepo();
        }
        // GET: Statistic
        [CheckLoginFilter]
        [RoleFilter(ActionName = "Statistic")]
        public ActionResult Index()
        {
            ViewBag.employee = _employeeRepo.GetListToShowOnCombobox();
            ViewBag.region = _StoreRepo.GetListRegionToShowOnCombobox();
            var model = _statisticRepo.getStoreNumber5Days();
            // Xu ly thanh doi tuong de tra ve json

            return View("Index", model);
        }
        [CheckLoginFilter]
        public ActionResult getPopUpDetail(Guid id)
        {
            var model = _statisticRepo.getAllTrackSessionRestore(id);
            if (model.trackSessions.Count > 0)
            {
                ViewBag.trackSessionsStart = model.trackSessions[0].Id;
            }
            return PartialView("PopupDetail", model);
        }
        [CheckLoginFilter]
        public JsonResult ShowFiveDayChart()
        {
            var model = _statisticRepo.getStoreNumber5Days();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [CheckLoginFilter]
        public ActionResult TrackSessionCarousel(string id, string TrackIdForCarousel)
        {
            var StoreInfor = _imageManagementRepo.GetStoreInfoByTrackSessionId(id);
            var model = _statisticRepo.GetTrackDetailListByTrackSessionId(id);
            model.ForEach(f =>
            {
                f.TrackDetailImages.ToList().ForEach(_ =>
                {
                    _imageManagementRepo.WriteTextToImageCustom(StoreInfor.SbvpCode + "_" + _.CreateDate.ToString(), rootMedia, _.Url, _.FileName);
                    _.Url = WebConfigurationManager.AppSettings["rootMediaURl"] + "/WriteText" + _.Url;
                });
            });
            Response.Headers["trackSessionIdForCarousel"] = TrackIdForCarousel;
            ViewBag.TrackIdForCarousel = TrackIdForCarousel;
            return PartialView("_TrackSessionCarousel", model);
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

    }
}