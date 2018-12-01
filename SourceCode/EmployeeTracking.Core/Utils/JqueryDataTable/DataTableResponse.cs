using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Utils.JqueryDataTable
{
    /// <summary>
    /// JqueryData response to client
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTableResponse<T> where T : class
    {
        /// <summary>
        /// Draw counter. 
        /// This is used by DataTables to ensure that the Ajax returns from server-side processing 
        /// requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence). 
        /// This is used as part of the draw return parameter.
        /// </summary>
        public int draw { get; set; }

        /// <summary>
        /// Sets the Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>
        public long recordsTotal { get; set; }

        /// <summary>
        /// Sets the Total records, after filtering 
        /// (i.e. the total number of records after filtering has been applied - 
        /// not just the number of records being returned in this result set)
        /// </summary>
        public long recordsFiltered { get; set; }

        /// <summary>
        /// Data return
        /// </summary>
        public List<T> data { get; set; }
    }
}
