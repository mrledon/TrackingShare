//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployeeTracking.Data.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class master_store
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNames { get; set; }
        public Nullable<long> ProvinceId { get; set; }
        public Nullable<long> DistrictId { get; set; }
        public Nullable<long> WardId { get; set; }
        public string Region { get; set; }
        public Nullable<double> LAT { get; set; }
        public Nullable<double> LNG { get; set; }
        public Nullable<System.Guid> MasterStoreDetailId { get; set; }
        public string StoreType { get; set; }
    }
}
