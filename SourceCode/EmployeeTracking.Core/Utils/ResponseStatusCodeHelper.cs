using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Utils
{
    /// <summary>
    /// Response status code
    /// </summary>
    public enum ResponseStatusCodeHelper
    {

        /// <summary>
        /// Return success when execute action is success
        /// </summary>
        Success,
        /// <summary>
        /// Return error when execute action is error
        /// </summary>
        Error,

        /// <summary>
        /// Whan to tell user know have some notification or something
        /// </summary>
        Warning,

        /// <summary>
        /// Status is allow
        /// </summary>
        OK,

        /// <summary>
        /// Status is disallow
        /// </summary>
        NG
    }
}
