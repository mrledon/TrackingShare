using EmployeeTracking.Admin.App_Helper;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Core;

namespace EmployeeTracking.Admin.Controllers
{
    public class StatisticController : Controller
    {
        private StoreRepo _storeRepo;

        public StatisticController()
        {
            _storeRepo = new StoreRepo();
        }
        // GET: Statistic
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult getPopUpDetail()
        {
            return PartialView("PopupDetail");
        }
    }
}