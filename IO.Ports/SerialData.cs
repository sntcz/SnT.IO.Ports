using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    
    /// <summary>
    /// Specifies the type of character that was received on the serial port of the object.
    /// </summary>
    public enum SerialData
    {
        /// <summary>
        /// A data was received and placed in the input buffer.
        /// </summary>
        Data = 1,
        /// <summary>
        /// A break condition is detected on the input line.
        /// </summary>
        Break = 2
    }
}
