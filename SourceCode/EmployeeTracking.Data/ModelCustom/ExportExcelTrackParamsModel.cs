using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class ExportExcelTrackParamsModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<string> Region { get; set; }
        public List<string> Store { get; set; }
        public List<string> Employee { get; set; }
    }
}
