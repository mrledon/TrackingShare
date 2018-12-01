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
    public class ChangeDetailImageViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string SessionDetailId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UrlFile { get; set; }
    }
}
