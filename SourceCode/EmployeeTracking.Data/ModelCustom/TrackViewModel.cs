using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    public class TrackViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MasterStoreName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StreetNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? ProvinceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? DistrictId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? WardId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StoreType { get; set; }
    }
}
