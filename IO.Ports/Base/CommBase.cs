using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

//JH 1.1: Version 1.1 changes labelled thus.
//JH 1.2: Version 1.2 changes labelled thus.
//JH 1.3: Version 1.3 changes labelled thus.
//TK 1.4: Version 1.4 (2009-08-05) changes labelled thus.
//TK 1.5: Removed unused references
//TK 1.6: SerialPort.cs - Added ReplaceBytes dictionary

// Original source: http://msdn.microsoft.com/en-us/magazine/cc301786.aspx
// Original author: John Hind (John.Hind@zen.co.uk)
// Original code download: http://download.microsoft.com/download/8/3/f/83f69587-47f1-48e2-86a6-aab14f01f1fe/NetSerialComm.exe
namespace SnT.IO.Ports.Base
{

    /// <summary>
    /// Lowest level Com driver handling all Win32 API calls and processing send and receive in terms of
    /// individual bytes. Used as a base class for higher level drivers.
    /// </summary>
    public class CommBase : IDisposable
    {
        private IntPtr hPort;
        private IntPtr ptrUWO = IntPtr.Zero;
        private Thread rxThread = null;
        private Exception rxException = null;
        private bool rxExceptionReported = false;
        private int writeCount = 0;
        private readonly ManualResetEvent writeEvent = new ManualResetEvent(false);
        //JH 1.2: Added below to improve robustness of thread start-up.
        private readonly ManualResetEvent startEvent = new ManualResetEvent(false);
        private int stateRTS = 2;
        private int stateDTR = 2;
        private int stateBRK = 2;
        //JH 1.3: Added to support the new congestion detection scheme (following two lines):
        private readonly bool[] empty = new bool[1];
        private bool dataQueued = false;

        #region Properties

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating the open or closed status of the port.
        /// </summary>
        public bool IsOpen { get { CheckDisposed(); return hPort != IntPtr.Zero; } }

        //public bool IsBluetooth { get; private set; }
        //public ulong BluetoohAddress { get; private set; }

        private string _PortName;
        /// <summary>
        /// Gets or sets the port for communications, 
        /// including but not limited to all available COM ports.
        /// </summary>
        public string PortName
        {
            get
            {
                CheckDisposed();
                return _PortName;
            }
            set
            {
                CheckDisposed();
                if (IsOpen)
                    throw new InvalidOperationException("Cannot change name of opened port");
                _PortName = value;
            }
        }

        private int _BaudRate;
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            get
            {
                CheckDisposed();
                return _BaudRate;
            }
            set
            {
                CheckDisposed();
                _BaudRate = value;
                SetCommState((dcb) => { dcb.BaudRate = (byte)_BaudRate; return dcb; });
            }
        }

        private Parity _Parity;
        /// <summary>
        /// Gets or sets the parity-checking protocol. (default: none)
        /// </summary>
        public Parity Parity
        {
            get
            {
                CheckDisposed();
                return _Parity;
            }
            set
            {
                CheckDisposed();
                _Parity = value;
                SetCommState((dcb) =>
                {
                    dcb.Parity = (byte)_Parity;
                    if (_Parity == Parity.Even || _Parity == Parity.Odd)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_PARITY;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_PARITY;
                    return dcb;
                });
            }
        }

        private int _DataBits;
        /// <summary>
        ///  Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits
        {
            get
            {
                CheckDisposed();
                return _DataBits;
            }
            set
            {
                CheckDisposed();
                _DataBits = value;
                SetCommState((dcb) =>
                {
                    dcb.ByteSize = (byte)_DataBits;
                    return dcb;
                });
            }
        }

