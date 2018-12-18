using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SnT.IO.Ports.Utils
{
    public static class PortHelper
    {
        #region Static methods

        //JH 1.3: Added AltName function, PortStatus enum and IsPortAvailable function.
        /// <summary>
        /// Returns the alternative name of a com port i.e. \\.\COM1 for COM1:
        /// Some systems require this form for double or more digit com port numbers.
        /// </summary>
        /// <param name="s">Name in form COM1 or COM1:</param>
        /// <returns>Name in form \\.\COM1</returns>
        internal static string AltName(string s)
        {
            string r = s.Trim();
            if (s.EndsWith(":")) s = s.Substring(0, s.Length - 1);
            if (s.StartsWith(@"\")) return s;
            return @"\\.\" + s;
        }

        /// <summary>
        /// Tests the availability of a named comm port.
        /// </summary>
        /// <param name="s">Name of port</param>
        /// <returns>Availability of port</returns>
        public static PortStatus IsPortAvailable(string s)
        {
            IntPtr h = Win32.NativeMethods.CreateFile(s, Win32.NativeMethods.GENERIC_READ | Win32.NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero,
                Win32.NativeMethods.OPEN_EXISTING, Win32.NativeMethods.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
            if (h == (IntPtr)Win32.NativeMethods.INVALID_HANDLE_VALUE)
            {
                if (Marshal.GetLastWin32Error() == Win32.NativeMethods.ERROR_ACCESS_DENIED)
                {
                    return PortStatus.Unavailable;
                }
                else
                {
                    //JH 1.3: Automatically try AltName if supplied name fails:
                    h = Win32.NativeMethods.CreateFile(AltName(s), Win32.NativeMethods.GENERIC_READ | Win32.NativeMethods.GENERIC_WRITE, 0, IntPtr.Zero,
                        Win32.NativeMethods.OPEN_EXISTING, Win32.NativeMethods.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                    if (h == (IntPtr)Win32.NativeMethods.INVALID_HANDLE_VALUE)
                    {
                        if (Marshal.GetLastWin32Error() == Win32.NativeMethods.ERROR_ACCESS_DENIED)
                        {
                            return PortStatus.Unavailable;
                        }
                        else
                        {
                            return PortStatus.Absent;
                        }
                    }
                }
            }
            Win32.NativeMethods.CloseHandle(h);
            return PortStatus.Available;
        }

        public static string[] GetPortNames()
        {
            List<string> portNames = new List<string>();
            using (RegistryKey serialCom =
                Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                foreach (string valueName in serialCom.GetValueNames())
                {
                    object value = serialCom.GetValue(valueName);
                    if ((value != null) && !String.IsNullOrEmpty(value.ToString()))
                        portNames.Add(value.ToString());
                }
                
                return portNames.ToArray();
            }
        }

        #endregion

    }
}
