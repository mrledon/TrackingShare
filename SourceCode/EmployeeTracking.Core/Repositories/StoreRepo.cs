using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                    //List<StoreManagerModel> result = new List<StoreManagerModel>();
                    //var data = _data.master_store.Where(x => (string.IsNullOrEmpty(filter.Code) || x.Code.Contains(filter.Code)) &&
                    //(string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                    //(string.IsNullOrEmpty(filter.StoreType) || x.StoreType.Contains(filter.StoreType)) &&
                    //(string.IsNullOrEmpty(filter.HouseNumber) || x.HouseNumber.Contains(filter.HouseNumber)) &&
                    //(string.IsNullOrEmpty(filter.StreetNames) || x.StreetNames.Contains(filter.StreetNames)) &&
                    //(!filter.ProvinceId.HasValue || x.ProvinceId == filter.ProvinceId) &&
                    //(!filter.DistrictId.HasValue || x.DistrictId == filter.DistrictId) &&
                    //(!filter.WardId.HasValue || x.WardId == filter.WardId) &&
                    //(string.IsNullOrEmpty(filter.Region) || x.Region.Contains(filter.Region))).OrderBy(f => f.Name).ToList();
                    //int index = 1;
                    //foreach (var item in data)
                    //{
                    //    StoreManagerModel employee = new StoreManagerModel();
                    //    employee.Index = index; index++;
                    //    employee.Id = item.Id;
                    //    employee.Code = item.Code;
                    //    employee.CreatedBy = item.CreatedBy;
                    //    employee.CreatedDate = item.CreatedDate;
                    //    employee.DistrictId = item.DistrictId;
                    //    employee.HouseNumber = item.HouseNumber;
                    //    employee.Name = item.Name;
                    //    employee.ProvinceId = item.ProvinceId;
                    //    employee.Region = item.Region;
                    //    employee.StoreType = item.StoreType;
                    //    employee.StreetNames = item.StreetNames;
                    //    employee.WardId = item.WardId;
                    //    result.Add(employee);
                    //}
                    var query = (from ms in _data.master_store
                    join mst in _data.master_store_type
                         on ms.StoreType equals mst.Id into temp1
                    from ms_mst in temp1.DefaultIfEmpty()
                    join p in _data.provinces
                         on ms.ProvinceId equals p.Id into temp2
                    from ms_p in temp2.DefaultIfEmpty()
                    join d in _data.districts
                         on ms.DistrictId equals d.Id into temp3
                    from ms_d in temp3.DefaultIfEmpty()
                    join w in _data.wards
                         on ms.WardId equals w.Id into temp4
                    from ms_w in temp4.DefaultIfEmpty()
                    where (string.IsNullOrEmpty(filter.Code) || ms.Code.Contains(filter.Code)) &&
                    (string.IsNullOrEmpty(filter.Name) || ms.Name.Contains(filter.Name)) &&
                    (string.IsNullOrEmpty(filter.StoreType) || ms.StoreType.Contains(filter.StoreType)) &&
                    (string.IsNullOrEmpty(filter.HouseNumber) || ms.HouseNumber.Contains(filter.HouseNumber)) &&
                    (string.IsNullOrEmpty(filter.StreetNames) || ms.StreetNames.Contains(filter.StreetNames)) &&
                    (!filter.ProvinceId.HasValue || ms.ProvinceId == filter.ProvinceId) &&
                    (!filter.DistrictId.HasValue || ms.DistrictId == filter.DistrictId) &&
                    (!filter.WardId.HasValue || ms.WardId == filter.WardId) &&
                    (string.IsNullOrEmpty(filter.Region) || ms.Region.Contains(filter.Region))
                    select new StoreManagerModel
                    {
                        Id = ms.Id,
                        Code = ms.Code,
                        Name = ms.Name,
                        DistrictName = ms_d.Name,
                        ProvinceName = ms_p.Name,
                        StoreTypeName = ms_mst.Name,
                        WardName = ms_w.Name,
                        HouseNumber = ms.HouseNumber,
                        Region = ms.Region,
                        StreetNames = ms.StreetNames
                    }).ToList();

                    int index = 1;
                    foreach (var item in query)
                    {
                        item.Index = index;
                        index++;
                    }
                    return query;
                }
            }
            catch (Exception ex)
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
                        Id = Guid.NewGuid(),
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

        public MessageReturnModel Delete(Guid id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store updateModel = _data.master_store.Where(x => x.Id == id).FirstOrDefault();
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

        public StoreManagerModel GetById(Guid id)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    master_store model = _data.master_store.Where(x => x.Id == id).FirstOrDefault();
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

    public class StoreExecStore
    {
        //public int Index { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string StoreTypeName { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
    }
}
