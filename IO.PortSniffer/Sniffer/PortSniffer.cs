using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.PortSniffer.Sniffer
{
    class PortSniffer : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public PortSettings LeftSettings { get; private set; }
        public PortSettings RightSettings { get; private set; }
        public DataCollection Data { get; private set; }

        public event EventHandler<DataRowChangedEventArgs> DataRowChanged;
        public event EventHandler DataCleared;

        private readonly CommSniffer leftPort;
        private readonly CommSniffer rightPort;

        public PortSniffer()
        {
            IsDisposed = false;
            LeftSettings = new PortSettings();
            RightSettings = new PortSettings();
            Data = new DataCollection();
            Data.DataRowChanged += new EventHandler<DataRowChangedEventArgs>(Data_DataRowChanged);
            Data.DataCleared += new EventHandler(Data_Cleared);
            leftPort = new CommSniffer();
            rightPort = new CommSniffer();
            LeftSettings.FromPort(leftPort);
            RightSettings.FromPort(rightPort);
            leftPort.ErrorReceived += new EventHandler<Ports.SerialErrorReceivedEventArgs>(port_ErrorReceived);
            rightPort.ErrorReceived += new EventHandler<Ports.SerialErrorReceivedEventArgs>(port_ErrorReceived);
            leftPort.DataReceived += new EventHandler<ByteReceivedEventArgs>(leftPort_DataReceived);
            rightPort.DataReceived += new EventHandler<ByteReceivedEventArgs>(rightPort_DataReceived);
        }

        void Data_Cleared(object sender, EventArgs e)
        {
            if (DataCleared != null)
                DataCleared(sender, EventArgs.Empty);
        }

        void Data_DataRowChanged(object sender, DataRowChangedEventArgs e)
        {
            if (DataRowChanged != null)
                DataRowChanged(sender, e);
        }

        void rightPort_DataReceived(object sender, ByteReceivedEventArgs e)
        {
            Data.Append(Direction.RightToLeft, e.Data);
            leftPort.Write(e.Data);            
        }

        void leftPort_DataReceived(object sender, ByteReceivedEventArgs e)
        {
            Data.Append(Direction.LeftToRight, e.Data);
            rightPort.Write(e.Data);
        }

        void port_ErrorReceived(object sender, Ports.SerialErrorReceivedEventArgs e)
        {
            Close();            
        }

        ~PortSniffer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                leftPort.Close();
                rightPort.Close();
                leftPort.ErrorReceived -= new EventHandler<Ports.SerialErrorReceivedEventArgs>(port_ErrorReceived);
                rightPort.ErrorReceived -= new EventHandler<Ports.SerialErrorReceivedEventArgs>(port_ErrorReceived);
                leftPort.DataReceived -= new EventHandler<ByteReceivedEventArgs>(leftPort_DataReceived);
                rightPort.DataReceived -= new EventHandler<ByteReceivedEventArgs>(rightPort_DataReceived);

                if (disposing)
                {
                    leftPort.Dispose();
                    rightPort.Dispose();
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true); ;
        }

        #endregion

        public void Open()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("PortSniffer");
            LeftSettings.ToPort(leftPort);
            leftPort.Open();
            RightSettings.ToPort(rightPort);
            rightPort.Open();
        }

        public void Close()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("PortSniffer");
            leftPort.Close();
            rightPort.Close();
        }

        public void Clear()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("PortSniffer");
            Data.Clear();
        }

        public void Save(System.IO.Stream stream)
        {
            Data.Save(stream);
        }

        public void SaveSettings(string folder)
        {
            using (System.IO.Stream stream = 
                System.IO.File.Create(System.IO.Path.Combine(folder, "LeftPort.xml")))
                LeftSettings.Save(stream);
            using (System.IO.Stream stream =
                System.IO.File.Create(System.IO.Path.Combine(folder, "RightPort.xml")))
                RightSettings.Save(stream);
        }

        public void LoadSettings(string folder)
        {
            using (System.IO.Stream stream =
                System.IO.File.OpenRead(System.IO.Path.Combine(folder, "LeftPort.xml")))
                LeftSettings.Load(stream);
            using (System.IO.Stream stream =
                System.IO.File.OpenRead(System.IO.Path.Combine(folder, "RightPort.xml")))
                RightSettings.Load(stream);            
        }
    }
}
