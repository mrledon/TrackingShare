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
                    int count = _data.master_store.Where(x => x.Code.Contains(model.Code)).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã cửa hàng đã tồn tại"
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
                        WardId = model.WardId
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
                    int count = _data.master_store.Where(x => x.Code.Contains(model.Code) && x.Id != model.Id).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã nhân viên đã tồn tại"
                        };
                    }
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
                            Message = "Không tìm thấy nhân viên"
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
                                                "STT",
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
                        ws.Cells[rowIndex, colIndex].Value = item.Index;
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

        public MessageReturnModel ImportExcel(List<StoreManagerModel> listStore)
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
                                master_store insertModel = new master_store
                                {
                                    Code = model.Code,
                                    Name = model.Name,
                                    CreatedBy = model.CreatedBy,
                                    CreatedDate = model.CreatedDate.Value,
                                    HouseNumber = model.HouseNumber,
                                    StreetNames = model.StreetNames,
                                    Region = model.Region,
                                    StoreType = getStoreType(model.StoreTypeName),
                                    ProvinceId = getProvice(model.ProvinceName),
                                    DistrictId = getDistrict(model.DistrictName),
                                    WardId = getWard(model.WardName)
                                };
                                _data.master_store.Add(insertModel);
                                _data.SaveChanges();
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

        private string getStoreType(string name)
        {
            string result = "";
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var data = _data.master_store_type.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
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
                var data = _data.districts.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
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
                var data = _data.provinces.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
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
                var data = _data.wards.Where(x => x.Name.ToLower().Contains(name.ToLower())).FirstOrDefault();
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

                    var tr_ss = _db.track_session.Where(_ => _.Id == track_sessionid).FirstOrDefault();
                    var tr = _db.tracks.Where(_ => _.Id == tr_ss.TrackId).FirstOrDefault();
                    return _db.master_store.Where(_ => _.Id == tr.MasterStoreId).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class StoreExecStore
    {
        //public int Index { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string StoreTypeName { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
    }
}
