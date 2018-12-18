using EmployeeTracking.Admin.App_Helper;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.ModelCustom;
using ExcelDataReader;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Core;
using System.Web.Script.Serialization;
using EmployeeTracking.Admin.Filters;

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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager")]
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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager_AddNew")]
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
        [CheckLoginFilter]
        public JsonResult GetStoreTypeSelect()
        {
            var jsonData = _storeTypeRepo.GetAll();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager_AddNew")]
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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager_Delete")]
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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager_ExportExcel")]
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
        [CheckLoginFilter]
        [RoleFilter(ActionName = "StoreManager_ImportExcel")]
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
                    //DataTable dt = new DataTable();
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
                    List<StoreManagerModel> listStore = new List<StoreManagerModel>();
                    try
                    {
                        string createBy = (HttpContext.Session["Account"] as EmployeeTracking.Data.Database.user).UserName;
                        DateTime createDate = DateTime.Now;
                        listStore = dt.AsEnumerable()
                            .Select(row => new StoreManagerModel
                            {
                                Id = new Guid(row[0].ConvertToObject<string>()),
                                Code = row[1].ConvertToObject<string>(),
                                Name = row[2].ConvertToObject<string>(),
                                StoreTypeName = row[3].ConvertToObject<string>(),
                                ProvinceName = row[4].ConvertToObject<string>(),
                                DistrictName = row[5].ConvertToObject<string>(),
                                WardName = row[6].ConvertToObject<string>(),
                                StreetNames = row[7].ConvertToObject<string>(),
                                HouseNumber = row[8].ConvertToObject<string>(),
                                Region = row[9].ConvertToObject<string>(),
                                CreatedBy = createBy,
                                ModifiedBy = createBy,
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

        /// <summary>
        /// Get list of store by location id    
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="districtID"></param>
        /// <param name="wardID"></param>
        /// <returns></returns>
        [CheckLoginFilter]
        public JsonResult GetByProvince(long provinceId, long districtID, long wardID, bool getAddress)
        {
            var jsonResult = Json(_storeRepo.GetListAddresStoreByLocation(provinceId, districtID, wardID, getAddress), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

    }
}