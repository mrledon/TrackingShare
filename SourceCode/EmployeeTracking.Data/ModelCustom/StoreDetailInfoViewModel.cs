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
    public class StoreDetailInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StoreType { get; set; }
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
        public string ProvinceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DistrictName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WardName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TrackSessionViewModel> trackSessions;
        /// <summary>
        /// 
        /// </summary>
        public List<StorePOSMViewModel> listPosm;
    }
}
