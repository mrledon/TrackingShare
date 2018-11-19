using EmployeeTracking.Admin.App_Helper;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        public ActionResult Index(int? page, string code, string name, string ddlStoreType, string houseNumber, string streetName, long? ddlProvinceId, long? ddlDistrictId, long? ddlWardId, string region)
        {
            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            StoreManagerFilterModel filter = new StoreManagerFilterModel()
            {
                Code = code,
                Name = name,
                StoreType = ddlStoreType,
                HouseNumber = houseNumber,
                StreetNames = streetName,
                ProvinceId = ddlProvinceId,
                DistrictId = ddlDistrictId,
                WardId = ddlWardId,
                Region = region
            };
            ViewBag.Code = code;
            ViewBag.Name = name;
            ViewBag.StoreType = ddlStoreType;
            ViewBag.HouseNumber = houseNumber;
            ViewBag.StreetNames = streetName;
            ViewBag.ProvinceId = ddlProvinceId;
            ViewBag.DistrictId = ddlDistrictId;
            ViewBag.WardId = ddlWardId;
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
                        param.ModifiedBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        param.ModifiedDate = DateTime.Now;
                        result = _storeRepo.Update(param);
                    }
                    else //Thêm mới
                    {
                        param.CreatedBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
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

        public ActionResult ExportExcel(string code, string name, string ddlStoreType, string houseNumber, string streetName, long? ddlProvinceId, long? ddlDistrictId, long? ddlWardId, string region)
        {
            StoreManagerFilterModel filter = new StoreManagerFilterModel()
            {
                Code = code,
                Name = name,
                StoreType = ddlStoreType,
                HouseNumber = houseNumber,
                StreetNames = streetName,
                ProvinceId = ddlProvinceId,
                DistrictId = ddlDistrictId,
                WardId = ddlWardId,
                Region = region
            };
            var bin = _storeRepo.GetExportTrackList(filter);

            string fileName = Guid.NewGuid().ToString() + ".xlsx";
            return File(bin, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

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
                    if (extension == ".csv")
                    {
                        dt = Utility.ConvertCSVtoDataTable(path);
                    }
                    //Connection String to Excel Workbook  
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        dt = Utility.ConvertXSLXtoDataTable(path, connString);
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        dt = Utility.ConvertXSLXtoDataTable(path, connString);
                    }
                    System.IO.File.Delete(path);
                    List<StoreManagerModel> listStore = new List<StoreManagerModel>();
                    try
                    {
                        string createBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        DateTime createDate = DateTime.Now;
                        listStore = dt.AsEnumerable()
                            .Select(row => new StoreManagerModel
                            {
                                Code = row.Field<string>(0),
                                Name = row.Field<string>(1),
                                StoreTypeName = row.Field<string>(2),
                                ProvinceName = row.Field<string>(3),
                                DistrictName = row.Field<string>(4),
                                WardName = row.Field<string>(5),
                                StreetNames = row.Field<string>(6),
                                HouseNumber = row.Field<string>(7),
                                Region = row.Field<string>(8),
                                CreatedBy = createBy,
                                CreatedDate = createDate
                            }).ToList();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { IsSuccess = false, Message = "File import không đúng định dạng", Data = "" });
                    }
                    result = _storeRepo.ImportExcel(listStore);
                }

                return Json(new { IsSuccess = result.IsSuccess, Message = result.Message, Data = "" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message, Data = "" });
            }
        }
    }
}