using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SnT.IO.Ports.Utils;

namespace SnT.IO.Ports.Base
{
    /// <summary>
    /// Overlays CommBase to provide string oriented communications to derived classes. 
    /// Strings are sent and received.
    /// </summary>
    public abstract class CommString : CommBase
    {
        private readonly ByteArray rxBuffer = new ByteArray();

        #region Properties

        public Encoding Encoding { get; set; }

        private string rxPattern;
        private Regex regex;
        public string RxPattern
        {
            get { return rxPattern; }
            set
            {
                rxPattern = value;
                if (String.IsNullOrEmpty(rxPattern))
                {
                    regex = null;
                }
                else
                {
                    regex = new Regex(
                        rxPattern,
                        RegexOptions.IgnoreCase
                        | RegexOptions.CultureInvariant
                        | RegexOptions.IgnorePatternWhitespace
                        | RegexOptions.Compiled
                        );
                }
            }
        }

        #endregion

        #region ...ctors

        public CommString()
            : base()
        {
            Initialize();
        }

        public CommString(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Initialize();
        }

        private void Initialize()
        {
            //RxPattern = "(?<CODE>.*)\\r\\n";
            Encoding = System.Text.Encoding.ASCII;
        }

        #endregion

        /// <summary>
        /// Queue the string for transmission.
        /// </summary>
        /// <param name="toSend">String to be sent.</param>
        protected void Send(string toSend)
        {
            Send(Encoding.GetBytes(toSend));
        }

        /// <summary>
        /// Override this to process input strings.
        /// </summary>
        /// <param name="s">String containing the received ASCII text.</param>
        protected virtual void OnRxString(string s) { }

        /// <summary>
        /// Overrides OnRxChar to process received bytes.
        /// </summary>
        /// <param name="ch">The byte that was received.</param>
        protected override void OnRxChar(byte ch)
        {
            rxBuffer.Append(ch);

            if (regex != null)
            {
                Match match = regex.Match(rxBuffer.GetString(Encoding));
                if (match.Success)
                {
                    while (match.Success)
                    {
                        OnRxString(match.Value);
                        match = match.NextMatch();
                    }
                    rxBuffer.Clear();
                } // if (match.Success)
            }
            else
            {
                OnRxString(rxBuffer.GetString(Encoding));
                rxBuffer.Clear();
            }

        }

    }
}
