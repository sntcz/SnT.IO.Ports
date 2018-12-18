using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SnT.IO.Ports.TestConsole
{
    class Program
    {
        static bool exit = false;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            string portName = "COM1";
            Console.Write("Enter port name [{0}]:", portName);
            string input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input) && !String.IsNullOrEmpty(input.Trim()))
            {
                int n = 0;
                if (Int32.TryParse(input, out n))
                    portName = String.Format("COM{0}", n);
                else
                    portName = input;
            }

            try
            {
                Console.WriteLine("Main loop - wait for Ctrl+C");
                using (SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One))
                {
                    port.ByteReceived += new EventHandler<SerialByteReceivedEventArgs>(port_ByteReceived);
                    port.Open();
                    Console.WriteLine(" ... Opened ({0} {1}) ({2})", port.PortName, port.BaudRate, DateTime.Now);
                    while (!exit)
                    {
                        input = Console.ReadLine();
                        port.Write(Encoding.Default.GetBytes(input));
                        port.Write(Encoding.Default.GetBytes(Environment.NewLine));
                    }

                    Console.WriteLine("\r\n ... Port Closed ({0})", DateTime.Now);
                    port.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey(true);
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("<CTRL+C> pressed, please wait ...");
            exit = true;
        }

        static void port_ByteReceived(object sender, SerialByteReceivedEventArgs e)
        {
            string s = Encoding.Default.GetString(new byte[] { e.Data });
            Console.Write(s);
        }
    }
}
