using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    public class TrackDetailImageViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PosmNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeSub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EmployeeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TrackSessionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MasterStoreId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TrackId { get; set; }
    }
}
