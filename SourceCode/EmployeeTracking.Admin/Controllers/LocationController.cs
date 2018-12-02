using EmployeeTracking.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Admin.Filters;

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
        [CheckLoginFilter]
        public JsonResult GetProvinceSelect() 
        {
            var jsonData = _provinceRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [CheckLoginFilter]
        public JsonResult GetDistrictSelect(long provinceId)
        {

            var jsonData = _districtRepo.GetByProvinceId(provinceId);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [CheckLoginFilter]
        public JsonResult GetWardSelect(long districtId)
        {

            var jsonData = _wardRepo.GetByDistrictId(districtId);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CheckLoginFilter]
        public JsonResult GetAllStoreAllProvince()
        {
            return this.Json(_provinceRepo.CountStoreAllProvince(), JsonRequestBehavior.AllowGet);
        }
    }
}