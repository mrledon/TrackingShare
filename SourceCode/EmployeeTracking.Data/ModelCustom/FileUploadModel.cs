using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    public class FileUploadModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SubType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PosmNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }
    }
}