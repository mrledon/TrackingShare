using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;

namespace EmployeeTracking.Data.ModelCustom
{
    public class AccountModel : user
    {
        public AccountModel()
        {
            UserName = "";
            PasswordHash = "";
        }
        public bool Remember { get; set; }
    }
}
