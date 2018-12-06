using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;

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

        public List<RoleUserTypeViewModel> GetRoleByUserType(string userType)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    var model = (from ut in _data.roles_usertypes
                                 where ut.UserType == userType
                                 select new RoleUserTypeViewModel
                                 {
                                     RoleCode = ut.RoleCode,
                                     UserTypeCode = ut.UserType
                                 }).ToList();
                    return model;
                }
            }
            catch (Exception ex)
            {
                return new List<RoleUserTypeViewModel>();
            }
        }

        public void InsertRoleUserType(List<RoleUserTypeViewModel> model)
        {
            if(model.Count>0)
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    for (int i=0; i<model.Count; i++)
                    {
                        roles_usertypes roles_UserType = new roles_usertypes();
                        roles_UserType.RoleCode = model[i].RoleCode;
                        roles_UserType.UserType = model[i].UserTypeCode;
                        _db.roles_usertypes.Add(roles_UserType);
                    }
                    _db.SaveChanges();
                }
            }
        }

        public void DeleteRoleUserType(List<RoleUserTypeViewModel> model)
        {
            if (model.Count > 0)
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    
                    for (int i = 0; i < model.Count; i++)
                    {
                        var role_usertype = _db.roles_usertypes.Where(x => x.RoleCode == model[i].RoleCode && x.UserType == model[i].UserTypeCode).ToList(); 
                        if (role_usertype.Count > 0)
                        for (int j = 0; j<role_usertype.Count; j++)
                        {
                                _db.roles_usertypes.Remove(role_usertype[i]);
                        }
                    }
                    _db.SaveChanges();
                }
            }
        }
    }
}
