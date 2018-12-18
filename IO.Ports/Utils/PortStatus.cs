using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports.Utils
{
    /// <summary>
    /// Availability status of a port
    /// </summary>
    public enum PortStatus
    {
        /// <summary>
        /// Port exists but is unavailable (may be open to another program)
        /// </summary>
        Unavailable = 0,
        /// <summary>
        /// Available for use
        /// </summary>
        Available = 1,
        /// <summary>
        /// Port does not exist
        /// </summary>
        Absent = -1
    }
}
