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
                List<ImageManagementViewModel> _list = new List<ImageManagementViewModel>();

                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    var _lData = (from u in _db.usertypes
                                  orderby u.Name descending
                                  select new
                                  {
                                      u.Id,
                                      u.Code,
                                      u.Name,
                                      u.de
                                  }).ToList();

                    //Area
                    if (request.Region.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Region.Contains(m.Region)
                                  select m).ToList();
                    }

                    //Store
                    if (request.Store.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Store.Contains(m.MasterStoreCode.ToString())
                                  select m).ToList();
                    }

                    //Employee
                    if (request.Employee.Count() > 0)
                    {
                        _lData = (from m in _lData
                                  where request.Employee.Contains(m.EmployeeId.ToString())
                                  select m).ToList();
                    }

                    _itemResponse.draw = request.draw;
                    _itemResponse.recordsTotal = _lData.Count;
                    //Search
                    if (request.search != null && !string.IsNullOrWhiteSpace(request.search.Value))
                    {
                        string searchValue = request.search.Value.ToLower();
                        _lData = _lData.Where(m => m.Date.ToString().ToLower().Contains(searchValue) ||
                                                   m.EmployeeId.ToLower().Contains(searchValue) ||
                                                   m.EmployeeName.ToLower().Contains(searchValue) ||
                                                   m.Manager.ToLower().Contains(searchValue) ||
                                                   m.MasterStoreCode.ToLower().Contains(searchValue) ||
                                                   m.MasterStoreName.ToLower().Contains(searchValue) ||
                                                   m.Region.ToLower().Contains(searchValue)).ToList();
                    }
                    //Add to list
                    foreach (var item in _lData)
                    {
                        var _trackSession = _db.track_session.Where(m => m.TrackId == item.Id).Select(m => new { m.Id, m.Date, m.Status }).ToList();

                        _list.Add(new ImageManagementViewModel()
                        {
                            Id = item.Id,
                            Date = item.Date.ToString("dd-MM-yyyy"),
                            EmployeeId = item.EmployeeId,
                            EmployeeName = item.EmployeeName,
                            Manager = item.Manager,
                            MasterStoreId = item.MasterStoreId,
                            MasterStoreCode = item.MasterStoreCode,
                            MasterStoreName = item.MasterStoreName,
                            StoreStatus = item.StoreStatus ?? false,
                            Region = item.Region,
                            QCStatus = item.QCStatus,
                            QCStatusString = QCStatus.QCStatusData().FirstOrDefault(m => m.Id == item.QCStatus).Name,
                            TrackSessions = _trackSession.Select(m => new TrackSessionViewModel()
                            {
                                Id = m.Id,
                                CreateDate = m.Date,
                                CreateDateString = m.Date.ToString("dd-MM-yyyy"),
                                Status = m.Status ?? false
                            }).ToList()
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

    }
}
