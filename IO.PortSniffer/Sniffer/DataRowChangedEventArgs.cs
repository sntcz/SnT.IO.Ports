using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.PortSniffer.Sniffer
{
    class DataRowChangedEventArgs : EventArgs
    {
        public DataRow Row { get; private set; }
        public int Index { get; private set; }
        public bool IsNew { get; private set; }

        public DataRowChangedEventArgs(DataRow row, int index, bool isNew)
        {
            Row = row;
            Index = index;
            IsNew = isNew;
        }
    }
}
