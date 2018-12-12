using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Core.Utils;
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        private UsersRepo _userRepo;
        public UserManagementController()
        {
            _userRepo = new UsersRepo();
        }
        [AllowAnonymous]
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
                if (id==null)
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
                if(param.IsEdit)
                {
                    ModelState["PasswordConfirm"].Errors.Clear();
                }
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

    }
}