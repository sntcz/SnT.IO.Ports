﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    /// <summary>
    /// Specifies the control protocol used in establishing 
    /// a serial port communication for 
    /// a <see cref="T:Snt.IO.Ports.SerialPort" /> object.
    /// </summary>
    public enum Handshake
    {
        /// <summary>
        /// Don't change setting, this is the default.
        /// </summary>
        Custom = -1,
        /// <summary>
        /// No control is used for the handshake.
        /// </summary>
        None = 0,
        /// <summary>
        /// The XON/XOFF software control protocol is used. 
        /// The XOFF control is sent to stop the transmission of data. 
        /// The XON control is sent to resume the transmission. 
        /// These software controls are used instead of Request to Send (RTS) 
        /// and Clear to Send (CTS) hardware controls.
        /// </summary>
        XOnXOff = 1,
        /// <summary>
        /// Request-to-Send (RTS) hardware flow control is used. 
        /// RTS signals that data is available for transmission. 
        /// If the input buffer becomes full, the RTS line will be set to false. 
        /// The RTS line will be set to true when more room becomes available 
        /// in the input buffer.
        /// </summary>
        RequestToSend = 2,
        /// <summary>
        /// Both the Request-to-Send (RTS) hardware control 
        /// and the XON/XOFF software controls are used.
        /// </summary>
        RequestToSendXOnXOff = 3
    }
}
