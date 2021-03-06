﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    public class StoreInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpDistrict { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpWard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpHouseNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SbvpStreetName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixDistrict { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixWard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixHouseNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DigixStreetName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<bool> StoreIsChanged { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Note { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }
    }
    [DataContract]
    public class StoreCoordinatesViewModel
    {
        [DataMember]
        public Guid? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public double? LAT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? LNG { get; set; }
        [DataMember]
        public string HouseNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
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
    }
}
