using EmployeeTracking.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.ModelCustom;

namespace EmployeeTracking.Core.Repositories
{
    public class MediaTypeRepo
    {
        public media_type GetByCode(string Code)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.media_type.FirstOrDefault(_ => _.Code == Code);
            }
        }

        public List<media_type> GetAllWithDefault()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.media_type.Where(_ => _.IsActive == true).ToList();
            }
        }
        public List<media_type> GetAll()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.media_type.Where(_ => _.IsActive == true && _.Code != "DEFAULT").ToList();
            }
        }
        public List<MinModel> GetOnlyPOSM()
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    return _db.media_type.Where(_ => _.IsActive == true 
                    && _.Code != "DEFAULT"
                    && _.Code != "SELFIE"
                    && _.Code != "STORE_FAILED"
                    ).Select(f => new MinModel{
                        Value = f.Code,
                        Text=f.Name
                    }).ToList();
                }
            }
            catch (Exception)
            {
                return new List<MinModel>();
            }
           
        }
    }
}
