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
    
    public partial class track
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string MaterStoreName { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNames { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Region { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Note { get; set; }
        public Nullable<System.Guid> MasterStoreId { get; set; }
    }
}
