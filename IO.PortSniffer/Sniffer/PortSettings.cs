using System;
using System.Collections.Generic;
using System.Text;
using SnT.IO.Ports;
using SnT.IO.Ports.Base;
using System.ComponentModel;
using SnT.IO.PortSniffer.Metadata;

namespace SnT.IO.PortSniffer.Sniffer
{
    public class PortSettings
    {
        /// <summary>
        /// Gets or sets the port for communications, 
        /// including but not limited to all available COM ports.
        /// </summary>
        [TypeConverter(typeof(PortNameTypeConverter))]
        [Description("The port for communications, including but not limited to all available COM ports")]
        [Category("Basic")]
        public string PortName { get; set; }
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        [TypeConverter(typeof(BaudRateTypeConverter))]
        [Description("The serial baud rate. (default: 9600)")]
        [Category("Basic")]
        [DefaultValue(9600)]
        public int BaudRate { get; set; }
        /// <summary>
        /// Gets or sets the parity-checking protocol. 
        /// (default: none)
        /// </summary>
        [Category("Basic")]
        [DefaultValue(Parity.None)]
        [Description("The parity-checking protocol. (default: none)")]
        public Parity Parity { get; set; }
        /// <summary>
        ///  Gets or sets the standard length of data bits per byte.
        /// </summary>
        [TypeConverter(typeof(DataBitsTypeConverter))]
        [Category("Basic")]
        [DefaultValue(8)]
        [Description("The standard length of data bits per byte. (default: 8)")]
        public int DataBits { get; set; }
        /// <summary>
        /// Gets or sets the number of stop bits. 
        /// (default: one)
        /// </summary>
        [Category("Basic")]
        [DefaultValue(StopBits.One)]
        [Description("If true, transmission is halted unless CTS is asserted by the remote station (default: false)")]
        public StopBits StopBits { get; set; }
        /// <summary>
        /// If true, transmission is halted unless CTS is asserted by the remote station 
        /// (default: false)
        /// </summary>
        [DefaultValue(false)]
        [Description("If true, transmission is halted unless DSR is asserted by the remote station (default: false)")]
        public bool TxFlowCTS { get; set; }
        /// <summary>
        /// If true, transmission is halted unless DSR is asserted by the remote station 
        /// (default: false)
        /// </summary>
        [DefaultValue(false)]
        [Description("If true, transmission is halted unless DSR is asserted by the remote station (default: false)")]
        public bool TxFlowDSR { get; set; }
        /// <summary>
        /// If true, transmission is halted when Xoff is received and restarted when Xon is received 
        /// (default: false)
        /// </summary>
        [DefaultValue(false)]
        [Description("If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)")]
        public bool TxFlowX { get; set; }
        /// <summary>
        /// If false, transmission is suspended when this station has sent Xoff to the remote station 
        /// (default: true)
        /// Set false if the remote station treats any character as an Xon.
        /// </summary>
        [DefaultValue(true)]
        [Description("If false, transmission is suspended when this station has sent Xoff " 
            + "to the remote station. Set false if the remote station treats any character "
            + "as an Xon. (default: true)")]
        public bool TxWhenRxXoff { get; set; }
        /// <summary>
        /// If true, received characters are ignored unless DSR is asserted by the remote station 
        /// (default: false)
        /// </summary>
        [DefaultValue(false)]
        [Description("If true, received characters are ignored unless DSR is asserted by the remote station (default: false)")]
        public bool RxGateDSR { get; set; }
        /// <summary>
        /// If true, Xon and Xoff characters are sent to control the data flow from the remote station 
        /// (default: false)
        /// </summary>
        [DefaultValue(false)]
        [Description(" If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)")]
        public bool RxFlowX { get; set; }
        /// <summary>
        /// Specifies the use to which the RTS output is put (default: none)
        /// </summary>
        [DefaultValue(HSOutput.None)]
        [Description("Specifies the use to which the RTS output is put (default: none)")]
        public HSOutput UseRTS { get; set; }
        /// <summary>
        /// Specifies the use to which the DTR output is put (default: none)
        /// </summary>
        [DefaultValue(HSOutput.None)]
        [Description("Specifies the use to which the DTR output is put (default: none)")]
        public HSOutput UseDTR { get; set; }
        /// <summary>
        /// The character used to signal Xon for X flow control (default: DC1)
        /// </summary>
        [DefaultValue(ASCII.DC1)]
        [Description("The character used to signal Xon for X flow control (default: DC1)")]
        public ASCII XonChar { get; set; }
        /// <summary>
        /// The character used to signal Xoff for X flow control (default: DC3)
        /// </summary>
        [DefaultValue(ASCII.DC3)]
        [Description("The character used to signal Xoff for X flow control (default: DC3)")]
        public ASCII XoffChar { get; set; }
        //JH 1.2: Next two defaults changed to 0 to use new defaulting mechanism dependant on queue size.
        /// <summary>
        /// The number of free bytes in the reception queue at which flow is disabled
        /// (Default: 0 = Set to 1/10th of actual rxQueue size)
        /// </summary>
        [DefaultValue(0)]
        [Description("The number of free bytes in the reception queue at which flow is disabled (Default: 0 = Set to 1/10th of actual rxQueue size)")]
        public int RxHighWater { get; set; }
        /// <summary>
        /// The number of bytes in the reception queue at which flow is re-enabled
        /// (Default: 0 = Set to 1/10th of actual rxQueue size)
        /// </summary>
        [DefaultValue(0)]
        [Description("Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0 = No timeout)")]
        public int RxLowWater { get; set; }
        /// <summary>
        /// Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant
        /// (default: 0 = No timeout)
        /// </summary>
        [DefaultValue(0u)]
        [Description("Constant. Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)")]
        public uint SendTimeoutMultiplier { get; set; }
        /// <summary>
        /// Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant 
        /// (default: 0)
        /// </summary>
        [DefaultValue(0u)]
        [Description("Constant. Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)")]
        public uint SendTimeoutConstant { get; set; }
        /// <summary>
        /// Requested size for receive queue 
        /// (default: 0 = use operating system default)
        /// </summary>
        [DefaultValue(0)]
        [Description("Requested size for receive queue (default: 0 = use operating system default)")]
        public int RxQueue { get; set; }
        /// <summary>
        /// Requested size for transmit queue 
        /// (default: 0 = use operating system default)
        /// </summary>
        [DefaultValue(0)]
        [Description("Requested size for transmit queue (default: 0 = use operating system default)")]
        public int TxQueue { get; set; }
        /// <summary>
        /// If true, subsequent Send commands wait for completion of earlier ones enabling the results
        /// to be checked. If false, errors, including timeouts, may not be detected, but performance
        /// may be better.
        /// </summary>
        [DefaultValue(true)]
        [Description("If true, subsequent Send commands wait for completion of earlier " 
            + "ones enabling the results to be checked. If false, errors, including "
            + "timeouts, may not be detected, but performance may be better.")]
        public bool CheckAllSends { get; set; }

