using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class AttendanceManagerModel
    {
        public Guid Id { get; set; }
        public TimeSpan? Start { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public TimeSpan? End { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int Index { get; set; }
    }
}
