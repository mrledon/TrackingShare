using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Utils.JqueryDataTable
{
    /// <summary>
    /// Ordering data columns
    /// </summary>
    public class OrderingHelper
    {
        /// <summary>
        /// order column on table
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// Custom property
        /// Set column name for the current ordering column
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Direction
        /// </summary>
        public SortingDirectionHelper Dir { get; set; }
    }
}
