using System;
using System.Collections.Generic;
using System.Text;
using SnT.IO.Ports.Utils;

namespace SnT.IO.Ports
{
    public class SerialPort : Base.CommBase
    {
        private readonly ByteArray rxBuffer = new ByteArray();
        private readonly object bufferLock = new object();
        private readonly Dictionary<byte, byte> replaceBytes = new Dictionary<byte, byte>();

        private Handshake handshake = Handshake.Custom;
        private bool rtsEnable = false;
        private bool dtrEnable = false;

        #region Properties

        /// <summary>
        /// Gets or sets the byte encoding (<see cref="T:System.Text.Encoding" />) for conversion of text.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="T:System.Text.Encoding.ASCII" />.
        /// </remarks>
        public Encoding Encoding { get; set; }

        public Dictionary<byte, byte> ReplaceBytes { get { return replaceBytes; } }

        #region Ignored properties

        /// <summary>
        /// Gets or sets the handshaking protocol for serial port transmission of data.
        /// </summary>
        /// <remarks>
        /// One of the <see cref="T:Snt.IO.Ports.Handshake" /> values. 
        /// The default is Custom.
        /// </remarks>        
        [Obsolete("For compatibility with system SerialPort. Use TxFlowX, RxFlowX, UseDTR instead.")]
        public Handshake Handshake
        {
            get { return handshake; }
            set { handshake = value; SetHandshake(handshake); }
        }

