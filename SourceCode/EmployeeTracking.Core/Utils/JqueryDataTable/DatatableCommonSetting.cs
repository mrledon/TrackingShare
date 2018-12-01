using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Utils.JqueryDataTable
{
    /// <summary>
    /// Common setting config in jquery datatable
    /// </summary>
    public class DatatableCommonSetting
    {
        /// <summary>
        /// Name of response object model
        /// </summary>
        public static class Response
        {
            /// <summary>
            /// Data
            /// </summary>
            public static readonly string DATA = "data";

            /// <summary>
            /// Message
            /// </summary>
            public static readonly string MESSAGE = "message";

            /// <summary>
            /// Status
            /// </summary>
            public static readonly string STATUS = "status";
        }
    }
}
