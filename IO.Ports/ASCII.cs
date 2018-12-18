using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports
{
    //JH 1.3: Corrected STH -> STX (Thanks - Johan Thelin!)
    /// <summary>
    /// Byte type with enumeration constants for ASCII control codes.
    /// </summary>
    public enum ASCII : byte
    {
        /// <summary>
        /// Null char
        /// </summary>
        NULL = 0x00,
        /// <summary>
        /// Start of header
        /// </summary>
        SOH = 0x01,
        /// <summary>
        /// Start TX
        /// </summary>
        STX = 0x02,
        /// <summary>
        /// End TX
        /// </summary>
        ETX = 0x03,
        /// <summary>
        /// End of tranfmission
        /// </summary>
        EOT = 0x04,
        /// <summary>
        /// Enquiry
        /// </summary>
        ENQ = 0x05,
        /// <summary>
        /// Acknowledge
        /// </summary>
        ACK = 0x06,
        /// <summary>
        /// Bell
        /// </summary>
        BELL = 0x07,
        /// <summary>
        /// Backspace
        /// </summary>
        BS = 0x08,
        /// <summary>
        /// Horizontal tab (same AS TAB)
        /// </summary>
        HT = 0x09,
        /// <summary>
        /// Horizontal tab (same as HT)
        /// </summary>
        TAB = 0x09,
        /// <summary>
        /// Line feed
        /// </summary>
        LF = 0x0A,
        /// <summary>
        /// Vertical tab
        /// </summary>
        VT = 0x0B,
        /// <summary>
        /// Form feed
        /// </summary>
        FF = 0x0C,
        /// <summary>
        /// Carriage return
        /// </summary>
        CR = 0x0D,
        /// <summary>
        /// Shift out
        /// </summary>
        SO = 0x0E,
        /// <summary>
        /// Shift in
        /// </summary>
        SI = 0x0F,
        /// <summary>
        /// Data link escape
        /// </summary>
        DLE = 0x10,
        /// <summary>
        /// Device control 1
        /// </summary>
        DC1 = 0x11,
        /// <summary>
        /// Device control 2
        /// </summary>
        DC2 = 0x12,
        /// <summary>
        /// Device control 3
        /// </summary>
        DC3 = 0x13,
        /// <summary>
        /// Device control 4
        /// </summary>
        DC4 = 0x14,
        /// <summary>
        /// Not acknowledge
        /// </summary>
        NAK = 0x15,
        /// <summary>
        /// Synchronous idle
        /// </summary>
        SYN = 0x16,
        /// <summary>
        /// End of transmission block
        /// </summary>
        ETB = 0x17,
        /// <summary>
        /// Cancel
        /// </summary>
        CAN = 0x18,
        /// <summary>
        /// Enf of medium
        /// </summary>
        EM = 0x19,
        /// <summary>
        /// Substitute
        /// </summary>
        SUB = 0x1A,
        /// <summary>
        /// Escape
        /// </summary>
        ESC = 0x1B,
        /// <summary>
        /// File separator
        /// </summary>
        FS = 0x1C,
        /// <summary>
        /// Group separator
        /// </summary>
        GS = 0x1D,
        /// <summary>
        /// Record separator
        /// </summary>
        RS = 0x1E,
        /// <summary>
        /// Unit separator
        /// </summary>
        US = 0x1F,
        /// <summary>
        /// Space
        /// </summary>
        SP = 0x20,
        /// <summary>
        /// Delete
        /// </summary>
        DEL = 0x7F,
        /// <summary>
        /// 0xFF (255) char
        /// </summary>
        CHAR_FF = 0xFF
    }
}
