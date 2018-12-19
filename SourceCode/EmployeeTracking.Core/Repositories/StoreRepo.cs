using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Threading;

namespace EmployeeTracking.Core.Repositories
{
    public class StoreRepo
    {
        public List<StoreManagerModel> GetAllEmployee(StoreManagerFilterModel filter)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    //List<StoreManagerModel> result = new List<StoreManagerModel>();
                    //var data = _data.master_store.Where(x => (string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                    //(string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    //(string.IsNullOrEmpty(filter.StoreType) || x.StoreType.Contains(filter.StoreType)) &&
                    //(string.IsNullOrEmpty(filter.HouseNumber) || x.HouseNumber.Contains(filter.HouseNumber)) &&
                    //(string.IsNullOrEmpty(filter.StreetNames) || x.StreetNames.Contains(filter.StreetNames)) &&
                    //(!filter.ProvinceId.HasValue || x.ProvinceId == filter.ProvinceId) &&
                    //(!filter.DistrictId.HasValue || x.DistrictId == filter.DistrictId) &&
                    //(!filter.WardId.HasValue || x.WardId == filter.WardId) &&
                    //(string.IsNullOrEmpty(filter.Region) || x.Region.Contains(filter.Region))).OrderBy(f => f.Name).ToList();
                    //int index = 1;
                    //foreach (var item in data)
                    //{
                    //    StoreManagerModel employee = new StoreManagerModel();
                    //    employee.Index = index; index++;
                    //    employee.Id = item.Id;
                    //    employee.Code = item.Code;
                    //    employee.CreatedBy = item.CreatedBy;
                    //    employee.CreatedDate = item.CreatedDate;
                    //    employee.DistrictId = item.DistrictId;
                    //    employee.HouseNumber = item.HouseNumber;
                    //    employee.Name = item.Name;
                    //    employee.ProvinceId = item.ProvinceId;
                    //    employee.Region = item.Region;
                    //    employee.StoreType = item.StoreType;
                    //    employee.StreetNames = item.StreetNames;
                    //    employee.WardId = item.WardId;
                    //    result.Add(employee);
                    //}
                    var query = (from ms in _data.master_store
                                 join mst in _data.master_store_type
                                      on ms.StoreType equals mst.Id into temp1
                                 from ms_mst in temp1.DefaultIfEmpty()
                                 join p in _data.provinces
                                      on ms.ProvinceId equals p.Id into temp2
                                 from ms_p in temp2.DefaultIfEmpty()
                                 join d in _data.districts
                                      on ms.DistrictId equals d.Id into temp3
                                 from ms_d in temp3.DefaultIfEmpty()
                                 join w in _data.wards
                                      on ms.WardId equals w.Id into temp4
                                 from ms_w in temp4.DefaultIfEmpty()
                                 where (string.IsNullOrEmpty(filter.Code) || ms.Code.Contains(filter.Code)) &&
                                 (string.IsNullOrEmpty(filter.Name) || ms.Name.Contains(filter.Name)) &&
                                 (string.IsNullOrEmpty(filter.StoreType) || ms.StoreType.Contains(filter.StoreType)) &&
                                 (string.IsNullOrEmpty(filter.HouseNumber) || ms.HouseNumber.Contains(filter.HouseNumber)) &&
                                 (string.IsNullOrEmpty(filter.StreetNames) || ms.StreetNames.Contains(filter.StreetNames)) &&
                                 (!filter.ProvinceId.HasValue || ms.ProvinceId == filter.ProvinceId) &&
                                 (!filter.DistrictId.HasValue || ms.DistrictId == filter.DistrictId) &&
                                 (!filter.WardId.HasValue || ms.WardId == filter.WardId) &&
                                 (string.IsNullOrEmpty(filter.Region) || ms.Region.Contains(filter.Region))
                                 select new StoreManagerModel
                                 {
                                     Id = ms.Id,
                                     Code = ms.Code,
                                     Name = ms.Name,
                                     DistrictName = ms_d.Name,
                                     ProvinceName = ms_p.Name,
                                     StoreTypeName = ms_mst.Name,
                                     WardName = ms_w.Name,
                                     HouseNumber = ms.HouseNumber,
                                     Region = ms.Region,
                                     StreetNames = ms.StreetNames
                                 }).ToList();

