﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class employeetracking_devEntities : DbContext
{
    public employeetracking_devEntities()
        : base("name=employeetracking_devEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public DbSet<employee> employees { get; set; }

    public DbSet<employee_token> employee_token { get; set; }

    public DbSet<role> roles { get; set; }

    public DbSet<media_type> media_type { get; set; }

    public DbSet<district> districts { get; set; }

    public DbSet<province> provinces { get; set; }

    public DbSet<ward> wards { get; set; }

    public DbSet<master_store_type> master_store_type { get; set; }

    public DbSet<track_session> track_session { get; set; }

    public DbSet<track_attendance> track_attendance { get; set; }

    public DbSet<track_detail> track_detail { get; set; }

    public DbSet<user> users { get; set; }

    public DbSet<master_store> master_store { get; set; }

    public DbSet<track> tracks { get; set; }

}

}

