using System;
using System.Collections.Generic;
using System.Text;
using SnT.IO.Ports.Base;
using SnT.IO.Ports;

namespace SnT.IO.PortSniffer.Sniffer
{
    class CommSniffer : CommBase
    {

        private event EventHandler<ByteReceivedEventArgs> dataReceived;
        public event EventHandler<ByteReceivedEventArgs> DataReceived
        {
            add { dataReceived += value; }
            remove { dataReceived -= value; }
        }

        private event EventHandler<SerialErrorReceivedEventArgs> errorReceived;
        public event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived
        {
            add { errorReceived += value; }
            remove { errorReceived -= value; }
        }

        public CommSniffer()
        {
            XonChar = ASCII.DC1;
        }

        protected virtual void OnSerialDataReceived(byte data)
        {
            if (dataReceived != null)
                dataReceived(this, new ByteReceivedEventArgs(data));
        }

        protected virtual void OnSerialError(Exception exceptionObject)
        {
            if (errorReceived != null)
                errorReceived(this, new SerialErrorReceivedEventArgs(exceptionObject));
        }

        protected override void OnRxChar(byte ch)
        {
            OnSerialDataReceived(ch);
        }

        protected override void OnRxException(Exception e)
        {
            base.OnRxException(e);
            OnSerialError(e);
        }

        public void Write(byte data)
        {
            Send(data);
        }
    }
}
