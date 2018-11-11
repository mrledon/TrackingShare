using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class MessageReturnModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
    }
}
