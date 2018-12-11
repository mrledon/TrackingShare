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
    [AllowAnonymous]
    public class UserTypeController : BasicController
    {

        private UserTypeRepo _userTypeRepo;
        private RoleRepo _roleRepo;


        public UserTypeController()
        {
            _userTypeRepo = new UserTypeRepo();
            _roleRepo = new RoleRepo();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
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
            
            // Process sorting column
            //requestData = requestData.SetOrderingColumnName();

            #endregion

            //Call to service
            Dictionary<string, object> _return = _userTypeRepo.List(requestData);
            //
            if ((ResponseStatusCodeHelper)_return[DatatableCommonSetting.Response.STATUS] == ResponseStatusCodeHelper.OK)
            {
                DataTableResponse<UserTypeModel> itemResponse = _return[DatatableCommonSetting.Response.DATA] as DataTableResponse<UserTypeModel>;
                return this.Json(itemResponse, JsonRequestBehavior.AllowGet);
            }
            //
            return this.Json(new DataTableResponse<ImageManagementViewModel>(), JsonRequestBehavior.AllowGet);
            //}
            //catch (ServiceException serviceEx)
            //{
            //    throw serviceEx;
            //}
            //catch (DataAccessException accessEx)
            //{
            //    throw accessEx;
            //}
            //catch (Exception ex)
            //{
            //    throw new ControllerException(FILE_NAME, MethodInfo.GetCurrentMethod().Name, UserID, ex);
            //}
        }


        public ActionResult View(int id)
        {
            try
            {
                var model = _userTypeRepo.GetByIdToView(id);
                return PartialView("~/Views/UserType/_View.cshtml", model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                return PartialView("~/Views/UserType/_Form.cshtml", _userTypeRepo.GetById(0));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [HttpPost]
        public JsonResult Save(UserTypeModel model, FormCollection fc)
        {
            try
            {
                var role = _roleRepo.List();

                foreach (var item in role)
                {
                    if (!string.IsNullOrEmpty(fc[item.Code]))
                    {
                        string selected = fc[item.Code].ToString().ToLower();
                        if (selected == "on" || selected == "true")
                        {
                            UserTypeDetailModel dt = new UserTypeDetailModel();
                            dt.RoleCode = item.Code;
                            dt.Selected = true;
                            model.details.Add(dt);
                        }
                    }
                }
                
                return this.Json(_userTypeRepo.Save(model), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return this.Json("", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                return PartialView("~/Views/UserType/_Form.cshtml", _userTypeRepo.GetById(id));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var rs = _userTypeRepo.Delete(id);
            return Json(rs);
        }
    }
}