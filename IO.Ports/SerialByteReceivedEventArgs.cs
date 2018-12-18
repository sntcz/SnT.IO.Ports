using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{    

    public class SerialByteReceivedEventArgs : EventArgs
    {
        public byte Data { get; set; }
        //TODO: Add handled and discard support
        // Don't process ReplaceBytes
        //public bool Handled { get; set; }
        // Don't append to buffer
        //public bool Discard { get; set; }

        public SerialByteReceivedEventArgs(byte data)
        {
            Data = data;
            //Handled = false;
            //Discard = false;
        }
    }
}
