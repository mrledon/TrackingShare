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
        private StatisticRepo _statisticRepo;
        public StatisticController()
        {
            _statisticRepo = new StatisticRepo();
        }
        // GET: Statistic
        public ActionResult Index()
        {
            var model = _statisticRepo.getStoreNumber5Days();
            // Xu ly thanh doi tuong de tra ve json
            
            return View("Index",model);
        }

        public ActionResult getPopUpDetail(Guid id)
        {
             var model = _statisticRepo.getAllTrackSessionRestore(id);
            return PartialView("PopupDetail", model);
        }
        
        public JsonResult ShowFiveDayChart()
        {
            var model = _statisticRepo.getStoreNumber5Days();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrackSessionCarousel(string id)
        {
            var model = _statisticRepo.GetTrackDetailListByTrackSessionId(id);
            model.ForEach(f =>
            {
                f.TrackDetailImages.ToList().ForEach(_ =>
                {
                    _.Url = WebConfigurationManager.AppSettings["rootURl"] + _.Url;
                });
            });
            return PartialView("_TrackSessionCarousel", model);
        }
    }
}