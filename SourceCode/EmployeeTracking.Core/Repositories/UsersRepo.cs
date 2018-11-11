﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;

namespace EmployeeTracking.Core.Repositories
{
    public class UsersRepo
    {
        public Tuple<users, string> Login(string username, string pwd)
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
                    { throw new Exception("Username or Password is incorrect !"); }
                    else
                    {
                        q.PasswordHash = "";
                        return new Tuple<users, string>(q, "");
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<users, string>(null, ex.Message);
            }
        }
    }
}