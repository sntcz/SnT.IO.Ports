using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SnT.IO.PortSniffer.Sniffer
{
    class DataCollection : IEnumerable, IEnumerable<DataRow>
    {
        private readonly List<DataRow> rows = new List<DataRow>();
        private readonly object syncRoot = new object();

        public event EventHandler<DataRowChangedEventArgs> DataRowChanged;
        public event EventHandler DataCleared;

        public int Count { get { return rows.Count; } }

        public DataRow this[int index]
        {
            get { return rows[index]; }
        }

        public void Append(Direction direction, byte data)
        {
            bool isNew = false;
            int index = -1;
            DataRow last = null;
            lock (syncRoot)
            {
                if (rows.Count > 0)
                {
                    last = rows[rows.Count - 1];
                    if (last.Direction != direction)
                        last = null;
                }
                if (last == null)
                {
                    last = new DataRow(direction);
                    rows.Add(last);
                    isNew = true;
                }
                last.Buffer.Append(data);
                index = rows.Count - 1;
            }
            OnDataRowChanged(last, index, isNew);
        }

        public DataRow GetLastRow()
        {
            DataRow last = null;
            lock (syncRoot)
            {
                if (rows.Count > 0)
                    last = rows[rows.Count - 1];
            }
            return last;
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                rows.Clear();
                OnDataCleared();
            }
        }

        private void OnDataRowChanged(DataRow row, int index, bool isNew)
        {
            if (DataRowChanged != null)
                DataRowChanged(this, new DataRowChangedEventArgs(row, index, isNew));
        }

        private void OnDataCleared()
        {
            if (DataCleared != null)
                DataCleared(this, EventArgs.Empty);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable<DataRow> Members

        IEnumerator<DataRow> IEnumerable<DataRow>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Enumerator

        public class Enumerator : IEnumerator, IEnumerator<DataRow>
        {
            private readonly DataCollection DataCollection;
            private int Cursor;

            public Enumerator(DataCollection dataCollection)
            {
                DataCollection = dataCollection;
                Cursor = -1;
            }

            private DataRow GetCurrent()
            {
                if ((Cursor < 0) || (Cursor >= DataCollection.Count))
                    throw new InvalidOperationException();
                return DataCollection[Cursor];
            }

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return GetCurrent(); }
            }

            bool IEnumerator.MoveNext()
            {
                if (Cursor < DataCollection.Count)
                    Cursor++;

                return (Cursor < DataCollection.Count);
            }

            void IEnumerator.Reset()
            {
                Cursor = -1;
            }

            #endregion

            #region IEnumerator<DataRow> Members

            DataRow IEnumerator<DataRow>.Current
            {
                get { return GetCurrent(); }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                /* NOP */
            }

            #endregion
        }

        #endregion

        public void Save(System.IO.Stream stream)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, Encoding.Default))
            {
                foreach (DataRow row in rows)
                {
                    writer.WriteLine(row.Text);
                }
            }
        }
    }
}
