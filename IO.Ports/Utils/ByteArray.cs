using System;
using System.Collections.Generic;
using System.Text;

namespace SnT.IO.Ports.Utils
{
    public class ByteArray
    {

        public static int DefaultSize { get; set; }
        public static int DefaultIncrement { get; set; }         

        static ByteArray()
        {
            DefaultSize = 512;
            DefaultIncrement = 128;
        }

        private byte[] buffer = null;

        public int Count { get; private set; }

        public int Increment { get; set; }

        public ByteArray(int size, int increment)
        {
            buffer = new byte[size];
            Count = 0;
            Increment = increment;
        }

        public ByteArray(int size) : this(size, DefaultIncrement) { /* NOP */ }

        public ByteArray() : this(DefaultSize) { /* NOP */ }

        public ByteArray(byte[] data)
            : this(GetRoundedLength(data.Length, DefaultSize, DefaultIncrement))
        {
            System.Diagnostics.Debug.Assert(data.Length <= buffer.Length,
                "Initialization data length error");
            data.CopyTo(buffer, 0);
            Count = data.Length;
        }

        public byte this[int pos]
        {
            get { return GetByte(pos); }
            set { SetByte(value, pos); }
        }

        public void Append(int data)
        {
            Append(Convert.ToByte(data));
        }

        public void Append(byte data)
        {
            if (Count >= buffer.Length)
                AdjustBuffer();

            buffer[Count] = data;
            Count++;
        }

        public void Append(params byte[] data)
        {
            // Nelze jednoduše optimalizovat přes blokové copy
            for (int i = 0; i < data.Length; i++)
            {
                Append(data[i]);
            }
        }

        /// <summary>
        /// Přidat od pozice ve zdrojovém poli
        /// </summary>
        /// <param name="data">pole pro přidání</param>
        /// <param name="offset">od kterého data zdrojového pole přidat</param>
        public void Append(byte[] srcData, int srcOffset)
        {
            if (srcOffset >= srcData.Length)
                throw new ArgumentOutOfRangeException("srcOffset");
            //TODO: Optimalizovat 
            // přes Buffer.BlockCopy asi takhle:
            //int copyCount = srcData.Length - srcOffset;
            //AdjustBuffer(buffer.Length + copyCount);
            //Buffer.BlockCopy(srcData, srcOffset, buffer, Count, copyCount);
            //Count += copyCount;
            // ale není to otestováno !!!!!
            for (int i = srcOffset; i < srcData.Length; i++)
            {
                Append(srcData[i]);
            }
        }

        public void Append(byte[] srcData, int srcOffset, int count)
        {
            if (srcOffset >= srcData.Length)
                throw new ArgumentOutOfRangeException("srcOffset");
            if (srcOffset + count >= srcData.Length)
                throw new ArgumentOutOfRangeException("srcOffset + count");
            //TODO: Optimalizovat
            for (int i = 0; i < count; i++)
            {
                Append(srcData[srcOffset + i]);
            }
        }

        public void SetByte(byte data, int pos)
        {
            if (pos >= Count)
                throw new ArgumentOutOfRangeException("pos");
            buffer[pos] = data;
        }

        public void Clear()
        {
            Count = 0;
        }

        public byte GetByte(int pos)
        {
            if (pos >= Count)
                throw new ArgumentOutOfRangeException("pos");
            return buffer[pos];
        }

        public byte GetReverseByte(int reversePos)
        {
            if (reversePos >= Count)
                throw new ArgumentOutOfRangeException("reversePos");
            return buffer[Count - reversePos - 1];
        }

        public byte[] GetBytes()
        {
            return GetBytes(0, Count);
        }

        public byte[] GetBytes(int offset)
        {
            return GetBytes(offset, Count - offset);
        }

        public byte[] GetBytes(int offset, int count)
        {
            if (offset >= Count)
                throw new ArgumentOutOfRangeException("offset");
            if (offset + count > Count)
                throw new ArgumentOutOfRangeException("offset + count");
            byte[] tmpBuff = new byte[count];
            Buffer.BlockCopy(buffer, offset, tmpBuff, 0, count);
            return tmpBuff;
        }

        public byte[] GetPage(int pageNo, int pageLength)
        {
            if ((pageNo * pageLength) >= Count)
                throw new ArgumentOutOfRangeException("pageNo + pageLength");
            int offset = pageNo * pageLength;
            int length = Count - offset > pageLength ? pageLength : Count - offset;
            return GetBytes(offset, length);
        }

        public string GetString(Encoding encoding)
        {
            return encoding.GetString(buffer, 0, Count);
        }

        private void AdjustBuffer()
        {
            AdjustBuffer(buffer.Length + Increment);
        }

        private void AdjustBuffer(int newLength)
        {
            byte[] tmpBuff = new byte[newLength];
            buffer.CopyTo(tmpBuff, 0);
            buffer = tmpBuff;
        }

        private static int GetRoundedLength(int length, int start, int increment)
        {
            int rounded = start;
            while (length > rounded)
                rounded += increment;
            return rounded;
        }
    }
}
