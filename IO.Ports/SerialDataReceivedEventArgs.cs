using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{    

    public class SerialDataReceivedEventArgs : EventArgs
    {
        public SerialData EventType { get; private set; }

        public SerialDataReceivedEventArgs(SerialData eventType)
        {
            EventType = eventType;
        }
    }
}
