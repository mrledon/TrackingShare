using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Data.Objects;

namespace EmployeeTracking.Core.Repositories
{
    public class EmployeeRepo
    {
        public List<EmployeeManagerModel> GetAllEmployee(EmployeeManagerFilterModel filter)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    List<EmployeeManagerModel> result = new List<EmployeeManagerModel>();
                    var data = _data.employees.Where(x => (string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (!filter.Gender.HasValue || x.Gender == filter.Gender.Value) &&
                    (!filter.Birthday.HasValue || x.Birthday == filter.Birthday) &&
                    (string.IsNullOrEmpty(filter.IdentityCard) || x.IdentityCard.Contains(filter.IdentityCard)) &&
                    (string.IsNullOrEmpty(filter.Phone) || x.Phone.Contains(filter.Phone)) &&
                    (string.IsNullOrEmpty(filter.Owner) || x.Owner.Contains(filter.Owner))).OrderBy(f => f.Name).ToList();
                    int index = 1;
                    foreach (var item in data)
                    {
                        EmployeeManagerModel employee = new EmployeeManagerModel();
                        employee.Index = index; index++;
                        employee.Birthday = item.Birthday;
                        employee.BirthdayString = item.Birthday.HasValue ? item.Birthday.Value.ToString("dd/MM/yyyy") : "";
                        employee.Code = item.Code;
                        employee.CreatedBy = item.CreatedBy;
                        employee.CreatedDate = item.CreatedDate;
                        employee.Gender = item.Gender;
                        employee.GenderString = item.Gender.HasValue ? (item.Gender.Value ? "Nam" : "Nữ") : "";
                        employee.IdentityCard = item.IdentityCard;
                        employee.Id = item.Id;
                        employee.ModifiedBy = item.ModifiedBy;
                        employee.ModifiedDate = item.ModifiedDate;
                        employee.Name = item.Name;
                        employee.Owner = item.Owner;
                        employee.Phone = item.Phone;
                        result.Add(employee);
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                return new List<EmployeeManagerModel>();
            }
        }

