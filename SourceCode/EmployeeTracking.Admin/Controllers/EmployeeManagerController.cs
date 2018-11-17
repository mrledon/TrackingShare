using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracking.Admin.Controllers
{
    public class EmployeeManagerController : BasicController
    {
        private EmployeeRepo _employeeRepo;
        private string passwordDefault = ConfigurationManager.AppSettings["PasswordDefault"];

        public EmployeeManagerController()
        {
            _employeeRepo = new EmployeeRepo();
        }

        // GET: EmployeeManager
        public ActionResult Index(int? page, string code, string name, bool? gender, DateTime? birthday, string identityCard, string phone, string owner)
        {
            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            EmployeeManagerFilterModel filter = new EmployeeManagerFilterModel()
            {
                Code = code,
                Birthday = birthday,
                Gender = gender,
                IdentityCard = identityCard,
                Name = name,
                Owner = owner,
                Phone = phone
            };
            ViewBag.Code = code;
            ViewBag.Birthday = birthday.HasValue ? birthday.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.Gender = gender;
            ViewBag.IdentityCard = identityCard;
            ViewBag.Name = name;
            ViewBag.Owner = owner;
            ViewBag.Phone = phone;
            var data = _employeeRepo.GetAllEmployee(filter);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetDetail(string id)
        {
            EmployeeManagerModel obj = new EmployeeManagerModel();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    obj.IsEdit = false;
                }
                else
                {
                    obj = _employeeRepo.GetById(id);
                    obj.IsEdit = true;
                }
                return PartialView("~/Views/EmployeeManager/PopupDetail.cshtml", obj);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("~/Views/Shared/ErrorPartial.cshtml");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PostDetail(EmployeeManagerModel param)
        {
            try
            {
                if (param != null && ModelState.IsValid)
                {
                    MessageReturnModel result = new MessageReturnModel();
                    //Cập nhật
                    if (param.IsEdit)
                    {
                        param.ModifiedBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        param.ModifiedDate = DateTime.Now;
                        result = _employeeRepo.Update(param);
                    }
                    else //Thêm mới
                    {
                        param.CreatedBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        param.CreatedDate = DateTime.Now;
                        param.Password = passwordDefault;
                        result = _employeeRepo.Insert(param);
                    }
                    return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Thiếu hoặc sai thông tin nhân viên", Data = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteModel(string id)
        {
            try
            {
                MessageReturnModel result = _employeeRepo.Delete(id);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ResetPassword(string id)
        {
            try
            {
                MessageReturnModel result = _employeeRepo.ResetPassword(id, passwordDefault);
                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = result.Id });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }
    }
}