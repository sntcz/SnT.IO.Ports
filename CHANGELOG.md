# Road map

- [x] SerialPort class with same API as System.IO.Ports.SerialPort
- [ ] Nothing


# Change log

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## Changes
### 1.?
 * PB: CommBase.cs - Change Parity of opened port
 * PB: SerialPort.cs - WriteImmediate
 * TK: CommBase.cs - Change comm port properties of opened Port
 * TK: CommBase.cs - Refaktoring of variable names.
### 1.6.6.1
 * TK: CommBase.cs - Append got bytes count to error message 005
### 1.6.6
 * TK: CommBase.cs - Append hPort to error message
 * TK: CommBase.cs - IgnoreErrorFraming, IgnoreErrorOverrun, IgnoreErrorParity
### 1.6.5
 * TK: SerialPort.cs - Virtual call in constructor removed
 * TK: SerialPort.cs - ByteReceived event
### 1.6.4
 * TK: NativeMethods.cs - Initialize port DCB by GetCommState
 * TK: CommBase.cs - Fixed inner exception in ThrowException
### 1.6.3.1
 * TK: CommBase.cs - ThrowException check LastDllError and create inner exception
### 1.6.3
 * TK: CommBase.cs - Worker thread marked as background thread
 * TK: ASCII.cs - Added 0xFF char
 * TK: ASCII.cs - Added TAB (same as HT)
 * TK: ASCII.cs - Added summary
### 1.6.2
 * TK: SerialPort.cs - RtsEnable, DtrEnable (improved compatibility with system SerialPort)
### 1.6.1
 * TK: SerialPort.cs - Added Handshake settings (improved compatibility with system SerialPort)
 * TK: PortHelper.cs - Resolved bug NullReferenceException in GetPortNames()
### 1.6 (2013-08-27)
 * TK: SerialPort.cs - Added ReplaceBytes dictionary
### 1.5 (2013-07-03)
 * TK: Removed unused references
### 1.4 (2009-08-05)
 * TK: Version 1.4 (2009-08-05) changes labelled thus.
### 1.3
 * JH: Version 1.3 changes labelled thus.
### 1.2
 * JH: Version 1.2 changes labelled thus.
### 1.1
 * JH: Version 1.1 changes labelled thus.

