using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    /// <summary>
    /// Stop bit settings
    /// </summary>
    /// <remarks>
    /// Not compatible with System.IO.Ports.StopBits
    /// </remarks>
    public enum StopBits
    {
        /// <summary>
        /// Line is asserted for 1 bit duration at end of each character
        /// </summary>
        One = 0,
        /// <summary>
        /// Line is asserted for 1.5 bit duration at end of each character
        /// </summary>
        OnePointFive = 1,
        /// <summary>
        /// Line is asserted for 2 bit duration at end of each character
        /// </summary>
        Two = 2
    };
}
