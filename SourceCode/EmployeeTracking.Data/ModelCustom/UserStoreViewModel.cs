using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class UserStoreViewModel
    {
        public long UserId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
    }
    public class StoreSearchViewModel
    {
        public string StoreName { get; set; }
        public string StoreType { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public string WardId { get; set; }
        public string Region { get; set; }
    }
}
