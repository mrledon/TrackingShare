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
    }
}
