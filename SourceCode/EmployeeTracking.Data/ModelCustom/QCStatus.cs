using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class QCStatus
    {
        public int Id { get; set; } = 0;

        public string Name { get; set; } = "";

        public static IEnumerable<QCStatus> QCStatusData()
        {
            return new List<QCStatus>()
            {
                new QCStatus(){Id = 4, Name = "Nhắc nhỡ" },
                new QCStatus(){Id = 3, Name = "Chờ phản hồi thị trường" },
                new QCStatus(){Id = 1, Name = "Đạt" },
                new QCStatus(){Id = 2, Name = "Không đạt" },
                new QCStatus(){Id = 0, Name = "Chưa phân loại" }
            };
        }
    }
}
