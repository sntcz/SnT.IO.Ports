using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.PortSniffer.Sniffer
{
    class ByteReceivedEventArgs : EventArgs
    {
        public byte Data { get; private set; }

        public ByteReceivedEventArgs(byte data)
        {
            Data = data;
        }
    }
}
