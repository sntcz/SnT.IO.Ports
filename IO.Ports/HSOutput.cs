using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    /// <summary>
    /// Uses for RTS or DTR pins
    /// </summary>
    public enum HSOutput
    {
        /// <summary>
        /// Pin is asserted when this station is able to receive data.
        /// </summary>
        Handshake = 2,
        /// <summary>
        /// Pin is asserted when this station is transmitting data (RTS on NT, 2000 or XP only).
        /// </summary>
        Gate = 3,
        /// <summary>
        /// Pin is asserted when this station is online (port is open).
        /// </summary>
        Online = 1,
        /// <summary>
        /// Pin is never asserted.
        /// </summary>
        None = 0
    };
}
