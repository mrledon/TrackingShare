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
        public string Categorie { get; set; } = "";
        public int Success { get; set; } = 0;
        public int Fail { get; set; } = 0;
        public int UnSubmit { get; set; } = 0;
    }

    public class StatisticNumberStoreDayFormat
    {
        public string categorie { get; set; }
        public List<RateValues> values { get; set; }
    }

    public class RateValues
    {
        public string rate { get; set; }
        public int value { get; set; }
    }
}