        public PortSettings()
        {
            LoadDefaults();
        }

        public void FromPort(CommBase port)
        {
            PortName = port.PortName;
            BaudRate = port.BaudRate;
            Parity = port.Parity;
            DataBits = port.DataBits;
            StopBits = port.StopBits;
            TxFlowCTS = port.TxFlowCTS;
            TxFlowDSR = port.TxFlowDSR;
            TxFlowX = port.TxFlowX;
            TxWhenRxXoff = port.TxWhenRxXoff;
            RxGateDSR = port.RxGateDSR;
            RxFlowX = port.RxFlowX;
            UseRTS = port.UseRTS;
            UseDTR = port.UseDTR;
            XonChar = port.XonChar;
            XoffChar = port.XoffChar;
            RxHighWater = port.RxHighWater;
            RxLowWater = port.RxLowWater;
            SendTimeoutMultiplier = port.SendTimeoutMultiplier;
            SendTimeoutConstant = port.SendTimeoutConstant;
            RxQueue = port.RxQueue;
            TxQueue = port.TxQueue;
            CheckAllSends = port.CheckAllSends;
        }

        public void ToPort(CommBase port)
        {
            port.PortName = PortName;
            port.BaudRate = BaudRate;
            port.Parity = Parity;
            port.DataBits = DataBits;
            port.StopBits = StopBits;
            port.TxFlowCTS = TxFlowCTS;
            port.TxFlowDSR = TxFlowDSR;
            port.TxFlowX = TxFlowX;
            port.TxWhenRxXoff = TxWhenRxXoff;
            port.RxGateDSR = RxGateDSR;
            port.RxFlowX = RxFlowX;
            port.UseRTS = UseRTS;
            port.UseDTR = UseDTR;
            port.XonChar = XonChar;
            port.XoffChar = XoffChar;
            port.RxHighWater = RxHighWater;
            port.RxLowWater = RxLowWater;
            port.SendTimeoutMultiplier = SendTimeoutMultiplier;
            port.SendTimeoutConstant = SendTimeoutConstant;
            port.RxQueue = RxQueue;
            port.TxQueue = TxQueue;
            port.CheckAllSends = CheckAllSends;
        }

        public void Save(System.IO.Stream stream)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());
            x.Serialize(stream, this);
        }

        public void Load(System.IO.Stream stream)
        {
            PortSettings port = InternalLoad(stream);
            PortName = port.PortName;
            BaudRate = port.BaudRate;
            Parity = port.Parity;
            DataBits = port.DataBits;
            StopBits = port.StopBits;
            TxFlowCTS = port.TxFlowCTS;
            TxFlowDSR = port.TxFlowDSR;
            TxFlowX = port.TxFlowX;
            TxWhenRxXoff = port.TxWhenRxXoff;
            RxGateDSR = port.RxGateDSR;
            RxFlowX = port.RxFlowX;
            UseRTS = port.UseRTS;
            UseDTR = port.UseDTR;
            XonChar = port.XonChar;
            XoffChar = port.XoffChar;
            RxHighWater = port.RxHighWater;
            RxLowWater = port.RxLowWater;
            SendTimeoutMultiplier = port.SendTimeoutMultiplier;
            SendTimeoutConstant = port.SendTimeoutConstant;
            RxQueue = port.RxQueue;
            TxQueue = port.TxQueue;
            CheckAllSends = port.CheckAllSends;
        }

        private static PortSettings InternalLoad(System.IO.Stream stream)
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(PortSettings));
            return x.Deserialize(stream) as PortSettings;
        }

        //http://stackoverflow.com/questions/5882164/xml-serialization-and-defaultvalue-related-problem-in-c-sharp
        private void LoadDefaults()
        {
            try
            {
                //Iterate through properties
                foreach (var property in GetType().GetProperties())
                {
                    //Iterate through attributes of this property
                    foreach (Attribute attr in property.GetCustomAttributes(true))
                    {
                        //does this property have [DefaultValueAttribute]?
                        if (attr is DefaultValueAttribute)
                        {
                            //So lets try to load default value to the property
                            DefaultValueAttribute dv = (DefaultValueAttribute)attr;
                            try
                            {
                                //Is it an array?
                                if (property.PropertyType.IsArray)
                                {
                                    //Use set value for arrays
                                    property.SetValue(this, null, (object[])dv.Value);
                                }
                                else
                                {
                                    //Use set value for.. not arrays
                                    property.SetValue(this, dv.Value, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
