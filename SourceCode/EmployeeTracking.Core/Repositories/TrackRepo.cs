using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class TrackRepo
    {
       

        public IList<TrackMinModel> GetTrackDoneByEmployeeId(string EmployeeId)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.Database.SqlQuery<TrackMinModel>(string.Format(@"SELECT
                                                        DATE_FORMAT(DATE, '%d/%m/%Y') AS Date,
                                                        CONCAT(MS.CODE, ' - ', MS.NAME) AS Store
                                                    FROM TRACK T
                                                        JOIN MASTER_STORE MS ON MS.ID = T.MasterStoreId
                                                    WHERE T.EmployeeId = '{0}'
                                                    ORDER BY
                                                        DATE_FORMAT(DATE, '%d/%m/%Y') DESC", EmployeeId)).ToList();
            }
        }


        public track GetTrackByDate(Guid masterStoreId, string EmployeeId, DateTime now)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                return _db.tracks.FirstOrDefault(_ =>
                _.Date.Year == now.Year && _.Date.Month == now.Month && _.Date.Day == now.Day && _.MasterStoreId == masterStoreId && _.EmployeeId == EmployeeId);
            }
        }
        public string Insert(track model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    _db.tracks.Add(model);
                    _db.SaveChanges();
                    return model.Id;
                }
                catch (Exception)
                {
                    return "";
                }
               
            }
        }
        public void UpdateFromMobile(track model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackModel = _db.tracks.FirstOrDefault(_ => _.Id == model.Id);

                trackModel.DistrictId = model.DistrictId;
                trackModel.HouseNumber = model.HouseNumber;
                trackModel.Lat = model.Lat;
                trackModel.Lng = model.Lng;
                trackModel.MaterStoreName = model.MaterStoreName;
                trackModel.Note = model.Note;
                trackModel.ProvinceId = model.ProvinceId;
                trackModel.Region = model.Region;
                trackModel.StreetNames = model.StreetNames;
                trackModel.WardId = model.WardId;
                trackModel.StoreStatus = model.StoreStatus;
                trackModel.PhoneNumber = model.PhoneNumber;
                trackModel.StoreIsChanged = model.StoreIsChanged;
                trackModel.StoreType = model.StoreType;
                _db.SaveChanges();
            }
        }

        public TrackViewModel GetTrackById(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                track tr= _db.tracks.FirstOrDefault(_ => _.Id == id);
                TrackViewModel model = new TrackViewModel();
                model.Id = tr.Id;
                model.MasterStoreName = tr.MaterStoreName;
                model.HouseNumber = tr.HouseNumber;
                model.StreetNames = tr.StreetNames;
                model.ProvinceId = tr.ProvinceId;
                model.DistrictId = tr.DistrictId;
                model.WardId = tr.WardId;
                model.PhoneNumber = tr.PhoneNumber;
                model.StoreType = tr.StoreType;
                model.StoreStatus = tr.StoreStatus;
                model.Note = tr.Note;
                model.EmployeeId = tr.EmployeeId;
                return model;
            }
        }


        /*Hieu.pt Update Store in Track*/
        public MessageReturnModel Update(TrackViewModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    track updateModel = _data.tracks.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.MaterStoreName = model.MasterStoreName;
                        updateModel.HouseNumber = model.HouseNumber;
                        updateModel.StoreType = model.StoreType;
                        updateModel.PhoneNumber = model.PhoneNumber;
                        updateModel.StreetNames = model.StreetNames;
                        updateModel.DistrictId = model.DistrictId;
                        updateModel.ProvinceId = model.ProvinceId;
                        updateModel.WardId = model.WardId;
                        updateModel.StoreIsChanged = true;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString(),
                            Message = "Cập nhật cửa hàng thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Cập nhật cửa hàng không thành công"
                        };
                    }
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

        /*Hieu.pt Update Store in Track*/
        public MessageReturnModel UpdateStoreStatus(TrackViewModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    track updateModel = _data.tracks.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.StoreStatus = model.StoreStatus;
                        updateModel.Note = model.Note;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString(),
                            Message = "Cập nhật cửa hàng trạng thái thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Cập nhật trạng thái cửa hàng không thành công"
                        };
                    }
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

        /*Hieu.pt Update Employee in Track*/
        public MessageReturnModel UpdateEmployee(TrackViewModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    track updateModel = _data.tracks.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.EmployeeId = model.EmployeeId;
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString(),
                            Message = "Cập nhật cửa hàng trạng thái thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Cập nhật trạng thái cửa hàng không thành công"
                        };
                    }
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
