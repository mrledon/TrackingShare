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
    public class EmployeeManagerController : BasicController
    {
        private EmployeeRepo _employeeRepo;
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
            ViewBag.Birthday = birthday;
            ViewBag.Gender = gender;
            ViewBag.IdentityCard = identityCard;
            ViewBag.Name = name;
            ViewBag.Owner = owner;
            ViewBag.Phone = phone;
            var data = _employeeRepo.GetAllEmployee(filter);
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        //public ActionResult GetDetail(Guid? id)
        //{
        //    MODELLoaiSanPham obj = new MODELLoaiSanPham();
        //    try
        //    {
        //        //Tải thông tin đối tượng.
        //        if (id != 0)
        //        {
        //            obj = context.LOAISANPHAMs.Where(x => x.ID == id && x.IsDeleted == false).Select(x => new MODELLoaiSanPham
        //            {
        //                ID = x.ID,
        //                TenGoi = x.TenGoi
        //            }).FirstOrDefault();
        //            obj.IsEdit = true;//mark edit
        //        }
        //        else
        //        {
        //            obj.IsEdit = false;
        //        }
        //        return PartialView("~/Views/LoaiSanPham/PopupDetail.cshtml", obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = "Lỗi tải thông tin";
        //        //SysLog             
        //        LogError(string.Format("{0} / {1}", HttpContext.User.Identity.Name, message), ex);
        //        ViewBag.ErrorMessage = ex.Message;
        //        return PartialView("~/Views/Shared/ErrorPartial.cshtml");
        //    }
        //}
    }
}