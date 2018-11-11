using EmployeeTracking.Data.Database;
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
        public IList<TrackMinModel> GetTrackByEmployeeId(string EmployeeId)
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
                _db.tracks.Add(model);
                _db.SaveChanges();
                return model.Id;
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
                _db.SaveChanges();
            }
        }
    }
}
