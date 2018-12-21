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
    public class TrackSessionDetailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string TrackDetailId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TrackSessionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime createSessionDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int posmnumber { get; set; }
    }
}
