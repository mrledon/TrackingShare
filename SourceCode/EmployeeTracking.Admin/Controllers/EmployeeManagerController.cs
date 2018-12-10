using EmployeeTracking.Admin.App_Helper;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Core;
using EmployeeTracking.Admin.Filters;

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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "EmployeeManager")]
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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "EmployeeManager_AddNew")]
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

        [RoleFilter(ActionName = "EmployeeManager_AddNew")]
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
        [RoleFilter(ActionName = "EmployeeManager_Delete")]
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
        [RoleFilter(ActionName = "EmployeeManager_ResetPassword")]
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

        [RoleFilter(ActionName = "EmployeeManager_ExportExcel")]
        public ActionResult ExportExcel(string code, string name, bool? gender, DateTime? birthday, string identityCard, string phone, string owner)
        {
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
            var bin = _employeeRepo.GetExportTrackList(filter);

            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [RoleFilter(ActionName = "EmployeeManager_ImportExcel")]
        [CheckLoginFilter]
        public ActionResult ImportExcel()
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileData = Request.Files[file];
                    string connString = "";
                    DataTable dt = new DataTable();
                    string extension = System.IO.Path.GetExtension(Request.Files["IMPORTEXCEL"].FileName).ToLower();
                    string path = string.Format("{0}/{1}", Server.MapPath("~/FileImportTemp"), Request.Files["IMPORTEXCEL"].FileName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/FileImportTemp"));
                    }
                    if (System.IO.File.Exists(path))
                    { System.IO.File.Delete(path); }
                    Request.Files["IMPORTEXCEL"].SaveAs(path);
                    using (FileStream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        IExcelDataReader excelReader;
                        switch (Path.GetExtension(path).ToLower())
                        {
                            case ".csv":
                                excelReader = ExcelReaderFactory.CreateCsvReader(stream);
                                break;
                            case ".xls":
                                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                                break;
                            case ".xlsx":
                                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                break;
                            default:
                                throw new Exception("ImportExcel() - phần mở rộng tệp không xác định / không được hỗ trợ");
                        }
                        dt = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        }).Tables[0];
                    }
                    //if (extension == ".csv")
                    //{
                    //    dt = Utility.ConvertCSVtoDataTable(path);
                    //}
                    ////Connection String to Excel Workbook  
                    //else if (extension.Trim() == ".xls")
                    //{
                    //    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    //    dt = Utility.ConvertXSLXtoDataTable(path, connString);
                    //}
                    //else if (extension.Trim() == ".xlsx")
                    //{
                    //    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //    dt = Utility.ConvertXSLXtoDataTable(path, connString);
                    //}
                    System.IO.File.Delete(path);
                    List<EmployeeManagerModel> listEmloyee = new List<EmployeeManagerModel>();
                    try
                    {
                        string createBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        DateTime createDate = DateTime.Now;
                        listEmloyee = dt.AsEnumerable()
                            .Select(row => new EmployeeManagerModel
                            {
                                Id = row[0].ConvertToObject<string>(),
                                Code = row[1].ConvertToObject<string>(),
                                Name = row[2].ConvertToObject<string>(),
                                Gender = GetGender(row[3].ConvertToObject<string>()),
                                Birthday = row[4].ConvertToDateTime_Null(),
                                IdentityCard = row[5].ConvertToObject<string>(),
                                Phone = row[6].ConvertToObject<string>(),
                                Owner = row[7].ConvertToObject<string>(),
                                Password = passwordDefault,
                                CreatedBy = createBy,
                                CreatedDate = createDate
                            }).ToList();
                    }
                    catch(Exception ex)
                    {
                        return Json(new { IsSuccess = false, Message = "File import không đúng định dạng", Data = "" });
                    }
                    result = _employeeRepo.ImportExcel(listEmloyee);
                }

                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
            }
            catch(Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }

        private bool? GetGender(string value)
        {
            bool? result = null;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Trim().ToLower().Contains("nam"))
                    result = true;
                else
                    result = false;
            }
            return result;
        }

        public JsonResult GetEmployeeSelect()
        {
            var jsonData = _employeeRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}