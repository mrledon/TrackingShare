using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom.Mobile
{
    public class EmployeeApiModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public string Start_Token { get; set; }

        public string End_Token { get; set; }
    }
}
