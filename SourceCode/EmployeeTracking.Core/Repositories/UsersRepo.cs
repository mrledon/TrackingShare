using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;

namespace EmployeeTracking.Core.Repositories
{
    public class UsersRepo
    {
        public Tuple<user, string> Login(string username, string pwd)
        {
            try
            {
                var passEncode = UtilMethods.CreateHashString(pwd, WebAppConstant.PasswordAppSalt);
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    var q = _data.users.Where(n =>
                        (n.UserName == username) &&
                        (n.PasswordHash == passEncode)
                        ).FirstOrDefault();
                    if (q == null)
                    { throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác !"); }
                    else
                    {
                        q.PasswordHash = "";
                        return new Tuple<user, string>(q, "");
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<user, string>(null, ex.Message);
            }
        }
    }
}
