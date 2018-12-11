﻿using EmployeeTracking.Core.Utils;
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
                        Description = _md.Description,
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
                Code = "",
                Name = "",
                Description = "",
                Insert = false,
                details = new List<UserTypeDetailModel>()
            };
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
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
                        bool _selected = false;
                        foreach (var item in _role)
                        {
                            var tmp = (from m in _db.usertypes
                                       join dt in _db.roles_usertypes on m.Code equals dt.RoleCode
                                       where m.Id == id && dt.UserType == item.Code
                                       select m).Count();
                            if(tmp != null && tmp > 0)
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
    }
}