        /// <summary>
        /// Gets or sets a value that enables the Data Terminal Ready 
        /// (DTR) signal during serial communication.
        /// </summary>        
        /// <remarks>
        /// For compatibility with <see cref="T:System.IO.Ports.SerialPort" /> Use UseDTR instead.
        /// </remarks>
        [Obsolete("For compatibility with system SerialPort. Use UseDTR instead.")]
        public bool DtrEnable
        {
            get { return dtrEnable; }
            set
            {                
                dtrEnable = value;
                if (dtrEnable)
                    UseDTR = HSOutput.Online;
                else
                    UseDTR = HSOutput.None;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Request to Send 
        /// (RTS) signal is enabled during serial communication.
        /// </summary>
        /// <remarks>
        /// For compatibility with <see cref="T:System.IO.Ports.SerialPort" /> Use UseRTS instead.
        /// </remarks>
        [Obsolete("For compatibility with system SerialPort. Use UseRTS instead.")]
        public bool RtsEnable 
        {
            get { return rtsEnable; }
            set
            {
                if (handshake == Ports.Handshake.RequestToSend ||
                    handshake == Ports.Handshake.RequestToSendXOnXOff)
                {
                    throw new InvalidOperationException();
                }
                rtsEnable = value;
                if (rtsEnable)
                    UseRTS = HSOutput.Online;
                else
                    UseRTS = HSOutput.None;
            }
        }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public bool DiscardNull { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public string NewLine { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public byte ParityReplace { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public int ReadBufferSize { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public int ReadTimeout { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public int ReceivedBytesThreshold { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public int WriteBufferSize { get; set; }

        /// <summary>
        /// Ignored, only for compatibility with <see cref="T:System.IO.Ports.SerialPort" />
        /// </summary>
        [Obsolete("Ignored in current version")]
        public int WriteTimeout { get; set; }

        #endregion

        #endregion

        #region Events

        private event EventHandler<SerialDataReceivedEventArgs> dataReceived;
        public event EventHandler<SerialDataReceivedEventArgs> DataReceived
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

        private event EventHandler<SerialByteReceivedEventArgs> byteReceived;
        /// <summary>
        /// Byte received from serial line. It could be modified before processing.
        /// </summary>
        public event EventHandler<SerialByteReceivedEventArgs> ByteReceived
        {
            add { byteReceived += value; }
            remove { byteReceived -= value; }
        }

        #endregion

        #region ...ctors

        public SerialPort()
        {
            Encoding = System.Text.Encoding.ASCII;
            handshake = Handshake.Custom;
            InternalSetHandshake(Handshake.Custom);
            NewLine = System.Environment.NewLine;
        }
        public SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Encoding = System.Text.Encoding.ASCII;
            handshake = Handshake.Custom;
            InternalSetHandshake(Handshake.Custom);
            NewLine = System.Environment.NewLine;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get all immediately available bytes, based on the encoding
        /// in the input buffer, but do not clear the buffer.
        /// </summary>
        /// <returns>The contents of the input buffer.</returns>
        public string GetString()
        {
            string data = null;
            lock (bufferLock)
            {
                if (rxBuffer.Count > 0)
                {
                    data = rxBuffer.GetString(Encoding);
                }
            }
            return data;
        }

        /// <summary>
        /// Discards data from the input buffer.
        /// </summary>
        public void DiscardInBuffer()
        {
            rxBuffer.Clear();
        }

        /// <summary>
        /// Reads all immediately available bytes, based on the encoding
        /// in the input buffer and clear input buffer.
        /// </summary>
        /// <returns>The contents of the input buffer.</returns>
        public string ReadExisting()
        {
            string data = null;
            lock (bufferLock)
            {
                if (rxBuffer.Count > 0)
                {
                    data = rxBuffer.GetString(Encoding);
                    rxBuffer.Clear();
                }
            }
            return data;
        }

        /// <summary>
        /// Writes the specified string based on the encoding to the serial port.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void Write(string text)
        {
            Send(Encoding.GetBytes(text));
        }

        /// <summary>
        /// Writes bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        public void Write(byte[] buffer)
        {
            Send(buffer);
        }

        /// <summary>
        /// Writes a specified number of bytes to the serial port using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains the data to write to the port.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Array.Copy(buffer, offset, data, 0, count);
            Send(data);
        }

        /// <summary>
        /// Writes one byte to serial porty.
        /// </summary>
        /// <param name="data">Data byte to write</param>
        public void Write(byte data)
        {
            Send(data);
        }

        /// <summary>
        /// Writes one byte immediately to serial porty. Skips all buffers.
        /// </summary>
        /// <param name="data">Data byte to write</param>
        public void WriteImmediate(byte data)
        {
            SendImmediate(data);
        }

        #endregion

        #region Event handlers

        protected virtual void OnSerialDataReceived(SerialData eventType)
        {
            if (dataReceived != null)
                dataReceived(this, new SerialDataReceivedEventArgs(eventType));
        }

        protected virtual void OnSerialError(Exception exceptionObject)
        {
            if (errorReceived != null)
                errorReceived(this, new SerialErrorReceivedEventArgs(exceptionObject));
        }

        protected virtual byte OnSerialByteReceived(byte data)
        {
            SerialByteReceivedEventArgs ea = new SerialByteReceivedEventArgs(data);
            if (byteReceived != null)
                byteReceived(this, ea);
            return ea.Data;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overrides OnRxChar to process received bytes.
        /// </summary>
        /// <param name="ch">The byte that was received.</param>
        protected override void OnRxChar(byte ch)
        {
            ch = OnSerialByteReceived(ch);
            if ((ReplaceBytes != null) &&
                ReplaceBytes.ContainsKey(ch))
                ch = ReplaceBytes[ch];
            lock (bufferLock)
            {
                rxBuffer.Append(ch);
            }
            OnSerialDataReceived(SerialData.Data);
        }

        protected override void OnRxException(Exception e)
        {
            base.OnRxException(e);
            OnSerialError(e);
        }

        protected override void OnBreak()
        {
            base.OnBreak();
            OnSerialDataReceived(SerialData.Break);
        }

        #endregion

        #region Protected methods

        // 1.6.5
        private void InternalSetHandshake(Handshake handshake)
        {
            // Default
            rtsEnable = false;
            dtrEnable = false;
            switch (handshake)
            {
                case Handshake.Custom:
                    //TxFlowCTS = false;
                    //TxFlowDSR = false;
                    //TxFlowX = false;
                    //TxWhenRxXoff = true;
                    //RxGateDSR = false;
                    //RxFlowX = false;
                    //UseRTS = HSOutput.None;
                    //UseDTR = HSOutput.None;
                    break;
                case Handshake.None:
                    TxFlowCTS = false;
                    //TxFlowDSR = false;
                    TxFlowX = false;
                    TxWhenRxXoff = true;
                    RxGateDSR = false;
                    RxFlowX = false;
                    UseRTS = HSOutput.None;
                    //UseDTR = HSOutput.None;
                    break;
                case Handshake.XOnXOff:
                    TxFlowCTS = false;
                    //TxFlowDSR = false;
                    TxFlowX = true;
                    TxWhenRxXoff = true;
                    RxGateDSR = false;
                    RxFlowX = true;
                    UseRTS = HSOutput.None;
                    //UseDTR = HSOutput.None;
                    break;
                case Handshake.RequestToSend:
                    TxFlowCTS = true;
                    //TxFlowDSR = false;
                    TxFlowX = false;
                    TxWhenRxXoff = true;
                    RxGateDSR = false;
                    RxFlowX = false;
                    UseRTS = HSOutput.Handshake;
                    //UseDTR = HSOutput.None;
                    break;
                case Handshake.RequestToSendXOnXOff:
                    TxFlowCTS = true;
                    //TxFlowDSR = false;
                    TxFlowX = true;
                    TxWhenRxXoff = true;
                    RxGateDSR = false;
                    RxFlowX = true;
                    UseRTS = HSOutput.Handshake;
                    //UseDTR = HSOutput.None;
                    break;
                default:
                    //TxFlowCTS = false;
                    //TxFlowDSR = false;
                    //TxFlowX = false;
                    //TxWhenRxXoff = true;
                    //RxGateDSR = false;
                    //RxFlowX = false;
                    //UseRTS = HSOutput.None;
                    //UseDTR = HSOutput.None;
                    break;
            }
        }

        // 1.6.1, 1.6.5
        protected virtual void SetHandshake(Handshake handshake)
        {
            InternalSetHandshake(handshake);
        }


        #endregion

    }
}
