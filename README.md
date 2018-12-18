# Serial Port

A complete wrapper for the Win32 Waitable Events API before stumbling on 
the ManualResetEvent and AutoResetEvent framework classes that already 
encapsulated all the functionality.. 

Class with the similar/same interface as System.IO.Port.SerialPort built
on top wrapper.

Original source: http://msdn.microsoft.com/en-us/magazine/cc301786.aspx
Original author: John Hind (John.Hind@zen.co.uk)
Original code download: http://download.microsoft.com/download/8/3/f/83f69587-47f1-48e2-86a6-aab14f01f1fe/NetSerialComm.exe


---------------------------------------

See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Serial port wrapper for the Win32 API
    * CommBase
    * CommLine
    * CommString
- SerialPort class


### Serial port wrapper for the Win32 API
#### CommBase
Lowest level Com driver handling all Win32 API calls and processing 
send and receive in terms of individual bytes. Used as a base class 
for higher level drivers.

#### CommLine
Overlays CommBase to provide line or packet oriented communications 
to derived classes. Strings are sent and received and the Transact 
method is added which transmits a string and then blocks until
a reply string has been received (subject to a timeout).

#### CommString
Overlays CommBase to provide string oriented communications to derived classes. 
Strings are sent and received.

### SerialPort class
Overlays CommBase to provide class with the same API as System.IO.Port.SerialPort.


## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.


## License
Dual licensed, see [LICENSE.dm](LICENSE.md)