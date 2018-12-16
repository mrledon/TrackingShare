using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class UserEmployeeManagerModel
    {
        public long UserId { get; set; }

        public string EmployeeId { get; set; } = "";

        public string EmployeeCode { get; set; } = "";

        public string EmployeeName { get; set; } = "";

        public string Phone { get; set; } = "";
    }
}
