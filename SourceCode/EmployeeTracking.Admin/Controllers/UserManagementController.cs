using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Core.Utils;
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Admin.Filters;

namespace EmployeeTracking.Admin.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        private UsersRepo _userRepo;
        private StoreTypeRepo _storeTypeRepo;
        private ProvinceRepo _provinceRepo;
        private DistrictRepo _districtRepo;
        private WardRepo _wardRepo;
        private StoreRepo _storeRepo;
        public UserManagementController()
        {
            _userRepo = new UsersRepo();
            _storeTypeRepo = new StoreTypeRepo();
            _provinceRepo = new ProvinceRepo();
            _districtRepo = new DistrictRepo();
            _wardRepo = new WardRepo();
            _storeRepo = new StoreRepo();
        }
        [AllowAnonymous]
        [CheckLoginFilter]
        [RoleFilter(ActionName = "UserManagement")]
        public ActionResult Index()
        {
            ViewBag.UserType = _userRepo.getAllUserType();
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Index(CustomDataTableRequestHelper requestData)
        {
            //try
            //{
            #region " [ Declaration ] "

            #endregion

            #region " [ Main processing ] "

            #endregion

            //Call to service
            Dictionary<string, object> _return = _userRepo.List(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<UserViewModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<UserViewModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<UserViewModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetail(long? id)
        {
            UserViewModel obj = new UserViewModel();
            ViewBag.UserType = _userRepo.getAllUserType();
            try
            {
                if (id == null)
                {
                    obj.IsEdit = false;
                }
                else
                {
                    obj = _userRepo.GetById(id);
                    obj.IsEdit = true;
                }
                return PartialView("~/Views/UserManagement/PopupDetail.cshtml", obj);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        public ActionResult GetUserToChangePass(long? id)
        {
            UserViewModel obj = new UserViewModel();
            ViewBag.UserType = _userRepo.getAllUserType();
            try
            {
                obj = _userRepo.GetById(id);
                return PartialView("~/Views/UserManagement/ChangePassword.cshtml", obj);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostDetail(UserViewModel param)
        {
            try
            {
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    //Cập nhật
                    if (param.IsEdit)
                    {
                        result = _userRepo.Update(param);
                    }
                    else //Thêm mới
                    {
                        //param.Password = passwordDefault;
                        result = _userRepo.Insert(param);
                    }
                    return Json(result);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Cập nhật tài khoản không thành công", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeIsActive(long? id)
        {
            try
            {
                if (id != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    //param.Password = passwordDefault;
                    result = _userRepo.changeIsActive(id);
                    return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Thay đổi trạng thái thành công", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteUser(long? id)
        {
            try
            {
                if (id != null)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    //param.Password = passwordDefault;
                    result = _userRepo.Delete(id);
                    return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Thay đổi trạng thái thành công", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangePassword(UserViewModel param)
        {
            try
            {
                ModelState["PasswordConfirm"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    result = _userRepo.ChangePass(param);
                    return Json(result);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Đổi mật khẩu không thành công!", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        public ActionResult GetDecentralizedStore(long id)
        {
            UserStoreViewModel model = new UserStoreViewModel();
            model.UserId = id;
            return PartialView("PopupDecentralizedStore", model);
        }

        [CheckLoginFilter]
        public JsonResult GetStoreTypeSelect()
        {
            var jsonData = _storeTypeRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

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

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StoreSearch(CustomDataTableRequestHelper requestData)
        {

            Dictionary<string, object> _return = _userRepo.GetStore(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<StoreManagerModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<StoreManagerModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<StoreManagerModel>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult StoreDecentralizedList(CustomDataTableRequestHelper requestData)
        {
            Dictionary<string, object> _return = _userRepo.GetStoreByUser(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<UserStoreViewModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<UserStoreViewModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<UserStoreViewModel>(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SetStoreForUser(long UserId, Guid StoreId)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _userRepo.SaveStoreForUser(UserId, StoreId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveStoreForUser(long UserId, Guid StoreId)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                result = _userRepo.RemoveStoreForUser(UserId, StoreId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SetAllStoreForUser(CustomDataTableRequestHelper requestData, long UserId, string Name, string StoreType, string HouseNumber, string StreetName, long? ProvinceId, long? DistrictId, long? WardId, string SearchStoreRegion)
        {
            try
            {
                requestData.Name = Name;
                requestData.StoreType = StoreType;
                requestData.HouseNumber = HouseNumber;
                requestData.StreetNames = StreetName;
                requestData.ProvinceId = ProvinceId;
                requestData.DistrictId = DistrictId;
                requestData.WardId = WardId;
                requestData.SearchStoreRegion = SearchStoreRegion;

                MessageReturnModel result = new MessageReturnModel();
                result = _userRepo.SaveAllStoreForUser(requestData, UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveAllStoreForUser(long UserId)
        {
            try
            {

                MessageReturnModel result = new MessageReturnModel();
                result = _userRepo.RemoveAllStoreForUser(UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        #region " Employee Manager "

        public ActionResult EmployeeManager(long id)
        {
            ViewBag.id = id;
            return PartialView("_PopupEmployeeManager");
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult EmployeeWithoutManaged(CustomDataTableRequestHelper requestData)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();

            if(requestData.UserId == null)
            {
                requestData.UserId = 0;
            }

            Dictionary<string, object> _return = _emplRepo.ListEmployeeWithoutManaged(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<UserEmployeeManagerModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<UserEmployeeManagerModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<UserEmployeeManagerModel>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ListEmployeeManaged(CustomDataTableRequestHelper requestData)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();

            if (requestData.UserId == null)
            {
                requestData.UserId = 0;
            }

            Dictionary<string, object> _return = _emplRepo.ListEmployeeManaged(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<UserEmployeeManagerModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<UserEmployeeManagerModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<UserEmployeeManagerModel>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Chọn tất cả nhân viên là trực thuộc quyền quản lý của user account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetAllEmployeeForUser(long userId)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();
            return this.Json(_emplRepo.SetAllEmployeeForUser(userId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa tất cả nhân viên đang trực thuộc quyền quản lý của user account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveAllEmployeeByUser(long userId)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();
            return this.Json(_emplRepo.RemoveAllEmployeeByUser(userId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetEmployeeForUser(long userId, string employeeId)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();
            return this.Json(_emplRepo.SetEmployeeForUser(userId, employeeId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa 1 nhân viên đang thuộc quyền quản lý của user account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public JsonResult RemoveEmployeeForUser(long userId, string employeeId)
        {
            EmployeeRepo _emplRepo = new EmployeeRepo();
            return this.Json(_emplRepo.RemoveEmployeeForUser(userId, employeeId), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}