using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Models
{
    public partial class Employeetracking_devEntities : DbContext
    {
        public Employeetracking_devEntities()
            : base("name=employeetracking_devEntities")
        {
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}

        public DbSet<employee> employees { get; set; }
        public DbSet<master_store> master_store { get; set; }
        public DbSet<role> roles { get; set; }
        public DbSet<sys_user_track_media_rm> sys_user_track_media_rm { get; set; }
        public DbSet<track> tracks { get; set; }
        public DbSet<track_attendance> track_attendance { get; set; }
        public DbSet<track_detail> track_detail { get; set; }
        public DbSet<track_media_rm> track_media_rm { get; set; }
        public DbSet<userclaim> userclaims { get; set; }
        public DbSet<userlogin> userlogins { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<media_type> media_type { get; set; }
    }
}