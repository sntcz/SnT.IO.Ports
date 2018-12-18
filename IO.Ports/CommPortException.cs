using System;

namespace SnT.IO.Ports
{
    /// <summary>
    /// Exception used for all errors.
    /// </summary>
    public class CommPortException : ApplicationException
    {
        /// <summary>
        /// Constructor for raising direct exceptions
        /// </summary>
        /// <param name="message">Description of error</param>
        public CommPortException(string message) 
            : base(message) { }

        /// <summary>
        /// Constructor for re-raising exceptions from receive thread
        /// </summary>
        /// <param name="e">Inner exception raised on receive thread</param>
        public CommPortException(Exception e) 
            : base("Receive Thread Exception", e) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Description of error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public CommPortException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
