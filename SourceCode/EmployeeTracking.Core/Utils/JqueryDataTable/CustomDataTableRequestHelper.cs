using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Utils.JqueryDataTable
{
    /// <summary>
    /// Use for some cases user want to have some variable included into parameter of jquery datatable
    /// </summary>
    public class CustomDataTableRequestHelper : DataTableRequestHelper
    {
        /// <summary>
        /// Custom table parameter string
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// custom filter parameter string
        /// </summary>
        public string Filter { get; set; } = string.Empty;

        /// <summary>
        /// Custom parameter 1
        /// </summary>
        public string FromDate { get; set; } = string.Empty;

        /// <summary>
        /// Custom parameter 2
        /// </summary>
        public string ToDate { get; set; } = string.Empty;

        /// <summary>
        /// Custom parameter 3
        /// </summary>
        public List<string> Region { get; set; } = new List<string>();

        /// <summary>
        /// Custom parameter 4
        /// </summary>
        public List<string> Store { get; set; } = new List<string>();

        /// <summary>
        /// Custom parameter 5
        /// </summary>
        public List<string> Employee { get; set; } = new List<string>();

        public string UserName { get; set; }

        public string UserTypeCode { get; set; }

        public bool? IsActive{ get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}
