using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class StoreManagerFilterModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string StoreType { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNames { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string Region { get; set; }
    }


    public class StatisticNumberStoreDay
    {
        public int storeSuccesss { get; set; }
        public int storeFail { get; set; }
        public int storeUnSubmit { get; set; }
    }
}
