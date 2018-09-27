using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xServer.Controls.HexEditor
{
    public class ByteCollection
    {
        private List<byte> _bytes;

        #region Properties

        public int Length
        {
            get { return _bytes.Count; }
        }

        #endregion

        #region Constructor

        public ByteCollection()
        {
            _bytes = new List<byte>();
        }

        public ByteCollection(byte[] bytes)
        {
            _bytes = new List<byte>(bytes);
        }

        #endregion

        #region Methods

        public void Add(byte item)
        {
            _bytes.Add(item);
        }

        public void Insert(int index, byte item)
        {
            _bytes.Insert(index, item);
        }

        public void Remove(byte item)
        {
            _bytes.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _bytes.RemoveAt(index);
        }

        public void RemoveRange(int startIndex, int count)
        {
            _bytes.RemoveRange(startIndex, count);
        }

        public byte GetAt(int index)
        {
            return _bytes[index];
        }

        public void SetAt(int index, byte item)
        {
            _bytes[index] = item;
        }

        public char GetCharAt(int index)
        {
            return Convert.ToChar(_bytes[index]);
        }

        public byte[] ToArray()
        {
            return _bytes.ToArray();
        }

        #endregion
    }
}
