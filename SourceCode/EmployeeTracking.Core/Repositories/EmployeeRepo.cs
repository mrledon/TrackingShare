﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;

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

        public bool Insert(EmployeeManagerModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    employee insertModel = new employee
                    {
                        Id = rnd.Next(1, 999999).ToString(),
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
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(EmployeeManagerModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Delete(string id)
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ResetPassword(string id, string password)
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        public employee CheckToken(string id, string token)
        {
            var d = DateTime.Now;
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return (
                    from e in _db.employees
                    join tk in _db.employee_token on e.Id equals tk.EmployeeId
                    where tk.Start <= d && d <= tk.End
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
                    throw new Exception(string.Format("Employee {0} not found.", model.Id));
                else
                {
                    var d = DateTime.Now;
                    var newToken = new employee_token()
                    {
                        EmployeeId = emp.Id,
                        Start = d,
                        End = d.AddHours(1),
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

    }
}
