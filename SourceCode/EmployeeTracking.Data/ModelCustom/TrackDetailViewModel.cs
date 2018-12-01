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
    public class TrackDetailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public sbyte MediaTypeOrder { get; set; }
        public string SessionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TrackDetailImageViewModel> TrackDetailImages { get; set; }
    }
}
