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

namespace EmployeeTracking.Admin.Controllers
{
    public class StatisticController : Controller
    {
        private string rootMedia = @"" + WebConfigurationManager.AppSettings["rootMedia"];
        private string tempMedia = @"" + WebConfigurationManager.AppSettings["WebServerTempFolder"];
        private ImageManagementRepo _imageManagementRepo;
        private StatisticRepo _statisticRepo;
        public StatisticController()
        {
            _statisticRepo = new StatisticRepo();
            _imageManagementRepo = new ImageManagementRepo();
        }
        // GET: Statistic
        public ActionResult Index()
        {
            var model = _statisticRepo.getStoreNumber5Days();
            // Xu ly thanh doi tuong de tra ve json

            return View("Index", model);
        }

        public ActionResult getPopUpDetail(Guid id)
        {
            var model = _statisticRepo.getAllTrackSessionRestore(id);
            if (model.trackSessions.Count > 0)
            {
                ViewBag.trackSessionsStart = model.trackSessions[0].Id;
            }
            return PartialView("PopupDetail", model);
        }

        public JsonResult ShowFiveDayChart()
        {
            var model = _statisticRepo.getStoreNumber5Days();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrackSessionCarousel(string id)
        {
            var StoreInfor = _imageManagementRepo.GetStoreInfoByTrackSessionId(id);
            var model = _statisticRepo.GetTrackDetailListByTrackSessionId(id);
            model.ForEach(f =>
            {
                f.TrackDetailImages.ToList().ForEach(_ =>
                {
                    _imageManagementRepo.WriteTextToImageCustom(StoreInfor.SbvpCode + "_" + StoreInfor.Date, rootMedia, _.Url, _.FileName);
                    _.Url = WebConfigurationManager.AppSettings["rootMediaURl"] + "/WriteText" + _.Url;
                });
            });
            return PartialView("_TrackSessionCarousel", model);
        }
    }
}