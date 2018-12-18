using System;
using System.Text;
using System.Threading;
using System.IO;

namespace SnT.IO.Ports.Base
{
    /// <summary>
    /// Overlays CommBase to provide line or packet oriented communications to derived classes. Strings
    /// are sent and received and the Transact method is added which transmits a string and then blocks until
    /// a reply string has been received (subject to a timeout).
    /// </summary>
    public abstract class CommLine : CommBase
    {
        private byte[] RxBuffer;
        private uint RxBufferP = 0;
        private string RxString = String.Empty;
        private readonly ManualResetEvent TransFlag = new ManualResetEvent(true);

        /// <summary>
        /// Maximum size of received string (default: 256)
        /// </summary>
        public int RxStringBufferSize { get; set; }
        /// <summary>
        /// ASCII code that terminates a received string (default: CR)
        /// </summary>
        public ASCII RxTerminator { get; set; }
        /// <summary>
        /// ASCII codes that will be ignored in received string (default: null)
        /// </summary>
        public ASCII[] RxFilter { get; set; }
        /// <summary>
        /// Maximum time for the Transact method to complete (default: 500 ms)
        /// </summary>
        public TimeSpan TransactTimeout { get; set; }
        /// <summary>
        /// ASCII codes transmitted after each Send string (default: null)
        /// </summary>
        public ASCII[] TxTerminator { get; set; }

        public CommLine()
            : base()
        {
            Initialize();
        }

        public CommLine(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Initialize();
        }

        private void Initialize()
        {
            RxStringBufferSize = 256;
            RxTerminator = ASCII.CR;
            TransactTimeout = TimeSpan.FromMilliseconds(500);
        }

        protected override void BeforeOpen()
        {
            base.BeforeOpen();
            RxBuffer = new byte[RxStringBufferSize];
        }

        /// <summary>
        /// Queue the ASCII representation of a string and then the set terminator bytes for sending.
        /// </summary>
        /// <param name="toSend">String to be sent.</param>
        protected void Send(string toSend)
        {
            //JH 1.1: Use static encoder for efficiency. Thanks to Prof. Dr. Peter Jesorsky!
            uint l = (uint)Encoding.ASCII.GetByteCount(toSend);
            if (TxTerminator != null) l += (uint)TxTerminator.GetLength(0);
            byte[] b = new byte[l];
            byte[] s = Encoding.ASCII.GetBytes(toSend);
            int i;
            for (i = 0; (i <= s.GetUpperBound(0)); i++) b[i] = s[i];
            if (TxTerminator != null) for (int j = 0; (j <= TxTerminator.GetUpperBound(0)); j++, i++) b[i] = (byte)TxTerminator[j];
            Send(b);
        }

        /// <summary>
        /// Transmits the ASCII representation of a string followed by the set terminator bytes and then
        /// awaits a response string.
        /// </summary>
        /// <param name="toSend">The string to be sent.</param>
        /// <returns>The response string.</returns>
        protected string Transact(string toSend)
        {
            Send(toSend);
            TransFlag.Reset();
            if (!TransFlag.WaitOne(Convert.ToInt32(TransactTimeout.TotalMilliseconds), false)) 
                ThrowException("Timeout");
            string s;
            lock (RxString) { s = RxString; }
            return s;
        }

        /// <summary>
        /// Override this to process unsolicited input lines (not a result of Transact).
        /// </summary>
        /// <param name="s">String containing the received ASCII text.</param>
        protected virtual void OnRxLine(string s) { }

        /// <summary>
        /// Overrides OnRxChar to process received bytes.
        /// </summary>
        /// <param name="ch">The byte that was received.</param>
        protected override void OnRxChar(byte ch)
        {
            ASCII ca = (ASCII)ch;
            if ((ca == RxTerminator) || (RxBufferP > RxBuffer.GetUpperBound(0)))
            {
                //JH 1.1: Use static encoder for efficiency. Thanks to Prof. Dr. Peter Jesorsky!
                lock (RxString) { RxString = Encoding.ASCII.GetString(RxBuffer, 0, (int)RxBufferP); }
                RxBufferP = 0;
                if (TransFlag.WaitOne(0, false))
                {
                    OnRxLine(RxString);
                }
                else
                {
                    TransFlag.Set();
                }
            }
            else
            {
                bool wr = true;
                if (RxFilter != null)
                {
                    for (int i = 0; i <= RxFilter.GetUpperBound(0); i++) if (RxFilter[i] == ca) wr = false;
                }
                if (wr)
                {
                    RxBuffer[RxBufferP] = ch;
                    RxBufferP++;
                }
            }
        }
    }
}
