using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;

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
                    var data = _data.employee.Where(x=>(string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (!filter.Gender.HasValue || x.Gender == filter.Gender.Value) &&
                    (!filter.Birthday.HasValue || x.Birthday == filter.Birthday) &&
                    (string.IsNullOrEmpty(filter.IdentityCard) || x.IdentityCard.Contains(filter.IdentityCard)) &&
                    (string.IsNullOrEmpty(filter.Phone) || x.Phone.Contains(filter.Phone)) &&
                    (string.IsNullOrEmpty(filter.Owner) || x.Owner.Contains(filter.Owner))).OrderBy(f => f.Name).ToList();
                    int index = 1;
                    foreach(var item in data)
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
    }
}
