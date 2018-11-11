using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class JsonResultModel<T>
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