                    int index = 1;
                    foreach (var item in query)
                    {
                        item.Index = index;
                        index++;
                    }
                    return query;
                }
            }
            catch (Exception ex)
            {
                return new List<StoreManagerModel>();
            }
        }

        public MessageReturnModel Insert(StoreManagerModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.master_store.Where(x => x.Code == model.Code).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã cửa hàng " + model.Code + " đã tồn tại"
                        };
                    }
                    master_store insertModel = new master_store
                    {
                        Id = Guid.NewGuid(),
                        Code = model.Code,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate.Value,
                        DistrictId = model.DistrictId,
                        HouseNumber = model.HouseNumber,
                        Name = model.Name,
                        ProvinceId = model.ProvinceId,
                        Region = model.Region,
                        StoreType = model.StoreType,
                        StreetNames = model.StreetNames,
                        WardId = model.WardId,
                        PhoneNumber = model.PhoneNumber,
                        LAT = model.LAT,
                        LNG = model.LNG
                    };
                    _data.master_store.Add(insertModel);
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = insertModel.Id.ToString(),
                        Message = "Thêm mới cửa hàng thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel Update(StoreManagerModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store updateModel = _data.master_store.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.ModifiedBy = model.ModifiedBy;
                        updateModel.ModifiedDate = model.ModifiedDate;
                        updateModel.Code = model.Code;
                        updateModel.DistrictId = model.DistrictId;
                        updateModel.HouseNumber = model.HouseNumber;
                        updateModel.Name = model.Name;
                        updateModel.ProvinceId = model.ProvinceId;
                        updateModel.Region = model.Region;
                        updateModel.StoreType = model.StoreType;
                        updateModel.StreetNames = model.StreetNames;
                        updateModel.WardId = model.WardId;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString(),
                            Message = "Cập nhật cửa hàng thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm thấy cửa hàng"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel UpdateLocation(string id, double LAT, double LNG)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var store = _db.master_store.FirstOrDefault(m => m.Id == new Guid(id));
                if (store != null)
                {
                    store.LAT = LAT;
                    store.LNG = LNG;
                    _db.master_store.Attach(store);
                    _db.Entry(store).State = System.Data.EntityState.Modified;
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy cửa hàng!"
                    };
                }
            }
        }

        //Coordinates
        public MessageReturnModel UpdateCoordinates(Guid id, double lat, double lng)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {

                    master_store updateModel = _data.master_store.Where(x => x.Id == id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.LAT = lat;
                        updateModel.LNG = lng;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString(),
                            Message = "Cập nhật tọa độ cửa hàng thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm cửa hàng cập nhật tọa độ."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel Delete(Guid id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store updateModel = _data.master_store.Where(x => x.Id == id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        _data.master_store.Remove(updateModel);
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString()
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm thấy cửa hàng"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public StoreManagerModel GetById(Guid id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store model = _data.master_store.Where(x => x.Id == id).FirstOrDefault();
                    if (model != null)
                    {
                        return new StoreManagerModel
                        {
                            Id = model.Id,
                            Code = model.Code,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = model.CreatedDate,
                            DistrictId = model.DistrictId,
                            HouseNumber = model.HouseNumber,
                            Name = model.Name,
                            ProvinceId = model.ProvinceId,
                            Region = model.Region,
                            StoreType = model.StoreType,
                            StreetNames = model.StreetNames,
                            WardId = model.WardId
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Byte[] GetExportTrackList(StoreManagerFilterModel filter)
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var query = (from ms in _data.master_store
                             join mst in _data.master_store_type
                                  on ms.StoreType equals mst.Id into temp1
                             from ms_mst in temp1.DefaultIfEmpty()
                             join p in _data.provinces
                                  on ms.ProvinceId equals p.Id into temp2
                             from ms_p in temp2.DefaultIfEmpty()
                             join d in _data.districts
                                  on ms.DistrictId equals d.Id into temp3
                             from ms_d in temp3.DefaultIfEmpty()
                             join w in _data.wards
                                  on ms.WardId equals w.Id into temp4
                             from ms_w in temp4.DefaultIfEmpty()
                             where (string.IsNullOrEmpty(filter.Code) || ms.Code.Contains(filter.Code)) &&
                             (string.IsNullOrEmpty(filter.Name) || ms.Name.Contains(filter.Name)) &&
                             (string.IsNullOrEmpty(filter.StoreType) || ms.StoreType.Contains(filter.StoreType)) &&
                             (string.IsNullOrEmpty(filter.HouseNumber) || ms.HouseNumber.Contains(filter.HouseNumber)) &&
                             (string.IsNullOrEmpty(filter.StreetNames) || ms.StreetNames.Contains(filter.StreetNames)) &&
                             (!filter.ProvinceId.HasValue || ms.ProvinceId == filter.ProvinceId) &&
                             (!filter.DistrictId.HasValue || ms.DistrictId == filter.DistrictId) &&
                             (!filter.WardId.HasValue || ms.WardId == filter.WardId) &&
                             (string.IsNullOrEmpty(filter.Region) || ms.Region.Contains(filter.Region))
                             select new StoreManagerModel
                             {
                                 Id = ms.Id,
                                 Code = ms.Code,
                                 Name = ms.Name,
                                 DistrictName = ms_d.Name,
                                 ProvinceName = ms_p.Name,
                                 StoreTypeName = ms_mst.Name,
                                 WardName = ms_w.Name,
                                 HouseNumber = ms.HouseNumber,
                                 Region = ms.Region,
                                 StreetNames = ms.StreetNames
                             }).ToList();

                int index = 1;
                foreach (var item in query)
                {
                    item.Index = index;
                    index++;
                }

                /////////////////////////////////////////////////
                // Write data to excel

                using (ExcelPackage p = new ExcelPackage())
                {
                    //Here setting some document properties
                    p.Workbook.Properties.Author = "Công ty";
                    p.Workbook.Properties.Title = "Báo cáo";

                    //Create a sheet
                    p.Workbook.Worksheets.Add("Báo cáo");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Báo cáo"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Times New Roman"; //Default Font name for whole sheet

                    // Create header column
                    string[] arrColumnHeader = {
                                                "Id Cửa hàng",
                                                "Mã cửa hàng",
                                                "Tên cửa hàng",
                                                "Loại hình cửa hàng",
                                                "Tỉnh/Thành phố",
                                                "Quận/Huyện",
                                                "Phường/Xã",
                                                "Đường",
                                                "Số nhà",
                                                "Khu vực",
                    };
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 1;
                    ws.Column(1).Hidden = true;
                    //Creating Headings
                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Font.Bold = true;
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        var cell = ws.Cells[rowIndex, colIndex, rowIndex, colIndex];

                        //Setting the background color of header cells to Gray
                        var fill3 = cell.Style.Fill;
                        fill3.PatternType = ExcelFillStyle.Solid;
                        fill3.BackgroundColor.SetColor(1, 0, 176, 240);

                        //Setting Top/left,right/bottom borders.
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        //Setting Value in cell
                        cell.Value = item;

                        colIndex++;
                    }

                    // Adding Data into rows
                    foreach (var item in query)
                    {
                        colIndex = 1;
                        rowIndex++;
                        //Setting Value in cell
                        ws.Cells[rowIndex, colIndex].Value = item.Id;
                        //Setting Top/left,right/bottom borders.
                        var border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Code;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Name;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.StoreTypeName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.ProvinceName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DistrictName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.WardName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.StreetNames;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.HouseNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Region;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    //Generate A File with name
                    Byte[] bin = p.GetAsByteArray();

                    return bin;
                }
            }
        }

        public MessageReturnModel ImportExcel_old(List<StoreManagerModel> listStore)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        StoreManagerModel temp = new StoreManagerModel();
                        try
                        {
                            foreach (var model in listStore)
                            {
                                temp = model;
                                int count = _data.employees.Where(x => x.Code.Contains(model.Code)).Count();
                                if (count > 0)
                                {
                                    scope.Dispose();
                                    return new MessageReturnModel
                                    {
                                        IsSuccess = false,
                                        Message = "Mã cửa hàng " + temp.Code + " đã tồn tại"
                                    };
                                }
                                master_store insertModel = new master_store();
                                insertModel.Id = Guid.NewGuid();
                                insertModel.Code = model.Code ?? "n/a";
                                insertModel.Name = model.Name ?? "n/a";
                                insertModel.CreatedBy = model.CreatedBy;
                                insertModel.CreatedDate = model.CreatedDate.Value;
                                insertModel.HouseNumber = model.HouseNumber ?? "n/a";
                                insertModel.StreetNames = model.StreetNames ?? "n/a";
                                insertModel.Region = model.Region ?? "n/a";
                                insertModel.StoreType = getStoreType(model.StoreTypeName);
                                insertModel.ProvinceId = getProvice(model.ProvinceName) ?? 0;
                                insertModel.DistrictId = getDistrict(model.DistrictName) ?? 0;
                                insertModel.WardId = getWard(model.WardName) ?? 0;

                                _data.master_store.Add(insertModel);
                                _data.SaveChanges();
                                Thread.Sleep(100);
                            }
                            result.IsSuccess = true;
                            scope.Complete();
                        }
                        catch
                        {
                            result.IsSuccess = false;
                            result.Message = "Lỗi khi thêm cửa hàng có mã là " + temp.Code;
                            scope.Dispose();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }


        public MessageReturnModel ImportExcel(List<StoreManagerModel> listStore)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    StoreManagerModel temp = new StoreManagerModel();
                    try
                    {


                        var data_store_types = _data.master_store_type.ToList();
                        var data_provinces = _data.provinces.ToList();
                        var data_districts = _data.districts.ToList();
                        var data_wards = _data.wards.ToList();

                        //listStore.ForEach(f =>
                        //{
                        //    var typeId = (data_store_types.Where(_ => _.Name.ToLower() == (f.StoreType ?? "").ToLower()).FirstOrDefault() != null) ? data_store_types.Where(_ => _.Name.ToLower() == (f.StoreType ?? "").ToLower()).FirstOrDefault().Id : "";
                        //    var proId = (data_provinces.Where(_ => _.Name.ToLower() == (f.ProvinceName ?? "").ToLower()).FirstOrDefault() != null) ? data_provinces.Where(_ => _.Name.ToLower() == (f.ProvinceName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;
                        //    var discId = (data_districts.Where(_ => _.Name.ToLower() == (f.DistrictName ?? "").ToLower()).FirstOrDefault() != null) ? data_districts.Where(_ => _.Name.ToLower() == (f.DistrictName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;
                        //    var wardId = (data_wards.Where(_ => _.Name.ToLower() == (f.WardName ?? "").ToLower()).FirstOrDefault() != null) ? data_wards.Where(_ => _.Name.ToLower() == (f.WardName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;

                        //    f.Id = Guid.NewGuid();
                        //    f.Code = f.Code ?? "n/a";
                        //    f.Name = f.Name ?? "n/a";
                        //    f.CreatedBy = f.CreatedBy;
                        //    f.CreatedDate = f.CreatedDate.Value;
                        //    f.HouseNumber = f.HouseNumber ?? "n/a";
                        //    f.StreetNames = f.StreetNames ?? "n/a";
                        //    f.Region = f.Region ?? "n/a";
                        //    f.StoreType = typeId;
                        //    f.ProvinceId = proId;
                        //    f.DistrictId = discId;
                        //    f.WardId = wardId;
                        //});


                        foreach (var model in listStore)
                        {
                            temp = model;
                            int count = _data.master_store.Where(x => x.Id == model.Id).Count();
                            var typeId = (data_store_types.Where(_ => _.Name.ToLower() == (model.StoreTypeName ?? "").ToLower()).FirstOrDefault() != null) ? data_store_types.Where(_ => _.Name.ToLower() == (model.StoreTypeName ?? "").ToLower()).FirstOrDefault().Id : "";
                            var proId = (data_provinces.Where(_ => _.Name.ToLower() == (model.ProvinceName ?? "").ToLower()).FirstOrDefault() != null) ? data_provinces.Where(_ => _.Name.ToLower() == (model.ProvinceName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;
                            var discId = (data_districts.Where(_ => _.Name.ToLower() == (model.DistrictName ?? "").ToLower()).FirstOrDefault() != null) ? data_districts.Where(_ => _.Name.ToLower() == (model.DistrictName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;
                            var wardId = (data_wards.Where(_ => _.Name.ToLower() == (model.WardName ?? "").ToLower()).FirstOrDefault() != null) ? data_wards.Where(_ => _.Name.ToLower() == (model.WardName ?? "").ToLower()).FirstOrDefault().Id : (long?)null;
                            if (count > 0)
                            {
                                var store = _data.master_store.Find(model.Id);
                                store.Code = model.Code ?? "n/a";
                                store.Name = model.Name ?? "n/a";
                                store.ModifiedBy = model.ModifiedBy;
                                store.ModifiedDate = DateTime.Now;
                                store.HouseNumber = model.HouseNumber ?? "n/a";
                                store.StreetNames = model.StreetNames ?? "n/a";
                                store.Region = model.Region ?? "n/a";
                                store.StoreType = typeId;
                                store.ProvinceId = proId;
                                store.DistrictId = discId;
                                store.WardId = wardId;
                                _data.SaveChanges();
                            }
                            else
                            {
                                master_store insertModel = new master_store
                                {
                                    Id = Guid.NewGuid(),
                                    Code = model.Code ?? "n/a",
                                    Name = model.Name ?? "n/a",
                                    CreatedBy = model.CreatedBy,
                                    CreatedDate = model.CreatedDate.Value,
                                    HouseNumber = model.HouseNumber ?? "n/a",
                                    StreetNames = model.StreetNames ?? "n/a",
                                    Region = model.Region ?? "n/a",
                                    StoreType = typeId,
                                    ProvinceId = proId,
                                    DistrictId = discId,
                                    WardId = wardId
                                };
                                _data.master_store.Add(insertModel);
                                _data.SaveChanges();
                            }
                            //Thread.Sleep(150);
                        }
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = "Lỗi khi thêm cửa hàng có mã là " + temp.Code + ". " + ex.ToString();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private string getStoreType(string name)
        {
            string result = "";
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = _data.master_store_type.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }
        private string getStoreType2(string name, List<master_store_type> lst)
        {
            string result = "";
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = lst.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }

        private long? getDistrict(string name)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = _data.districts.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }
        private long? getDistrict2(string name, List<district> lst)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = lst.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }

        private long? getProvice(string name)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = _data.provinces.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }
        private long? getProvice2(string name, List<province> lst)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = lst.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }

        private long? getWard(string name)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = _data.wards.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }
        private long? getWard2(string name, List<ward> lst)
        {
            long? result = null;
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = lst.Where(x => x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).FirstOrDefault();
                if (data != null)
                    result = data.Id;
            }
            return result;
        }

        public master_store GetByCode(string code)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    return _db.master_store.Where(_ => _.Code == code).FirstOrDefault();
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }

        public master_store getstoreByTrackSSId(string track_sessionid)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var model = _db.Database.SqlQuery<master_store>(string.Format(@"SELECT st.* FROM track_session tr_se LEFT JOIN track tr ON tr_se.TrackId = tr.Id
LEFT JOIN master_store st ON tr.MasterStoreId = st.Id WHERE tr_se.Id = '{0}'", track_sessionid)).FirstOrDefault();
                    return model;
                    //var tr_ss = _db.track_session.Where(_ => _.Id == track_sessionid).FirstOrDefault();
                    //var tr = _db.tracks.Where(_ => _.Id == tr_ss.TrackId).FirstOrDefault();
                    //return _db.master_store.Where(_ => _.Id == tr.MasterStoreId).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<StoreManagerModel> getStoreByProvince(long? provinceId)
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var query = (from ms in _data.master_store
                             join mst in _data.master_store_type
                                  on ms.StoreType equals mst.Id into temp1
                             from ms_mst in temp1.DefaultIfEmpty()
                             join p in _data.provinces
                                  on ms.ProvinceId equals p.Id into temp2
                             from ms_p in temp2.DefaultIfEmpty()
                             join d in _data.districts
                                  on ms.DistrictId equals d.Id into temp3
                             from ms_d in temp3.DefaultIfEmpty()
                             join w in _data.wards
                                  on ms.WardId equals w.Id into temp4
                             from ms_w in temp4.DefaultIfEmpty()
                             where ms.ProvinceId == provinceId
                             select new StoreManagerModel
                             {
                                 Id = ms.Id,
                                 Code = ms.Code,
                                 Name = ms.Name,
                                 DistrictName = ms_d.Name,
                                 ProvinceName = ms_p.Name,
                                 StoreTypeName = ms_mst.Name,
                                 WardName = ms_w.Name,
                                 HouseNumber = ms.HouseNumber,
                                 Region = ms.Region,
                                 StreetNames = ms.StreetNames
                             }).ToList();
                return query;
            }
        }
        public List<StoreManagerModel> getStoreByDistrict(long? districtID)
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var query = (from ms in _data.master_store
                             join mst in _data.master_store_type
                                  on ms.StoreType equals mst.Id into temp1
                             from ms_mst in temp1.DefaultIfEmpty()
                             join p in _data.provinces
                                  on ms.ProvinceId equals p.Id into temp2
                             from ms_p in temp2.DefaultIfEmpty()
                             join d in _data.districts
                                  on ms.DistrictId equals d.Id into temp3
                             from ms_d in temp3.DefaultIfEmpty()
                             join w in _data.wards
                                  on ms.WardId equals w.Id into temp4
                             from ms_w in temp4.DefaultIfEmpty()
                             where ms.DistrictId == districtID
                             select new StoreManagerModel
                             {
                                 Id = ms.Id,
                                 Code = ms.Code,
                                 Name = ms.Name,
                                 DistrictName = ms_d.Name,
                                 ProvinceName = ms_p.Name,
                                 StoreTypeName = ms_mst.Name,
                                 WardName = ms_w.Name,
                                 HouseNumber = ms.HouseNumber,
                                 Region = ms.Region,
                                 StreetNames = ms.StreetNames
                             }).ToList();
                return query;
            }
        }
        public List<StoreManagerModel> getStoreByWard(long? WardID)
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var query = (from ms in _data.master_store
                             join mst in _data.master_store_type
                                  on ms.StoreType equals mst.Id into temp1
                             from ms_mst in temp1.DefaultIfEmpty()
                             join p in _data.provinces
                                  on ms.ProvinceId equals p.Id into temp2
                             from ms_p in temp2.DefaultIfEmpty()
                             join d in _data.districts
                                  on ms.DistrictId equals d.Id into temp3
                             from ms_d in temp3.DefaultIfEmpty()
                             join w in _data.wards
                                  on ms.WardId equals w.Id into temp4
                             from ms_w in temp4.DefaultIfEmpty()
                             where ms.WardId == WardID
                             select new StoreManagerModel
                             {
                                 Id = ms.Id,
                                 Code = ms.Code,
                                 Name = ms.Name,
                                 DistrictName = ms_d.Name,
                                 ProvinceName = ms_p.Name,
                                 StoreTypeName = ms_mst.Name,
                                 WardName = ms_w.Name,
                                 HouseNumber = ms.HouseNumber,
                                 Region = ms.Region,
                                 StreetNames = ms.StreetNames
                             }).ToList();
                return query;
            }
        }

        public List<StoreCoordinatesViewModel> GetListAddresStoreByLocation(long provinceID, long districtID, long wardID, bool getAddress)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    if (!getAddress)
                    {
                        return (from s in _data.master_store
                                where provinceID == (provinceID == 0 ? provinceID : s.ProvinceId)
                                && districtID == (districtID == 0 ? districtID : s.DistrictId)
                                && wardID == (wardID == 0 ? wardID : s.WardId)
                                && (s.LAT.HasValue && s.LNG.HasValue)
                                select new StoreCoordinatesViewModel()
                                {
                                    Id = s.Id,
                                    LAT = s.LAT ?? 0,
                                    LNG = s.LNG ?? 0
                                }).ToList();
                    }
                    return (from s in _data.master_store
                            join p in _data.provinces on s.ProvinceId equals p.Id into province
                            from pr in province.DefaultIfEmpty()
                            join d in _data.districts on s.DistrictId equals d.Id into district
                            from dis in district.DefaultIfEmpty()
                            join w in _data.wards on s.WardId equals w.Id into ward
                            from wd in ward.DefaultIfEmpty()
                            where provinceID == (provinceID == 0 ? provinceID : s.ProvinceId)
                            && districtID == (districtID == 0 ? districtID : s.DistrictId)
                            && wardID == (wardID == 0 ? wardID : s.WardId)
                            select new StoreCoordinatesViewModel()
                            {
                                Id = s.Id,
                                Code = s.Code,
                                HouseNumber = s.HouseNumber ?? "",
                                StreetNames = s.StreetNames ?? "",
                                ProvinceName = (pr == null ? "" : pr.Name),
                                DistrictName = (dis == null ? "" : dis.Name),
                                WardName = (wd == null ? "" : wd.Name),
                                LAT = s.LAT ?? 0,
                                LNG = s.LNG ?? 0
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<StoreCoordinatesViewModel>();
            }
        }

        /// <summary>
        /// Lấy danh sách cửa hàng (id, name) để hiển thị lên combobox
        /// </summary>
        /// <returns></returns>
        public List<StoreManagerModel> GetListStoreToShowOnCombobox()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    return (from m in _data.master_store
                            select new StoreManagerModel() { Id = m.Id, Name = m.Name }).Take(50).ToList();
                }
            }
            catch (Exception)
            {
                return new List<StoreManagerModel>();
            }
        }
        /// <summary>
        /// Lấy danh sách khu vực (name) để hiển thị lên combobox
        /// </summary>
        /// <returns></returns>
        public List<string> GetListRegionToShowOnCombobox()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    List<string> _returnList = new List<string>();

                    var _list = (from m in _data.master_store
                                 where m.Region.Length > 0
                                 select new { m.Region }).Distinct().Take(50).ToList();

                    foreach (var item in _list)
                    {
                        _returnList.Add(item.Region);
                    }

                    return _returnList;
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }

}
