//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployeeTracking.Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class sys_user
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordChanged { get; set; }
        public string Roles { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Token { get; set; }
        public string UserTypeCode { get; set; }
    }
}
