using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTracking.Core.Repositories
{
    public class StoreRepo
    {
        public List<StoreManagerModel> GetAllEmployee(StoreManagerFilterModel filter)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    List<StoreManagerModel> result = new List<StoreManagerModel>();
                    var data = _data.master_store.Where(x => (string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                    (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    (string.IsNullOrEmpty(filter.StoreType) || x.StoreType.Contains(filter.StoreType)) &&
                    (string.IsNullOrEmpty(filter.HouseNumber) || x.HouseNumber.Contains(filter.HouseNumber)) &&
                    (string.IsNullOrEmpty(filter.StreetNames) || x.StreetNames.Contains(filter.StreetNames)) &&
                    (!filter.ProvinceId.HasValue || x.ProvinceId == filter.ProvinceId) &&
                    (!filter.DistrictId.HasValue || x.DistrictId == filter.DistrictId) &&
                    (!filter.WardId.HasValue || x.WardId == filter.WardId) &&
                    (string.IsNullOrEmpty(filter.Region) || x.Region.Contains(filter.Region))).OrderBy(f => f.Name).ToList();
                    int index = 1;
                    foreach (var item in data)
                    {
                        StoreManagerModel employee = new StoreManagerModel();
                        employee.Index = index; index++;
                        employee.Id = item.Id;
                        employee.Code = item.Code;
                        employee.CreatedBy = item.CreatedBy;
                        employee.CreatedDate = item.CreatedDate;
                        employee.DistrictId = item.DistrictId;
                        employee.HouseNumber = item.HouseNumber;
                        employee.Name = item.Name;
                        employee.ProvinceId = item.ProvinceId;
                        employee.Region = item.Region;
                        employee.StoreType = item.StoreType;
                        employee.StreetNames = item.StreetNames;
                        employee.WardId = item.WardId;
                        result.Add(employee);
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                return new List<StoreManagerModel>();
            }
        }

        public MessageReturnModel Insert(StoreManagerModel model)
        {
            try
            {
                //var passEncode = UtilMethods.CreateHashString(model.Password, WebAppConstant.PasswordAppSalt);
                //Random rnd = new Random();
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.master_store.Where(x => x.Code.Contains(model.Code)).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã cửa hàng đã tồn tại"
                        };
                    }
                    master_store insertModel = new master_store
                    {
                        //Id = rnd.Next(1, 999999).ToString(),
                        Code = model.Code,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate.Value,
                        DistrictId = model.DistrictId,
                        HouseNumber = model.HouseNumber,
                        Name = model.Name,
                        ProvinceId = model.ProvinceId,
                        Region = model.Region,
                        StoreType = model.StoreType,
                        StreetNames = model.StreetNames,
                        WardId = model.WardId
                    };
                    _data.master_store.Add(insertModel);
                    _data.SaveChanges();
                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = insertModel.Id.ToString(),
                        Message = "Thêm mới cửa hàng thành công"
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

        public MessageReturnModel Update(StoreManagerModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    int count = _data.master_store.Where(x => x.Code.Contains(model.Code) && x.Id != model.Id).Count();
                    if (count > 0)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Mã nhân viên đã tồn tại"
                        };
                    }
                    master_store updateModel = _data.master_store.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.ModifiedBy = model.ModifiedBy;
                        updateModel.ModifiedDate = model.ModifiedDate;
                        updateModel.Code = model.Code;
                        updateModel.DistrictId = model.DistrictId;
                        updateModel.HouseNumber = model.HouseNumber;
                        updateModel.Name = model.Name;
                        updateModel.ProvinceId = model.ProvinceId;
                        updateModel.Region = model.Region;
                        updateModel.StoreType = model.StoreType;
                        updateModel.StreetNames = model.StreetNames;
                        updateModel.WardId = model.WardId;
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
                            Message = "Không tìm thấy nhân viên"
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

        public MessageReturnModel Delete(string id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store updateModel = _data.master_store.Where(x => x.Id.ToString() == id).FirstOrDefault();
                    if (updateModel != null)
                    {
                        _data.master_store.Remove(updateModel);
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Id = updateModel.Id.ToString()
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm thấy cửa hàng"
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

        public StoreManagerModel GetById(string id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store model = _data.master_store.Where(x => x.Id.ToString() == id).FirstOrDefault();
                    if (model != null)
                    {
                        return new StoreManagerModel
                        {
                            Id = model.Id,
                            Code = model.Code,
                            CreatedBy = model.CreatedBy,
                            CreatedDate = model.CreatedDate,
                            DistrictId = model.DistrictId,
                            HouseNumber = model.HouseNumber,
                            Name = model.Name,
                            ProvinceId = model.ProvinceId,
                            Region = model.Region,
                            StoreType = model.StoreType,
                            StreetNames = model.StreetNames,
                            WardId = model.WardId
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

        public master_store GetByCode(string code)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                try
                {
                    return _db.master_store.Where(_ => _.Code == code).FirstOrDefault();
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }
    }
}
