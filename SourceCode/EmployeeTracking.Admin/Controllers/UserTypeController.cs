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


        public UserTypeController()
        {
            _userTypeRepo = new UserTypeRepo();
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
        public JsonResult Add(UserTypeModel model, FormCollection fc)
        {
            return this.Json("thêm mới thành công", JsonRequestBehavior.AllowGet);
        }

    }
}