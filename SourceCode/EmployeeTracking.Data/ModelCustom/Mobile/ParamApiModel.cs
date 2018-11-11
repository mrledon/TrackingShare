using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EmployeeTracking.Data.ModelCustom.Mobile
{
    public class ParamApiModel
    {
        public string Token { get; set; }
    }
    public class WardApiModel
    {
        public long DistrictId { get; set; }
    }
    public class DistrictApiModel
    {
        public long ProvinceId { get; set; }
    }
    public class AttendanceApiModel : ParamApiModel
    {
        public string Id { get; set; } //EmployeeId
        public string AttendanceStart { get; set; } // yyyy-MM-dd HH:mm:ss
        public string AttendanceEnd { get; set; } // yyyy-MM-dd HH:mm:ss
    }
    public class TrackingInforModel : ParamApiModel
    {
        public string Id { get; set; }
    }

    public class TrackingDataModel : ParamApiModel
    {
        public string Id { get; set; }

        //public string EmployeeId { get; set; }
        public string MaterStoreName { get; set; }
        public string HouseNumber { get; set; }
        public string StreetNames { get; set; }
        public Nullable<long> ProvinceId { get; set; }
        public Nullable<long> DistrictId { get; set; }
        public Nullable<long> WardId { get; set; }
        //public string Region { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Note { get; set; }
        public string Region { get; set; }
        public System.Guid MasterStoreId { get; set; }

        public string Date { get; set; }
    }

    public class TrackingDataFileModel : ParamApiModel
    {
        public string Code { get; set; }
        public string Date { get; set; }
        public System.Guid MasterStoreId { get; set; }
        public string Id { get; set; }
        //public HttpPostedFileBase __file { get; set; }
    }
}
