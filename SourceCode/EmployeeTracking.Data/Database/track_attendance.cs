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
    
    public partial class track_attendance
    {
        public System.Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.TimeSpan> Start { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.TimeSpan> End { get; set; }
        public string EmployeeId { get; set; }
        public string StartCoordinates { get; set; }
        public string EndCoordinates { get; set; }
    }
}