        private StopBits _StopBits;
        /// <summary>
        /// Gets or sets the number of stop bits. (default: one)
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                //CheckDisposed();
                return _StopBits;
            }
            set
            {
                CheckDisposed();
                _StopBits = value;
                SetCommState((dcb) =>
                {
                    dcb.StopBits = (byte)_StopBits;
                    return dcb;
                });
            }
        }

        private bool _TxFlowCTS;
        /// <summary>
        /// If true, transmission is halted unless CTS is asserted 
        /// by the remote station (default: false)
        /// </summary>
        public bool TxFlowCTS
        {
            get
            {
                CheckDisposed();
                return _TxFlowCTS;
            }
            set
            {
                CheckDisposed();
                _TxFlowCTS = value;
                SetCommState((dcb) =>
                {
                    if (_TxFlowCTS)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_TX_FLOW_CTS;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_TX_FLOW_CTS;
                    return dcb;
                });
            }
        }

        private bool _TxFlowDSR;
        /// <summary>
        /// If true, transmission is halted unless DSR is asserted by the remote station (default: false)
        /// </summary>
        public bool TxFlowDSR
        {
            get
            {
                CheckDisposed();
                return _TxFlowDSR;
            }
            set
            {
                CheckDisposed();
                _TxFlowDSR = value;
                SetCommState((dcb) =>
                {
                    if (_TxFlowDSR)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_TX_FLOW_DSR;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_TX_FLOW_DSR;
                    return dcb;
                });
            }
        }

        private bool _TxFlowX;
        /// <summary>
        /// If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)
        /// </summary>
        public bool TxFlowX
        {
            get
            {
                CheckDisposed();
                return _TxFlowX;
            }
            set
            {
                CheckDisposed();
                _TxFlowX = value;
                SetCommState((dcb) =>
                {
                    if (_TxFlowX)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_TX_FLOW_X;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_TX_FLOW_X;
                    return dcb;
                });
            }
        }

        private bool _TxWhenRxXoff;
        /// <summary>
        /// If false, transmission is suspended when this station has sent Xoff to the remote station (default: true)
        /// Set false if the remote station treats any character as an Xon.
        /// </summary>
        public bool TxWhenRxXoff
        {
            get
            {
                CheckDisposed();
                return _TxWhenRxXoff;
            }
            set
            {
                CheckDisposed();
                _TxWhenRxXoff = value;
                SetCommState((dcb) =>
                {
                    if (_TxWhenRxXoff)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_TX_WHEN_RX_XOFF;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_TX_WHEN_RX_XOFF;
                    return dcb;
                });
            }
        }

        private bool _RxGateDSR;
        /// <summary>
        /// If true, received characters are ignored unless DSR is asserted by the remote station (default: false)
        /// </summary>
        public bool RxGateDSR
        {
            get
            {
                CheckDisposed();
                return _RxGateDSR;
            }
            set
            {
                CheckDisposed();
                _RxGateDSR = value;
                SetCommState((dcb) =>
                {
                    if (_RxGateDSR)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_RX_GATE_DSR;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_RX_GATE_DSR;
                    return dcb;
                });
            }
        }

        private bool _RxFlowX;
        /// <summary>
        /// If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)
        /// </summary>
        public bool RxFlowX
        {
            get
            {
                CheckDisposed();
                return _RxFlowX;
            }
            set
            {
                CheckDisposed();
                _RxFlowX = value;
                SetCommState((dcb) =>
                {
                    if (_RxFlowX)
                        dcb.PackedValues |= Win32.NativeMethods.DCB_RX_FLOW_X;
                    else
                        dcb.PackedValues &= ~Win32.NativeMethods.DCB_RX_FLOW_X;
                    return dcb;
                });
            }
        }

        private HSOutput _UseRTS;
        /// <summary>
        /// Specifies the use to which the RTS output is put (default: none)
        /// </summary>
        public HSOutput UseRTS
        {
            get
            {
                CheckDisposed();
                return _UseRTS;
            }
            set
            {
                CheckDisposed();
                _UseRTS = value;
                SetCommState((dcb) =>
                {
                    // Clean bits
                    dcb.PackedValues &= ~(0x0003 << 12);
                    // Set bits for new use RTS
                    dcb.PackedValues |= (((int)_UseRTS & 0x0003) << 12);
                    return dcb;
                });
            }
        }

        private HSOutput _UseDTR;
        /// <summary>
        /// Specifies the use to which the DTR output is put (default: none)
        /// </summary>
        public HSOutput UseDTR
        {
            get
            {
                CheckDisposed();
                return _UseDTR;
            }
            set
            {
                CheckDisposed();
                _UseDTR = value;
                SetCommState((dcb) =>
                {
                    // Clean bits
                    dcb.PackedValues &= ~(0x0003 << 4);
                    // Set bits for new use DTR
                    dcb.PackedValues |= (((int)_UseDTR & 0x0003) << 4);
                    return dcb;
                });
            }
        }

        private ASCII _XonChar;
        /// <summary>
        /// The character used to signal Xon for X flow control (default: DC1)
        /// </summary>
        public ASCII XonChar
        {
            get
            {
                CheckDisposed();
                return _XonChar;
            }
            set
            {
                CheckDisposed();
                _XonChar = value;
                SetCommState((dcb) =>
                {
                    dcb.XonChar = (byte)_XonChar;
                    return dcb;
                });
            }
        }

        private ASCII _XoffChar;
        /// <summary>
        /// The character used to signal Xoff for X flow control (default: DC3)
        /// </summary>
        public ASCII XoffChar
        {
            get
            {
                CheckDisposed();
                return _XoffChar;
            }
            set
            {
                CheckDisposed();
                _XoffChar = value;
                SetCommState((dcb) =>
                {
                    dcb.XoffChar = (byte)_XoffChar;
                    return dcb;
                });
            }
        }

        //JH 1.2: Next two defaults changed to 0 to use new defaulting mechanism dependant on queue size.
        private int _RxHighWater;
        /// <summary>
        /// The number of free bytes in the reception queue at which flow is disabled
        /// (Default: 0 = Set to 1/10th of actual rxQueue size)
        /// </summary>
        public int RxHighWater
        {
            get
            {
                CheckDisposed();
                return _RxHighWater;
            }
            set
            {
                CheckDisposed();
                _RxHighWater = value;
                SetCommState((dcb) =>
                {
                    SetXonXoffLim(ref dcb);
                    return dcb;
                });
            }
        }

        private int _RxLowWater;
        /// <summary>
        /// The number of bytes in the reception queue at which flow is re-enabled
        /// (Default: 0 = Set to 1/10th of actual rxQueue size)
        /// </summary>
        public int RxLowWater
        {
            get
            {
                CheckDisposed();
                return _RxLowWater;
            }
            set
            {
                CheckDisposed();
                _RxLowWater = value;
                SetCommState((dcb) =>
                {
                    SetXonXoffLim(ref dcb);
                    return dcb;
                });
            }
        }

        private uint _SendTimeoutMultiplier;
        /// <summary>
        /// Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant
        /// (default: 0 = No timeout)
        /// </summary>
        public uint SendTimeoutMultiplier
        {
            get
            {
                CheckDisposed();
                return _SendTimeoutMultiplier;
            }
            set
            {
                CheckDisposed();
                _SendTimeoutMultiplier = value;
                if (IsOpen)
                {
                    Win32.NativeMethods.COMMTIMEOUTS commTimeouts = Win32.NativeMethods.COMMTIMEOUTS.Create(_SendTimeoutMultiplier, _SendTimeoutConstant);
                    if (!Win32.NativeMethods.SetCommTimeouts(hPort, ref commTimeouts))
                        ThrowException(String.Format("Bad timeout multiplier settings (hPort = {0})", hPort));
                }
            }
        }

        private uint _SendTimeoutConstant;
        /// <summary>
        /// Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)
        /// </summary>
        public uint SendTimeoutConstant
        {
            get
            {
                CheckDisposed();
                return _SendTimeoutConstant;
            }
            set
            {
                CheckDisposed();
                _SendTimeoutConstant = value;
                if (IsOpen)
                {
                    Win32.NativeMethods.COMMTIMEOUTS commTimeouts = Win32.NativeMethods.COMMTIMEOUTS.Create(_SendTimeoutMultiplier, _SendTimeoutConstant);
                    if (!Win32.NativeMethods.SetCommTimeouts(hPort, ref commTimeouts))
                        ThrowException(String.Format("Bad timeout constant settings (hPort = {0})", hPort));
                }
            }
        }

        private int _RxQueue;
        /// <summary>
        /// Requested size for receive queue (default: 0 = use operating system default)
        /// </summary>
        public int RxQueue
        {
            get
            {
                CheckDisposed();
                return _RxQueue;
            }
            set
            {
                CheckDisposed();
                _RxQueue = value;
                if (IsOpen && (_RxQueue != 0) || (_TxQueue != 0))
                    if (!Win32.NativeMethods.SetupComm(hPort, (uint)_RxQueue, (uint)_TxQueue))
                        ThrowException(String.Format("Bad rx queue settings (hPort = {0})", hPort));
            }
        }

        private int _TxQueue;
        /// <summary>
        /// Requested size for transmit queue (default: 0 = use operating system default)
        /// </summary>
        public int TxQueue
        {
            get
            {
                CheckDisposed();
                return _TxQueue;
            }
            set
            {
                CheckDisposed();
                _TxQueue = value;
                if (IsOpen && (_RxQueue != 0) || (_TxQueue != 0))
                    if (!Win32.NativeMethods.SetupComm(hPort, (uint)_RxQueue, (uint)_TxQueue))
                        ThrowException(String.Format("Bad tx queue settings (hPort = {0})", hPort));
            }
        }

        private bool _CheckAllSends;
        /// <summary>
        /// If true, subsequent Send commands wait for completion of earlier ones enabling the results
        /// to be checked. If false, errors, including timeouts, may not be detected, but performance
        /// may be better.
        /// </summary>
        public bool CheckAllSends
        {
            get
            {
                CheckDisposed();
                return _CheckAllSends;
            }
            set
            {
                CheckDisposed();
                _CheckAllSends = value;
            }
        }

        private bool _IgnoreErrorFraming;
        /// <summary>
        /// Ignore UART framing error
        /// </summary>
        public bool IgnoreErrorFraming
        {
            get
            {
                CheckDisposed();
                return _IgnoreErrorFraming;
            }
            set
            {
                CheckDisposed();
                _IgnoreErrorFraming = value;
            }
        }

        private bool _IgnoreErrorOverrun;
        /// <summary>
        /// Ignore UART overrun error
        /// </summary>
        public bool IgnoreErrorOverrun
        {
            get
            {
                CheckDisposed();
                return _IgnoreErrorOverrun;
            }
            set
            {
                CheckDisposed();
                _IgnoreErrorOverrun = value;
            }
        }

        private bool _IgnoreErrorParity;
        /// <summary>
        /// Ignore UART parity error
        /// </summary>
        public bool IgnoreErrorParity
        {
            get
            {
                CheckDisposed();
                return _IgnoreErrorParity;
            }
            set
            {
                CheckDisposed();
                _IgnoreErrorParity = value;
            }
        }

        #endregion

        #region ...ctors

        public CommBase()
        {
            IsDisposed = false;

            hPort = IntPtr.Zero;

            _PortName = "COM1";
            _BaudRate = 9600;
            _Parity = Parity.None;
            _DataBits = 8;
            _StopBits = StopBits.One;
            _TxFlowCTS = false;
            _TxFlowDSR = false;
            _TxFlowX = false;
            _TxWhenRxXoff = true;
            _RxGateDSR = false;
            _RxFlowX = false;
            _UseRTS = HSOutput.None;
            _UseDTR = HSOutput.None;
            _XoffChar = ASCII.DC3;
            //JH 1.2: Next two defaults changed to 0 to use new defaulting mechanism dependant on queue size.
            _RxHighWater = 0;
            _RxLowWater = 0;
            _SendTimeoutMultiplier = 0;
            _SendTimeoutConstant = 0;
            _RxQueue = 0;
            _TxQueue = 0;
            _CheckAllSends = true;

            _IgnoreErrorFraming = false;
            _IgnoreErrorOverrun = false;
            _IgnoreErrorParity = false;
        }

        public CommBase(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : this()
        {
            _PortName = portName;
            _BaudRate = baudRate;
            _Parity = parity;
            _DataBits = dataBits;
            _StopBits = stopBits;
        }

        /// <summary>
        /// Destructor (just in case)
        /// </summary>
        ~CommBase()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable implementation

        //TK 1.4: Added disposable pattern

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            Close();
            if (disposing)
            {
                //TK 1.4: Dispose managed resources.
                writeEvent.Close();
                startEvent.Close();
            }
            IsDisposed = true;
            OnDisposed();
        } // Dispose(disposing)

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } // Dispose()

        private void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(CommBase));
        }

        #endregion

        private void SetXonXoffLim(ref Win32.NativeMethods.DCB PortDCB)
        {
            Win32.NativeMethods.COMMPROP cp;
            //JH 1.2: Defaulting mechanism for handshake thresholds - prevents problems of setting specific
            //defaults which may violate the size of the actually granted queue. If the user specifically sets
            //these values, it's their problem!
            if ((_RxLowWater == 0) || (_RxHighWater == 0))
            {
                if (!Win32.NativeMethods.GetCommProperties(hPort, out cp))
                    cp.dwCurrentRxQueue = 0;
                if (cp.dwCurrentRxQueue > 0)
                {
                    //If we can determine the queue size, default to 1/10th, 8/10ths, 1/10th.
                    //Note that HighWater is measured from top of queue.
                    PortDCB.XoffLim = PortDCB.XonLim = (short)((int)cp.dwCurrentRxQueue / 10);
                }
                else
                {
                    //If we do not know the queue size, set very low defaults for safety.
                    PortDCB.XoffLim = PortDCB.XonLim = 8;
                }
            }
            else
            {
                PortDCB.XoffLim = (short)_RxHighWater;
                PortDCB.XonLim = (short)_RxLowWater;
            }
        }

        /// <summary>
        /// Opens the com port and configures it with the required settings
        /// </summary>
        /// <returns>false if the port could not be opened</returns>
        public bool Open()
        {
            CheckDisposed();
            Win32.NativeMethods.DCB portDCB;
            Win32.NativeMethods.COMMTIMEOUTS commTimeouts;
            Win32.NativeMethods.OVERLAPPED wo = new Win32.NativeMethods.OVERLAPPED();

            if (IsOpen)
                return false;

            BeforeOpen();

            hPort = Win32.NativeMethods.CreateFile(PortName, Win32.NativeMethods.GENERIC_READ | Win32.NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero,
                Win32.NativeMethods.OPEN_EXISTING, Win32.NativeMethods.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
            if (hPort == (IntPtr)Win32.NativeMethods.INVALID_HANDLE_VALUE)
            {
                if (Marshal.GetLastWin32Error() == Win32.NativeMethods.ERROR_ACCESS_DENIED)
                {
                    return false;
                }
                else
                {
                    //JH 1.3: Try alternative name form if main one fails:
                    hPort = Win32.NativeMethods.CreateFile(Utils.PortHelper.AltName(PortName), Win32.NativeMethods.GENERIC_READ | Win32.NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero,
                        Win32.NativeMethods.OPEN_EXISTING, Win32.NativeMethods.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                    if (hPort == (IntPtr)Win32.NativeMethods.INVALID_HANDLE_VALUE)
                    {
                        hPort = IntPtr.Zero;
                        if (Marshal.GetLastWin32Error() == Win32.NativeMethods.ERROR_ACCESS_DENIED)
                        {
                            return false;
                        }
                        else
                        {
                            throw new CommPortException(String.Format("Port Open Failure. Error Code: {0}.", Marshal.GetLastWin32Error()));
                        }
                    }
                }
            }

            commTimeouts = Win32.NativeMethods.COMMTIMEOUTS.Create(_SendTimeoutMultiplier, _SendTimeoutConstant);

            try
            {
                portDCB = Win32.NativeMethods.DCB.Create(hPort, _Parity, _TxFlowCTS, _TxFlowDSR,
                    _UseDTR, _RxGateDSR, _TxWhenRxXoff, _TxFlowX, _RxFlowX, _UseRTS, _BaudRate,
                    _DataBits, _StopBits, _XoffChar, _XonChar);
            }
            catch (Exception ex)
            {
                ThrowException(String.Format("Comm settings create failed (hPort = {0})", hPort), ex);
                return false;
            }

            if ((_RxQueue != 0) || (_TxQueue != 0))
                if (!Win32.NativeMethods.SetupComm(hPort, (uint)_RxQueue, (uint)_TxQueue))
                    ThrowException(String.Format("Bad queue settings (hPort = {0})", hPort));

            SetXonXoffLim(ref portDCB);

            if (!Win32.NativeMethods.SetCommState(hPort, ref portDCB))
                ThrowException(String.Format("Bad com settings (hPort = {0})", hPort));

            if (!Win32.NativeMethods.SetCommTimeouts(hPort, ref commTimeouts))
                ThrowException(String.Format("Bad timeout settings (hPort = {0})", hPort));

            // Bluetooth test
            //UInt32 uCnt = 0;            

            //IntPtr buffer = Marshal.AllocHGlobal(sizeof(Int64));
            //bool result = Win32.NativeMethods.DeviceIoControl(hPort,
            //    Win32.NativeMethods.IOCTL_BLUETOOTH_GET_PEER_DEVICE,
            //    IntPtr.Zero, 0, buffer, 8, out uCnt, IntPtr.Zero);
            //long l = Marshal.ReadInt64(buffer);
            //Marshal.FreeHGlobal(buffer);

            //if (result)
            //{
            //    IsBluetooth = true;
            //    //BluetoohAddress = BitConverter.ToUInt64(btAddress, 0);
            //    BluetoohAddress = Convert.ToUInt64(l);
            //    //string strAddress = btAddress.ToString("X12");
            //    Console.WriteLine("resultOK, count: " + uCnt.ToString());
            //}
            //else
            //{
            //    IsBluetooth = false;
            //    BluetoohAddress = 0;
            //    int lastWin32Error = Marshal.GetLastWin32Error();
            //    Exception innerException = new System.ComponentModel.Win32Exception(lastWin32Error);
            //    Console.WriteLine("result error: " + lastWin32Error.ToString() + " => " + innerException.Message);
            //    Console.WriteLine("       count: " + uCnt.ToString());
            //}

            stateBRK = 0;
            if (_UseDTR == HSOutput.None)
                stateDTR = 0;
            if (_UseDTR == HSOutput.Online)
                stateDTR = 1;
            if (_UseRTS == HSOutput.None)
                stateRTS = 0;
            if (_UseRTS == HSOutput.Online)
                stateRTS = 1;

            wo.Offset = 0;
            wo.OffsetHigh = 0;
            if (CheckAllSends)
                wo.hEvent = writeEvent.Handle;
            else
                wo.hEvent = IntPtr.Zero;

            ptrUWO = Marshal.AllocHGlobal(Marshal.SizeOf(wo));

            Marshal.StructureToPtr(wo, ptrUWO, true);
            writeCount = 0;
            //JH1.3:
            empty[0] = true;
            dataQueued = false;

            rxException = null;
            rxExceptionReported = false;
            rxThread = new Thread(new ThreadStart(this.ReceiveThread));
            rxThread.Name = "CommBaseRx";
            rxThread.Priority = ThreadPriority.AboveNormal;
            rxThread.IsBackground = true;
            rxThread.Start();

            //JH1.2: More robust thread start-up wait.
            startEvent.WaitOne(500, false);

            AfterOpen();

            return true;
        }

        /// <summary>
        /// Closes the com port.
        /// </summary>
        public void Close()
        {
            if (IsOpen)
            {
                BeforeClose(false);
                InternalClose();
                rxException = null;
            }
        }

        private void InternalClose()
        {
            Win32.NativeMethods.CancelIo(hPort);
            if (rxThread != null)
            {
                try
                {
                    rxThread.Abort();
                    //JH 1.3: Improve robustness of Close in case were followed by Open:
                    rxThread.Join(100);
                }
                catch (System.Threading.ThreadAbortException)
                { /* NOP */ }
                rxThread = null;
            }
            //TK 1.4: Purge in/out buffers (some drivers do not accept CancelIo)
            Win32.NativeMethods.PurgeComm(hPort, Win32.NativeMethods.PURGE_RXABORT + Win32.NativeMethods.PURGE_RXCLEAR);
            Win32.NativeMethods.PurgeComm(hPort, Win32.NativeMethods.PURGE_TXABORT + Win32.NativeMethods.PURGE_TXCLEAR);
            // Now we can close handle without hangs
            Win32.NativeMethods.CloseHandle(hPort);
            if (ptrUWO != IntPtr.Zero)
                Marshal.FreeHGlobal(ptrUWO);
            stateRTS = 2;
            stateDTR = 2;
            stateBRK = 2;
            hPort = IntPtr.Zero;
        }

        /// <summary>
        /// Block until all bytes in the queue have been transmitted.
        /// </summary>
        public void Flush()
        {
            CheckOnline();
            CheckResult();
        }

        /// <summary>
        /// Use this to throw exceptions in derived classes. Correctly handles threading issues
        /// and closes the port if necessary.
        /// </summary>
        /// <param name="reason">Description of fault</param>
        protected void ThrowException(string reason)
        {
            int lastWin32Error = Marshal.GetLastWin32Error();
            Exception innerException = null;
            if (lastWin32Error != Win32.NativeMethods.ERROR_SUCCESS)
            {
                innerException = new System.ComponentModel.Win32Exception(lastWin32Error);
                reason = String.Format("{0} Error Code: {1}", reason, lastWin32Error);
            }
            ThrowException(reason, innerException);
        }

        /// <summary>
        /// Use this to throw exceptions in derived classes. Correctly handles threading issues
        /// and closes the port if necessary.
        /// </summary>
        /// <param name="reason">Description of fault</param>
        /// <param name="innerException">Inner exception</param>
        protected void ThrowException(string reason, Exception innerException)
        {
            if (Thread.CurrentThread == rxThread)
            {
                throw new CommPortException(reason, innerException);
            }
            else
            {
                if (IsOpen)
                {
                    BeforeClose(true);
                    InternalClose();
                }
                if (rxException == null)
                {
                    throw new CommPortException(reason, innerException);
                }
                else
                {
                    throw new CommPortException(rxException);
                }
            }
        }

        private void SetCommState(Func<Win32.NativeMethods.DCB, Win32.NativeMethods.DCB> stateChange)
        {
            if (IsOpen)
            {
                Win32.NativeMethods.DCB portDCB = Win32.NativeMethods.DCB.Create(hPort);
                portDCB = stateChange(portDCB);
                if (!Win32.NativeMethods.SetCommState(hPort, ref portDCB))
                    ThrowException(String.Format("Bad com settings (hPort = {0}", hPort));
            }
        }

        /// <summary>
        /// Queues bytes for transmission. 
        /// </summary>
        /// <param name="tosend">Array of bytes to be sent</param>
        protected void Send(byte[] tosend)
        {
            uint sent = 0;
            CheckOnline();
            CheckResult();
            writeCount = tosend.GetLength(0);
            if (Win32.NativeMethods.WriteFile(hPort, tosend, (uint)writeCount, out sent, ptrUWO))
            {
                writeCount -= (int)sent;
            }
            else
            {
                if (Marshal.GetLastWin32Error() != Win32.NativeMethods.ERROR_IO_PENDING)
                    ThrowException("Send failed");
                //JH1.3:
                dataQueued = true;
            }
        }

        /// <summary>
        /// Queues a single byte for transmission.
        /// </summary>
        /// <param name="tosend">Byte to be sent</param>
        protected void Send(byte tosend)
        {
            byte[] b = new byte[1];
            b[0] = tosend;
            Send(b);
        }

        private void CheckResult()
        {
            uint sent = 0;

            //JH 1.3: Fixed a number of problems working with checkSends == false. Byte counting was unreliable because
            //occasionally GetOverlappedResult would return true with a completion having missed one or more previous
            //completions. The test for ERROR_IO_INCOMPLETE was incorrectly for ERROR_IO_PENDING instead.

            if (writeCount > 0)
            {
                if (Win32.NativeMethods.GetOverlappedResult(hPort, ptrUWO, out sent, CheckAllSends))
                {
                    if (CheckAllSends)
                    {
                        writeCount -= (int)sent;
                        if (writeCount != 0)
                            ThrowException("Send Timeout");
                        writeCount = 0;
                    }
                }
                else
                {
                    if (Marshal.GetLastWin32Error() != Win32.NativeMethods.ERROR_IO_INCOMPLETE)
                        ThrowException("Write Error");
                }
            }
        }

        /// <summary>
        /// Sends a protocol byte immediately ahead of any queued bytes.
        /// </summary>
        /// <param name="tosend">Byte to send</param>
        protected void SendImmediate(byte tosend)
        {
            CheckOnline();
            if (!Win32.NativeMethods.TransmitCommChar(hPort, tosend))
                ThrowException("Transmission failure");
        }

        /// <summary>
        /// Represents the status of the modem control input signals.
        /// </summary>
        public struct ModemStatus
        {
            private readonly uint status;
            internal ModemStatus(uint val) { status = val; }
            /// <summary>
            /// Condition of the Clear To Send signal.
            /// </summary>
            public bool Cts { get { return ((status & Win32.NativeMethods.MS_CTS_ON) != 0); } }
            /// <summary>
            /// Condition of the Data Set Ready signal.
            /// </summary>
            public bool Dsr { get { return ((status & Win32.NativeMethods.MS_DSR_ON) != 0); } }
            /// <summary>
            /// Condition of the Receive Line Status Detection signal.
            /// </summary>
            public bool Rlsd { get { return ((status & Win32.NativeMethods.MS_RLSD_ON) != 0); } }
            /// <summary>
            /// Condition of the Ring Detection signal.
            /// </summary>
            public bool Ring { get { return ((status & Win32.NativeMethods.MS_RING_ON) != 0); } }
        }

        /// <summary>
        /// Gets the status of the modem control input signals.
        /// </summary>
        /// <returns>Modem status object</returns>
        protected ModemStatus GetModemStatus()
        {
            uint f;

            CheckOnline();
            if (!Win32.NativeMethods.GetCommModemStatus(hPort, out f))
                ThrowException("Unexpected failure");
            return new ModemStatus(f);
        }

        /// <summary>
        /// Represents the current condition of the port queues.
        /// </summary>
        public struct QueueStatus
        {
            private readonly uint status;
            private readonly uint inQueue;
            private readonly uint outQueue;
            private readonly uint inQueueSize;
            private readonly uint outQueueSize;

            internal QueueStatus(uint stat, uint inQ, uint outQ, uint inQs, uint outQs)
            { status = stat; inQueue = inQ; outQueue = outQ; inQueueSize = inQs; outQueueSize = outQs; }
            /// <summary>
            /// Output is blocked by CTS handshaking.
            /// </summary>
            public bool CtsHold { get { return ((status & Win32.NativeMethods.COMSTAT.fCtsHold) != 0); } }
            /// <summary>
            /// Output is blocked by DRS handshaking.
            /// </summary>
            public bool DsrHold { get { return ((status & Win32.NativeMethods.COMSTAT.fDsrHold) != 0); } }
            /// <summary>
            /// Output is blocked by RLSD handshaking.
            /// </summary>
            public bool RlsdHold { get { return ((status & Win32.NativeMethods.COMSTAT.fRlsdHold) != 0); } }
            /// <summary>
            /// Output is blocked because software handshaking is enabled and XOFF was received.
            /// </summary>
            public bool XOffHold { get { return ((status & Win32.NativeMethods.COMSTAT.fXoffHold) != 0); } }
            /// <summary>
            /// Output was blocked because XOFF was sent and this station is not yet ready to receive.
            /// </summary>
            public bool XOffSent { get { return ((status & Win32.NativeMethods.COMSTAT.fXoffSent) != 0); } }

            /// <summary>
            /// There is a character waiting for transmission in the immediate buffer.
            /// </summary>
            public bool ImmediateWaiting { get { return ((status & Win32.NativeMethods.COMSTAT.fTxim) != 0); } }

            /// <summary>
            /// Number of bytes waiting in the input queue.
            /// </summary>
            public long InQueue { get { return (long)inQueue; } }
            /// <summary>
            /// Number of bytes waiting for transmission.
            /// </summary>
            public long OutQueue { get { return (long)outQueue; } }
            /// <summary>
            /// Total size of input queue (0 means information unavailable)
            /// </summary>
            public long InQueueSize { get { return (long)inQueueSize; } }
            /// <summary>
            /// Total size of output queue (0 means information unavailable)
            /// </summary>
            public long OutQueueSize { get { return (long)outQueueSize; } }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>String</returns>
            public override string ToString()
            {
                StringBuilder m = new StringBuilder("The reception queue is ", 60);
                if (inQueueSize == 0)
                {
                    m.Append("of unknown size and ");
                }
                else
                {
                    m.Append(inQueueSize.ToString() + " bytes long and ");
                }
                if (inQueue == 0)
                {
                    m.Append("is empty.");
                }
                else if (inQueue == 1)
                {
                    m.Append("contains 1 byte.");
                }
                else
                {
                    m.Append("contains ");
                    m.Append(inQueue.ToString());
                    m.Append(" bytes.");
                }
                m.Append(" The transmission queue is ");
                if (outQueueSize == 0)
                {
                    m.Append("of unknown size and ");
                }
                else
                {
                    m.Append(outQueueSize.ToString() + " bytes long and ");
                }
                if (outQueue == 0)
                {
                    m.Append("is empty");
                }
                else if (outQueue == 1)
                {
                    m.Append("contains 1 byte. It is ");
                }
                else
                {
                    m.Append("contains ");
                    m.Append(outQueue.ToString());
                    m.Append(" bytes. It is ");
                }
                if (outQueue > 0)
                {
                    if (CtsHold || DsrHold || RlsdHold || XOffHold || XOffSent)
                    {
                        m.Append("holding on");
                        if (CtsHold)
                            m.Append(" CTS");
                        if (DsrHold)
                            m.Append(" DSR");
                        if (RlsdHold)
                            m.Append(" RLSD");
                        if (XOffHold)
                            m.Append(" Rx XOff");
                        if (XOffSent)
                            m.Append(" Tx XOff");
                    }
                    else
                    {
                        m.Append("pumping data");
                    }
                }
                m.Append(". The immediate buffer is ");
                if (ImmediateWaiting)
                    m.Append("full.");
                else
                    m.Append("empty.");
                return m.ToString();
            }
        }

        /// <summary>
        /// Get the status of the queues
        /// </summary>
        /// <returns>Queue status object</returns>
        protected QueueStatus GetQueueStatus()
        {
            Win32.NativeMethods.COMSTAT cs;
            Win32.NativeMethods.COMMPROP cp;
            uint er;

            CheckOnline();
            if (!Win32.NativeMethods.ClearCommError(hPort, out er, out cs))
                ThrowException("Unexpected failure");
            if (!Win32.NativeMethods.GetCommProperties(hPort, out cp))
                ThrowException("Unexpected failure");
            return new QueueStatus(cs.Flags, cs.cbInQue, cs.cbOutQue, cp.dwCurrentRxQueue, cp.dwCurrentTxQueue);
        }

        // JH 1.3. Added for this version.
        /// <summary>
        /// Test if the line is congested (data being queued for send faster than it is being dequeued)
        /// This detects if baud rate is too slow or if handshaking is not allowing enough transmission
        /// time. It should be called at reasonably long fixed intervals. If data has been sent during
        /// the interval, congestion is reported if the queue was never empty during the interval.
        /// </summary>
        /// <returns>True if congested</returns>
        protected bool IsCongested()
        {
            bool e;
            if (!dataQueued)
                return false;
            lock (empty)
            { e = empty[0]; empty[0] = false; }
            dataQueued = false;
            return !e;
        }

        /// <summary>
        /// True if the RTS pin is controllable via the RTS property
        /// </summary>
        protected bool RTSavailable { get { return (stateRTS < 2); } }

        /// <summary>
        /// Set the state of the RTS modem control output
        /// </summary>
        protected bool RTS
        {
            set
            {
                if (stateRTS > 1)
                    return;

                CheckOnline();
                if (value)
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.SETRTS))
                        stateRTS = 1;
                    else
                        ThrowException("Unexpected Failure");
                }
                else
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.CLRRTS))
                        //JH 1.3: Was 1, should be 0:
                        stateRTS = 0;
                    else
                        ThrowException("Unexpected Failure");
                }
            }
            get
            {
                return (stateRTS == 1);
            }
        }

        /// <summary>
        /// True if the DTR pin is controllable via the DTR property
        /// </summary>
        protected bool DTRavailable { get { return (stateDTR < 2); } }

        /// <summary>
        /// The state of the DTR modem control output
        /// </summary>
        protected bool DTR
        {
            set
            {
                if (stateDTR > 1)
                    return;

                CheckOnline();
                if (value)
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.SETDTR))
                        stateDTR = 1;
                    else
                        ThrowException("Unexpected Failure");
                }
                else
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.CLRDTR))
                        stateDTR = 0;
                    else
                        ThrowException("Unexpected Failure");
                }
            }
            get
            {
                return (stateDTR == 1);
            }
        }

        /// <summary>
        /// Assert or remove a break condition from the transmission line
        /// </summary>
        protected bool Break
        {
            set
            {
                if (stateBRK > 1)
                    return;

                CheckOnline();
                if (value)
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.SETBREAK))
                        stateBRK = 0;
                    else
                        ThrowException("Unexpected Failure");
                }
                else
                {
                    if (Win32.NativeMethods.EscapeCommFunction(hPort, Win32.NativeMethods.CLRBREAK))
                        stateBRK = 0;
                    else
                        ThrowException("Unexpected Failure");
                }
            }
            get
            {
                return (stateBRK == 1);
            }
        }

        /// <summary>
        /// Override this to provide processing before the port is openned (i.e. to configure remote
        /// device or just check presence).
        /// </summary>
        protected virtual void BeforeOpen() { }

        /// <summary>
        /// Override this to provide processing after the port is openned (i.e. to configure remote
        /// device or just check presence).
        /// </summary>
        protected virtual void AfterOpen() { }

        /// <summary>
        /// Override this to provide processing prior to port closure.
        /// </summary>
        /// <param name="error">True if closing due to an error</param>
        protected virtual void BeforeClose(bool error) { }

        /// <summary>
        /// Override this to process received bytes.
        /// </summary>
        /// <param name="ch">The byte that was received</param>
        protected virtual void OnRxChar(byte ch) { }

        /// <summary>
        /// Override this to take action when transmission is complete (i.e. all bytes have actually
        /// been sent, not just queued).
        /// </summary>
        protected virtual void OnTxDone() { }

        /// <summary>
        /// Override this to take action when a break condition is detected on the input line.
        /// </summary>
        protected virtual void OnBreak() { }

        //JH 1.3: Deleted OnRing() which was never called: use OnStatusChange instead (Thanks Jim Foster)

        /// <summary>
        /// Override this to take action when one or more modem status inputs change state
        /// </summary>
        /// <param name="mask">The status inputs that have changed state</param>
        /// <param name="state">The state of the status inputs</param>
        protected virtual void OnStatusChange(ModemStatus mask, ModemStatus state) { }

        /// <summary>
        /// Override this to take action when the reception thread closes due to an exception being thrown.
        /// </summary>
        /// <param name="e">The exception which was thrown</param>
        protected virtual void OnRxException(Exception e) { }

        //TK 1.4: Added disposable pattern
        /// <summary>
        /// Override this to take action when the object disposed
        /// </summary>
        protected virtual void OnDisposed() { }

        private void ReceiveThread()
        {
            byte[] buf = new Byte[1];
            uint gotbytes;
            bool starting;

            starting = true;
            using (AutoResetEvent waitCommEvent = new AutoResetEvent(false))
            {
                Win32.NativeMethods.OVERLAPPED ov = new Win32.NativeMethods.OVERLAPPED();
                IntPtr unmanagedOv;
                IntPtr uMask;
                uint eventMask = 0;
                unmanagedOv = Marshal.AllocHGlobal(Marshal.SizeOf(ov));
                uMask = Marshal.AllocHGlobal(Marshal.SizeOf(eventMask));
                ov.Offset = 0;
                ov.OffsetHigh = 0;
                ov.hEvent = waitCommEvent.Handle;
                Marshal.StructureToPtr(ov, unmanagedOv, true);
                try
                {
                    while (true)
                    {
                        if (!Win32.NativeMethods.SetCommMask(hPort, Win32.NativeMethods.EV_RXCHAR | Win32.NativeMethods.EV_TXEMPTY | Win32.NativeMethods.EV_CTS | Win32.NativeMethods.EV_DSR | Win32.NativeMethods.EV_BREAK | Win32.NativeMethods.EV_RLSD | Win32.NativeMethods.EV_RING | Win32.NativeMethods.EV_ERR))
                        {
                            throw new CommPortException("IO Error [001]");
                        }
                        Marshal.WriteInt32(uMask, 0);
                        //JH 1.2: Tells the main thread that this thread is ready for action.
                        if (starting)
                        {
                            startEvent.Set();
                            starting = false;
                        }
                        if (!Win32.NativeMethods.WaitCommEvent(hPort, uMask, unmanagedOv))
                        {
                            if (Marshal.GetLastWin32Error() == Win32.NativeMethods.ERROR_IO_PENDING)
                            {
                                waitCommEvent.WaitOne();
                            }
                            else
                            {
                                throw new CommPortException("IO Error [002]");
                            }
                        }
                        eventMask = (uint)Marshal.ReadInt32(uMask);
                        if ((eventMask & Win32.NativeMethods.EV_ERR) != 0)
                        {
                            UInt32 errs;
                            if (Win32.NativeMethods.ClearCommError(hPort, out errs, IntPtr.Zero))
                            {
                                //JH 1.2: BREAK condition has an error flag and and an event flag. Not sure if both
                                //are always raised, so if CE_BREAK is only error flag ignore it and set the EV_BREAK
                                //flag for normal handling. Also made more robust by handling case were no recognised
                                //error was present in the flags. (Thanks to Fred Pittroff for finding this problem!)
                                int ec = 0;
                                StringBuilder s = new StringBuilder("UART Error: ", 40);
                                if (((errs & Win32.NativeMethods.CE_FRAME) != 0) &&
                                    !IgnoreErrorFraming)
                                {
                                    s = s.Append("Framing,");
                                    ec++;
                                }
                                if ((errs & Win32.NativeMethods.CE_IOE) != 0)
                                {
                                    s = s.Append("IO,");
                                    ec++;
                                }
                                if (((errs & Win32.NativeMethods.CE_OVERRUN) != 0) &&
                                    !IgnoreErrorOverrun)
                                {
                                    s = s.Append("Overrun,");
                                    ec++;
                                }
                                if ((errs & Win32.NativeMethods.CE_RXOVER) != 0)
                                {
                                    s = s.Append("Receive Overflow,");
                                    ec++;
                                }
                                if (((errs & Win32.NativeMethods.CE_RXPARITY) != 0) &&
                                    !IgnoreErrorParity)
                                {
                                    s = s.Append("Parity,");
                                    ec++;
                                }
                                if ((errs & Win32.NativeMethods.CE_TXFULL) != 0)
                                {
                                    s = s.Append("Transmit Overflow,");
                                    ec++;
                                }
                                if (ec > 0)
                                {
                                    s.Length = s.Length - 1;
                                    throw new CommPortException(s.ToString());
                                }
                                else
                                {
                                    if (errs == Win32.NativeMethods.CE_BREAK)
                                    {
                                        eventMask |= Win32.NativeMethods.EV_BREAK;
                                    }
                                    //else
                                    //{
                                    //    throw new CommPortException("IO Error [003]");
                                    //}
                                }
                            }
                            else
                            {
                                throw new CommPortException("IO Error [003]");
                            }
                        }
                        if ((eventMask & Win32.NativeMethods.EV_RXCHAR) != 0)
                        {
                            do
                            {
                                gotbytes = 0;
                                if (!Win32.NativeMethods.ReadFile(hPort, buf, 1, out gotbytes, unmanagedOv))
                                {
                                    //JH 1.1: Removed ERROR_IO_PENDING handling as comm timeouts have now
                                    //been set so ReadFile returns immediately. This avoids use of CancelIo
                                    //which was causing loss of data. Thanks to Daniel Moth for suggesting this
                                    //might be a problem, and to many others for reporting that it was!
                                    //TK 1.4: Added handling ERROR_IO_PENDING
                                    int lastWin32Error = Marshal.GetLastWin32Error();
                                    if (lastWin32Error == Win32.NativeMethods.ERROR_IO_PENDING)
                                    {
                                        waitCommEvent.WaitOne();
                                        if (!Win32.NativeMethods.GetOverlappedResult(hPort, unmanagedOv, out gotbytes, false))
                                        {
                                            lastWin32Error = Marshal.GetLastWin32Error();
                                            throw new CommPortException(String.Format("IO Error [004/{0}]", lastWin32Error),
                                                new System.ComponentModel.Win32Exception(lastWin32Error));
                                        }
                                    }
                                    else
                                        throw new CommPortException(String.Format("IO Error [005/{0}] (got bytes {1})", lastWin32Error, gotbytes),
                                            new System.ComponentModel.Win32Exception(lastWin32Error));
                                }
                                if (gotbytes == 1)
                                    OnRxChar(buf[0]);
                            }
                            while (gotbytes > 0);
                        }
                        if ((eventMask & Win32.NativeMethods.EV_TXEMPTY) != 0)
                        {
                            //JH1.3:
                            lock (empty)
                                empty[0] = true;
                            OnTxDone();
                        }
                        if ((eventMask & Win32.NativeMethods.EV_BREAK) != 0)
                            OnBreak();
                        uint i = 0;
                        if ((eventMask & Win32.NativeMethods.EV_CTS) != 0)
                            i |= Win32.NativeMethods.MS_CTS_ON;
                        if ((eventMask & Win32.NativeMethods.EV_DSR) != 0)
                            i |= Win32.NativeMethods.MS_DSR_ON;
                        if ((eventMask & Win32.NativeMethods.EV_RLSD) != 0)
                            i |= Win32.NativeMethods.MS_RLSD_ON;
                        if ((eventMask & Win32.NativeMethods.EV_RING) != 0)
                            i |= Win32.NativeMethods.MS_RING_ON;
                        if (i != 0)
                        {
                            uint f;
                            if (!Win32.NativeMethods.GetCommModemStatus(hPort, out f))
                                throw new CommPortException("IO Error [005]");
                            OnStatusChange(new ModemStatus(i), new ModemStatus(f));
                        }
                    }
                }
                catch (Exception e)
                {
                    //JH 1.3: Added for shutdown robustness (Thanks to Fred Pittroff, Mark Behner and Kevin Williamson!), .
                    Win32.NativeMethods.CancelIo(hPort);
                    if (!(e is ThreadAbortException))
                    {
                        rxException = e;
                        OnRxException(e);
                    }
                }
                finally
                {
                    if (uMask != IntPtr.Zero)
                        Marshal.FreeHGlobal(uMask);
                    if (unmanagedOv != IntPtr.Zero)
                        Marshal.FreeHGlobal(unmanagedOv);
                }
            }
        }

        private bool CheckOnline()
        {
            CheckDisposed();
            if ((rxException != null) && (!rxExceptionReported))
            {
                rxExceptionReported = true;
                ThrowException("rx");
            }
            if (IsOpen)
            {
                //JH 1.1: Avoid use of GetHandleInformation for W98 compatability.                
                if (hPort != (System.IntPtr)Win32.NativeMethods.INVALID_HANDLE_VALUE)
                    return true;
                ThrowException("Offline");
                return false;
            }
            else
            {
                ThrowException("Offline");
                return false;
            }
        }
    }

}
