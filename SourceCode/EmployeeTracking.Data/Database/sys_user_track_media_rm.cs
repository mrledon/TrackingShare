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
    
    public partial class sys_user_track_media_rm
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public System.Guid SysUserId { get; set; }
        public System.Guid TrackMediaId { get; set; }
        public System.Guid MasterStoreId { get; set; }
        public Nullable<double> LAT { get; set; }
        public Nullable<double> LNG { get; set; }
        public string MediaType { get; set; }
    }
}