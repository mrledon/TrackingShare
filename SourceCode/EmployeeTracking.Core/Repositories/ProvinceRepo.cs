using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class ProvinceRepo
    {
        public IList<province> GetAll()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.provinces
                    .OrderBy(_ => _.Type).ThenBy(_ => _.Name).ToList();
            }
        }

        public IList<ProvinceModel> CountStoreAllProvince()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var _countStore = (from m in _db.master_store
                                   where m.ProvinceId != null
                             group m by m.ProvinceId into s
                             select new
                             {
                                 key = s.Key,
                                 count = s.Count()
                             });

                return (from p in _db.provinces
                        join s in _countStore on p.Id equals s.key
                        select new ProvinceModel() { Id = (long)s.key, Name = p.Name, NumOfStore = s.count }).ToList();
            }
        }
    }
}