        public MessageReturnModel Insert(EmployeeManagerModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.employees.Where(x => x.Id.Contains(model.Id)).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Tên đăng nhập đã tồn tại"
                        };
                    }
                    count = _data.employees.Where(x => x.Code.Contains(model.Code)).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã nhân viên đã tồn tại"
                        };
                    }
                    employee insertModel = new employee
                    {
                        Id = model.Id,
                        Birthday = model.Birthday,
                        Code = model.Code,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate.Value,
                        Gender = model.Gender,
                        IdentityCard = model.IdentityCard,
                        Name = model.Name,
                        Owner = model.Owner,
                        Phone = model.Phone,
                        Password = model.Password
                    };
                    _data.employees.Add(insertModel);
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = insertModel.Id,
                        Message = "Thêm mới nhân viên thành công"
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

        public MessageReturnModel Update(EmployeeManagerModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.employees.Where(x => x.Code.Contains(model.Code) && x.Id != model.Id).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã nhân viên đã tồn tại"
                        };
                    }
                    employee updateModel = _data.employees.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.Birthday = model.Birthday;
                        updateModel.Code = model.Code;
                        updateModel.ModifiedBy = model.ModifiedBy;
                        updateModel.ModifiedDate = model.ModifiedDate;
                        updateModel.Gender = model.Gender;
                        updateModel.IdentityCard = model.IdentityCard;
                        updateModel.Name = model.Name;
                        updateModel.Owner = model.Owner;
                        updateModel.Phone = model.Phone;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id,
                            Message = "Cập nhật nhân viên thành công"
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

        public MessageReturnModel Delete(string id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    employee updateModel = _data.employees.Where(x => x.Id == id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        _data.employees.Remove(updateModel);
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id
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

        public MessageReturnModel ResetPassword(string id, string password)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(password, WebAppConstant.PasswordAppSalt);
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    employee updateModel = _data.employees.Where(x => x.Id == id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.Password = password;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id
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

        public EmployeeManagerModel GetById(string id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    employee model = _data.employees.Where(x => x.Id == id).FirstOrDefault();
                    if (model != null)
                    {
                        return new EmployeeManagerModel
                        {
                            Id = model.Id,
                            Birthday = model.Birthday,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = DateTime.Now,
                            Gender = model.Gender,
                            IdentityCard = model.IdentityCard,
                            Name = model.Name,
                            Owner = model.Owner,
                            Phone = model.Phone,
                            Code = model.Code
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

        public Byte[] GetExportTrackList(EmployeeManagerFilterModel filter)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                List<EmployeeManagerModel> result = new List<EmployeeManagerModel>();
                var data = _db.employees.Where(x => (string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                (!filter.Gender.HasValue || x.Gender == filter.Gender.Value) &&
                (!filter.Birthday.HasValue || x.Birthday == filter.Birthday) &&
                (string.IsNullOrEmpty(filter.IdentityCard) || x.IdentityCard.Contains(filter.IdentityCard)) &&
                (string.IsNullOrEmpty(filter.Phone) || x.Phone.Contains(filter.Phone)) &&
                (string.IsNullOrEmpty(filter.Owner) || x.Owner.Contains(filter.Owner))).OrderBy(f => f.Name).ToList();
                int index = 1;
                foreach (var item in data)
                {
                    EmployeeManagerModel employee = new EmployeeManagerModel();
                    employee.Index = index; index++;
                    employee.Birthday = item.Birthday;
                    employee.BirthdayString = item.Birthday.HasValue ? item.Birthday.Value.ToString("dd/MM/yyyy") : "";
                    employee.Code = item.Code;
                    employee.CreatedBy = item.CreatedBy;
                    employee.CreatedDate = item.CreatedDate;
                    employee.Gender = item.Gender;
                    employee.GenderString = item.Gender.HasValue ? (item.Gender.Value ? "Nam" : "Nữ") : "";
                    employee.IdentityCard = item.IdentityCard;
                    employee.Id = item.Id;
                    employee.ModifiedBy = item.ModifiedBy;
                    employee.ModifiedDate = item.ModifiedDate;
                    employee.Name = item.Name;
                    employee.Owner = item.Owner;
                    employee.Phone = item.Phone;
                    result.Add(employee);
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
                                                "Tên đăng nhập",
                                                "Mã nhân viên",
                                                "Tên nhân viên",
                                                "Giới tính",
                                                "Ngày sinh",
                                                "Số CMND",
                                                "Số điện thoại",
                                                "Người quản lý"
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
                    foreach (var item in result)
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

                        ws.Cells[rowIndex, colIndex].Value = item.Id;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
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

                        ws.Cells[rowIndex, colIndex].Value = item.GenderString;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.BirthdayString;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.IdentityCard;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Phone;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Owner;
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

        public MessageReturnModel ImportExcel(List<EmployeeManagerModel> listEmloyee)
        {
            try
            {
                MessageReturnModel result = new MessageReturnModel();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        EmployeeManagerModel temp = new EmployeeManagerModel();
                        try
                        {
                            foreach (var model in listEmloyee)
                            {
                                temp = model;
                                int count = _data.employees.Where(x => x.Id.Contains(model.Id)).Count();
                                if (count > 0)
                                {
                                    scope.Dispose();
                                    return new MessageReturnModel
                                    {
                                        IsSuccess = false,
                                        Message = "Tên đăng nhập " + temp.Id + " đã tồn tại"
                                    };
                                }
                                count = _data.employees.Where(x => x.Code.Contains(model.Code)).Count();
                                if (count > 0)
                                {
                                    scope.Dispose();
                                    return new MessageReturnModel
                                    {
                                        IsSuccess = false,
                                        Message = "Mã nhân viên " + temp.Code + " đã tồn tại"
                                    };
                                }
                                employee insertModel = new employee
                                {
                                    Id = model.Id,
                                    Birthday = model.Birthday,
                                    Code = model.Code,
                                    CreatedBy = model.CreatedBy,
                                    CreatedDate = model.CreatedDate.Value,
                                    Gender = model.Gender,
                                    IdentityCard = model.IdentityCard,
                                    Name = model.Name,
                                    Owner = model.Owner,
                                    Phone = model.Phone,
                                    Password = model.Password
                                };
                                _data.employees.Add(insertModel);
                                _data.SaveChanges();
                            }
                            result.IsSuccess = true;
                            scope.Complete();
                        }
                        catch
                        {
                            result.IsSuccess = false;
                            result.Message = "Lỗi khi thêm nhân viên có tên đăng nhập là " + temp.Id;
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

        public employee CheckToken(string id, string token)
        {
            var d = DateTime.Now;
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return (
                    from e in _db.employees
                    join tk in _db.employee_token on e.Id equals tk.EmployeeId
                    where
                    tk.Start <= d && d <= tk.End
                    && e.Id == id
                    select e).FirstOrDefault();
            }
        }

        public EmployeeApiModel LoginAPI(employee model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var emp = _db.employees.FirstOrDefault(
                    _ =>
                        _.Id == model.Id &&
                        _.Password == model.Password
                    );
                if (emp == null)
                    throw new Exception(string.Format("Sai tên đăng nhập hoặc mật khẩu.", model.Id));
                else
                {
                    var d = DateTime.Now;
                    var newToken = new employee_token()
                    {
                        EmployeeId = emp.Id,
                        Start = d,
                        End = d.AddHours(168),
                        Id = Guid.NewGuid().ToString()
                    };
                    _db.employee_token.Add(newToken);
                    _db.SaveChanges();

                    return new EmployeeApiModel()
                    {
                        Token = newToken.Id,
                        Id = emp.Id,
                        Name = emp.Name,
                        Start_Token = newToken.Start.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        End_Token = newToken.End.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    };
                }
            }
        }

        public void DeleteEmpTokenExpire(string empId)
        {
            var d = DateTime.Now;
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                _db.employee_token.Where(_ => _.EmployeeId == empId && _.End < d).ToList().ForEach(f => {
                    _db.employee_token.Remove(f);
                    
                });
                _db.SaveChanges();
            }
        }
    }
}
