using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    public class SerialErrorReceivedEventArgs : EventArgs
    {
        public Exception ExceptionObject { get; private set; }

        public SerialErrorReceivedEventArgs(Exception exceptionObject)
        {
            ExceptionObject = exceptionObject;
        }

    }
}
