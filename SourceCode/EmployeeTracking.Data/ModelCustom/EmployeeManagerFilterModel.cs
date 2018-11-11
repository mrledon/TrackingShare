using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class EmployeeManagerFilterModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdentityCard { get; set; }
        public string Phone { get; set; }
        public string Owner { get; set; }
    }
}
