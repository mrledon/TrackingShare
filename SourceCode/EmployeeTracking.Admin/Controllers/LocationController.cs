using EmployeeTracking.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class LocationController : Controller
    {
        private ProvinceRepo _provinceRepo;
        private DistrictRepo _districtRepo;
        private WardRepo _wardRepo;

        public LocationController()
        {
            _provinceRepo = new ProvinceRepo();
            _districtRepo = new DistrictRepo();
            _wardRepo = new WardRepo();
        }

        // GET: Location
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public JsonResult GetProvinceSelect() 
        {
            var jsonData = _provinceRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrictSelect(long provinceId)
        {

            var jsonData = _districtRepo.GetByProvinceId(provinceId);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWardSelect(long districtId)
        {

            var jsonData = _wardRepo.GetByDistrictId(districtId);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}