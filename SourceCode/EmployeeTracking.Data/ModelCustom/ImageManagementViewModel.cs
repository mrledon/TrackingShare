using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageManagementViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Ngày thực hiên
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Người quản lý
        /// </summary>
        public string Manager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EmployeeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid MasterStoreId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MasterStoreName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MasterStoreCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Create date string formats
        /// </summary>
        public string CreateDateString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? StoreStatus { get; set; }

        public int QCStatus { get; set; } = 1;

        public string QCNote { get; set; }

        public string QCStatusString { get; set; } = "Chưa xem xét";
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TrackSessionViewModel> TrackSessions { get; set; }
        public List<string> lstRole { get; set; }
        public int TrackSessionStatus { get; set; } = -1;
    }
}
