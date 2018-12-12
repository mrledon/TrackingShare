using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Core.Utils;

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
            if (model.Count > 0)
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    for (int i = 0; i < model.Count; i++)
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
                            for (int j = 0; j < role_usertype.Count; j++)
                            {
                                _db.roles_usertypes.Remove(role_usertype[i]);
                            }
                    }
                    _db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Get list data using jquery datatable
        /// </summary>
        /// <param name="request">Jquery datatable request</param>
        /// <returns><string, object></returns>
        public Dictionary<string, object> List(CustomDataTableRequestHelper request)
        {
            Dictionary<string, object> _return = new Dictionary<string, object>();
            try
            {
                //Declare response data to json object
                DataTableResponse<UserViewModel> _itemResponse = new DataTableResponse<UserViewModel>();
                //List of data
                List<UserViewModel> _list = new List<UserViewModel>();

                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var _lData = (from u in _db.users
                                  join ut in _db.usertypes on u.UserType equals ut.Code
                                  //orderby u.UserName descending
                                  select new
                                  {
                                      u.Id,
                                      u.Email,
                                      u.UserName,
                                      u.PhoneNumber,
                                      u.FullName,
                                      u.UserType,
                                      u.IsActive,
                                      ut.Name
                                  }).ToList();


                    _itemResponse.draw = request.draw;
                    _itemResponse.recordsTotal = _lData.Count;
                    //Search

                    if (request.UserName!=null)
                    {
                        _lData = _lData.Where(m => m.UserName!=null && m.UserName.ToString().ToLower().Contains(request.UserName.ToString().ToLower())).ToList();
                    }
                    if (request.UserTypeCode != null)
                    {
                        _lData = _lData.Where(m => m.UserType!=null && m.UserType.ToString().ToLower().Contains(request.UserTypeCode.ToString().ToLower())).ToList();
                    }
                    if (request.FullName != null)
                    {
                        _lData = _lData.Where(m => m.FullName!=null && m.FullName.ToString().ToLower().Contains(request.FullName.ToString().ToLower())).ToList();
                    }
                    if (request.Email != null)
                    {
                        _lData = _lData.Where(m =>m.Email!=null &&  m.Email.ToString().ToLower().Contains(request.Email.ToString().ToLower())).ToList();
                    }
                    if (request.PhoneNumber != null)
                    {
                        _lData = _lData.Where(m => m.PhoneNumber!=null && m.PhoneNumber.ToString().Contains(request.PhoneNumber.ToString().ToLower())).ToList();
                    }
                    if (request.IsActive != null)
                    {
                        _lData = _lData.Where(m => m.IsActive != null && m.IsActive == request.IsActive).ToList();
                    }
                    foreach (var item in _lData)
                    {
                        _list.Add(new UserViewModel()
                        {
                            Id = item.Id,
                            Email = item.Email,
                            UserName = item.UserName,
                            PhoneNumber = item.PhoneNumber,
                            FullName = item.FullName,
                            UserTypeCode = item.UserType,
                            UserTypeName = item.Name,
                            IsActive = item.IsActive
                        });
                    }
                    _itemResponse.recordsFiltered = _list.Count;
                    _itemResponse.data = _list.Skip(request.start).Take(request.length).ToList();
                    _return.Add(DatatableCommonSetting.Response.DATA, _itemResponse);
                }
                _return.Add(DatatableCommonSetting.Response.STATUS, ResponseStatusCodeHelper.OK);
            }
            catch (Exception ex)
            {
                //throw new ServiceException(FILE_NAME, "List", userID, ex);
            }
            return _return;
        }

        public List<UserTypeModel> getAllUserType()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    var model = (from ut in _data.usertypes
                                 select new UserTypeModel
                                 {
                                     Code = ut.Code,
                                     Name = ut.Name
                                 }).ToList();
                    return model;
                }
            }
            catch (Exception ex)
            {
                return new List<UserTypeModel>();
            }
        }

        public MessageReturnModel Insert(UserViewModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.users.Where(x => x.UserName.Contains(model.UserName)).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Tên đăng nhập đã tồn tại"
                        };
                    }
                    string passwordHash =  UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                    user insertModel = new user
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        PasswordHash = passwordHash,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        UserType = model.UserTypeCode,
                        IsActive = model.IsActive
                    };
                    _data.users.Add(insertModel);
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Thêm mới tài khoản thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel changeIsActive(long? id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    user userupdate = _data.users.Where(x => x.Id == id).FirstOrDefault();
                    if (userupdate == null)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Tài khoản không tồn tại"
                        };
                    }
                    if (userupdate.IsActive == false || userupdate.IsActive == null)
                        userupdate.IsActive = true;
                    else
                        userupdate.IsActive = false;
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Thay đổi trạng thái thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel Delete(long? id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var _u = _db.users.Find(id);
                if (_u != null)
                {
                    _db.users.Remove(_u);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Xoá tài khoản thành công!"
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy tài khoản!"
                    };
                }
            }
        }

        public UserViewModel GetById(long? id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    user model = _data.users.Where(x => x.Id == id).FirstOrDefault();
                    if (model != null)
                    {
                        return new UserViewModel
                        {
                            Id = model.Id,
                            UserName = model.UserName,
                            FullName = model.FullName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            UserTypeCode = model.UserType, 
                            IsActive = model.IsActive
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MessageReturnModel Update(UserViewModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    user userModel = _data.users.Where(x => x.UserName.Contains(model.UserName)).FirstOrDefault();
                    if (userModel==null)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Tài khoản không tồn tại!"
                        };
                    }
                    string passwordHash = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                    if (!passwordHash.Equals(userModel.PasswordHash))
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mật khẩu cũ không chính xác!"
                        };
                    }

                    userModel.Email = model.Email;
                    userModel.PasswordHash = UtilMethods.CreateHashString(model.NewPassWord, WebAppConstant.PasswordAppSalt);
                    userModel.FullName = model.FullName;
                    userModel.PhoneNumber = model.PhoneNumber;
                    userModel.UserType = model.UserTypeCode;
                    userModel.IsActive = model.IsActive;

                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Cập nhật tài khoản thành công!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel ChangePass(UserViewModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    user userModel = _data.users.Where(x => x.UserName.Contains(model.UserName)).FirstOrDefault();
                    if (userModel == null)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Tài khoản không tồn tại!"
                        };
                    }
                    string passwordHash = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                    if (!passwordHash.Equals(userModel.PasswordHash))
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mật khẩu cũ không chính xác!"
                        };
                    }

                    userModel.PasswordHash = UtilMethods.CreateHashString(model.NewPassWord, WebAppConstant.PasswordAppSalt);
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Đổi mật khẩu thành công!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
