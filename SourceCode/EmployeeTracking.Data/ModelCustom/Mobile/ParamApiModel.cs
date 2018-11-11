using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
