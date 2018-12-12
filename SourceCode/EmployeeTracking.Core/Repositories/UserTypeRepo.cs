using EmployeeTracking.Core.Utils;
using EmployeeTracking.Core.Utils.JqueryDataTable;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class UserTypeRepo
    {
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
                DataTableResponse<UserTypeModel> _itemResponse = new DataTableResponse<UserTypeModel>();
                //List of data
                List<UserTypeModel> _list = new List<UserTypeModel>();

                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var _lData = (from u in _db.usertypes
                                  orderby u.Name descending
                                  select new
                                  {
                                      u.Id,
                                      u.Code,
                                      u.Name,
                                      u.Description
                                  }).ToList();


                    _itemResponse.draw = request.draw;
                    _itemResponse.recordsTotal = _lData.Count;
                    //Search
                    if (request.search != null && !string.IsNullOrWhiteSpace(request.search.Value))
                    {
                        string searchValue = request.search.Value.ToLower();
                        _lData = _lData.Where(m => m.Code.ToString().ToLower().Contains(searchValue) ||
                                                   m.Name.ToLower().Contains(searchValue) ||
                                                   m.Description.ToLower().Contains(searchValue)).ToList();
                    }
                    //Add to list
                    foreach (var item in _lData)
                    {
                        _list.Add(new UserTypeModel()
                        {
                            Id = item.Id,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserTypeModel GetByIdToView(int id)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var _md = _db.usertypes.FirstOrDefault(m => m.Id == id);
                    if (_md == null)
                    {
                        return new UserTypeModel();
                    }

                    var detailRole = (from m in _db.roles_usertypes
                                      join r in _db.roles on m.RoleCode equals r.Code
                                      where m.UserType == _md.Code
                                      select new
                                      {
                                          r.Name,
                                          r.FunctionalGroup,
                                          r.FunctionalGroupName
                                      }).ToList();

                    return new UserTypeModel()
                    {
                        Id = _md.Id,
                        Code = _md.Code,
                        Name = _md.Name,
                        Description = _md.Description ?? "",
                        details = detailRole.Select(m => new UserTypeDetailModel()
                        {
                            FormCode = m.FunctionalGroup,
                            FormName = m.FunctionalGroupName,
                            RoleName = m.Name
                        }).ToList()
                    };


                }
            }
            catch
            {
                return new UserTypeModel();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserTypeModel GetById(int id)
        {
            UserTypeModel _returnModel = new UserTypeModel()
            {
                Id = 0,
                Code = Guid.NewGuid().ToString().Substring(0, 8),
                Name = "",
                Description = "",
                Insert = true,
                details = new List<UserTypeDetailModel>()
            };
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    //Role
                    var _role = _db.roles.ToList();
                    if (id == 0)
                    {
                        foreach (var item in _role)
                        {
                            _returnModel.details.Add(new UserTypeDetailModel()
                            {
                                Id = item.Id,
                                RoleCode = item.Code,
                                RoleName = item.Name,
                                FormCode = item.FunctionalGroup,
                                FormName = item.FunctionalGroupName,
                                Selected = false
                            });
                        }
                    }
                    else
                    {
                        //User Type
                        var userType = _db.usertypes.FirstOrDefault(m => m.Id == id);
                        if (userType == null)
                        {
                            throw new Exception();
                        }
                        _returnModel.Id = id;
                        _returnModel.Code = userType.Code;
                        _returnModel.Name = userType.Name;
                        _returnModel.Description = userType.Description;
                        _returnModel.Insert = false;
                        //Role
                        bool _selected = false;
                        foreach (var item in _role)
                        {
                            _selected = false;
                            var tmp = (from m in _db.roles_usertypes
                                       where m.UserType == userType.Code && m.RoleCode == item.Code
                                       select m).Count();
                            if (tmp > 0)
                            {
                                _selected = true;
                            }

                            _returnModel.details.Add(new UserTypeDetailModel()
                            {
                                Id = item.Id,
                                RoleCode = item.Code,
                                RoleName = item.Name,
                                FormCode = item.FunctionalGroup,
                                FormName = item.FunctionalGroupName,
                                Selected = _selected
                            });
                        }

                    }

                }
            }
            catch
            {
            }
            return _returnModel;
        }


        public MessageReturnModel Save(UserTypeModel model)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    MessageReturnModel _returnModel = new MessageReturnModel
                    {
                        IsSuccess = true,
                        Message = "Thêm mới thành công"
                    };
                    try
                    {
                        if (model.Insert)
                        {
                            usertype _md = new usertype()
                            {
                                Code = model.Code,
                                Name = model.Name,
                                Description = model.Description.Trim(),
                                STATIC = true,
                                ISACTIVE = true
                            };
                            _db.usertypes.Add(_md);
                            _db.SaveChanges();

                            foreach (var item in model.details)
                            {
                                roles_usertypes dt = new roles_usertypes()
                                {
                                    RoleCode = item.RoleCode,
                                    UserType = model.Code
                                };
                                _db.roles_usertypes.Add(dt);
                                _db.SaveChanges();
                            }

                        }
                        else
                        {
                            usertype _md = _db.usertypes.FirstOrDefault(m => m.Id == model.Id);
                            if (_md == null)
                            {
                                return new MessageReturnModel
                                {
                                    IsSuccess = false,
                                    Message = "Mã không tồn tại"
                                };
                            }

                            //Save user type
                            _md.Name = model.Name;
                            _md.Description = model.Description.Trim();
                            _db.usertypes.Attach(_md);

                            //Remove old detail
                            var _oldRole = _db.roles_usertypes.Where(m => m.UserType == model.Code).ToList();
                            foreach (var item in _oldRole)
                            {
                                _db.roles_usertypes.Remove(item);
                            }

                            _db.SaveChanges();

                            //Save new
                            foreach (var item in model.details)
                            {
                                if (item.RoleCode.Length == 0)
                                {
                                    continue;
                                }
                                roles_usertypes dt = new roles_usertypes()
                                {
                                    RoleCode = item.RoleCode,
                                    UserType = model.Code
                                };
                                _db.roles_usertypes.Add(dt);
                            }

                            _db.SaveChanges();

                            _returnModel = new MessageReturnModel
                            {
                                IsSuccess = true,
                                Message = "Cập nhật thành công"
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
                    return _returnModel;
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

        public MessageReturnModel Delete(int id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var _md = _db.usertypes.Find(id);
                if (_md != null)
                {
                    var _detail = _db.roles_usertypes.Where(m => m.RoleCode == _md.Code).ToList();
                    foreach (var item in _detail)
                    {
                        _db.roles_usertypes.Remove(item);
                    }
                    _db.usertypes.Remove(_md);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại tài khoản!"
                    };
                }
            }
        }

        public MessageReturnModel CheckDelete(int id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var _count = (from m in _db.usertypes
                           join u in _db.users on m.Code equals u.UserType
                           where m.Id == id
                           select u).Count();
                if (_count > 0)
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Thông tin đang được sử dụng trong tài khoản, Không được phép xóa"
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
            }
        }

    }
}
