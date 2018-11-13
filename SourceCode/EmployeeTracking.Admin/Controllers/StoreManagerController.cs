using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class StoreManagerController : BasicController
    {
        private StoreRepo _storeRepo;
        private StoreTypeRepo _storeTypeRepo;

        public StoreManagerController()
        {
            _storeRepo = new StoreRepo();
            _storeTypeRepo = new StoreTypeRepo();
        }

        // GET: StoreManager
        public ActionResult Index(int? page, string code, string name, string storeType, string houseNumber, string streetName, long? provinceId, long? districtId, long? wardId, string region)
        {
            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            StoreManagerFilterModel filter = new StoreManagerFilterModel()
            {
                Code = code,
                Name = name,
                StoreType = storeType,
                HouseNumber = houseNumber,
                StreetNames = streetName,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                Region = region
            };
            ViewBag.Code = code;
            ViewBag.Name = name;
            ViewBag.StoreType = storeType;
            ViewBag.HouseNumber = houseNumber;
            ViewBag.StreetNames = streetName;
            ViewBag.ProvinceId = provinceId;
            ViewBag.DistrictId = districtId;
            ViewBag.WardId = wardId;
            ViewBag.Region = region;
            var data = _storeRepo.GetAllEmployee(filter);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetDetail(string id)
        {
            StoreManagerModel obj = new StoreManagerModel();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    obj.IsEdit = false;
                }
                else
                {
                    obj = _storeRepo.GetById(Guid.Parse(id));
                    obj.IsEdit = true;
                }
                return PartialView("~/Views/StoreManager/PopupDetail.cshtml", obj);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        public JsonResult GetStoreTypeSelect()
        {
            var jsonData = _storeTypeRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostDetail(StoreManagerModel param)
        {
            try
            {
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    //Cập nhật
                    if (param.IsEdit)
                    {
                        param.ModifiedBy = "1";
                        param.ModifiedDate = DateTime.Now;
                        result = _storeRepo.Update(param);
                    }
                    else //Thêm mới
                    {
                        param.CreatedBy = "1";
                        param.CreatedDate = DateTime.Now;
                        result = _storeRepo.Insert(param);
                    }
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteModel(Guid id)
        {
            try
            {
                MessageReturnModel result = _storeRepo.Delete(id);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }
    }
}