using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class ProvinceModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Number of store by province
        /// </summary>
        public int NumOfStore { get; set; }
    }
}
