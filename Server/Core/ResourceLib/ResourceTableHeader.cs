using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// A resource table header.
    /// </summary>
    public class ResourceTableHeader
    {
        /// <summary>
        /// Resource table header.
        /// </summary>
        protected Kernel32.RESOURCE_HEADER _header;

        /// <summary>
        /// Resource table key.
        /// </summary>
        protected string _key;

        /// <summary>
        /// Resource table key.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Resource header.
        /// </summary>
        public Kernel32.RESOURCE_HEADER Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        /// <summary>
        /// A new resource table header.
        /// </summary>
        public ResourceTableHeader()
        {

        }

        /// <summary>
        /// An resource table header with a specific key.
        /// </summary>
        /// <param name="key">resource key</param>
        public ResourceTableHeader(string key)
        {
            _key = key;
        }

        /// <summary>
        /// An existing resource table.
        /// </summary>
        /// <param name="lpRes">Pointer to resource table data.</param>
        internal ResourceTableHeader(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read the resource header, return a pointer to the end of it.
        /// </summary>
        /// <param name="lpRes">Top of header.</param>
        /// <returns>End of header, after the key, aligned at a 32 bit boundary.</returns>
        internal virtual IntPtr Read(IntPtr lpRes)
        {
            _header = (Kernel32.RESOURCE_HEADER) Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.RESOURCE_HEADER));

            IntPtr pBlockKey = new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pBlockKey);

            return ResourceUtil.Align(pBlockKey.ToInt64() + (_key.Length + 1) * Marshal.SystemDefaultCharSize);
        }

        /// <summary>
        /// Write the resource table.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal virtual void Write(BinaryWriter w)
        {
            // wLength
            w.Write((UInt16) _header.wLength);
            // wValueLength
            w.Write((UInt16) _header.wValueLength);
            // wType
            w.Write((UInt16) _header.wType);
            // write key
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null-terminator
            w.Write((UInt16) 0);
            // pad fixed info
            ResourceUtil.PadToDWORD(w);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ToString(0);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String representation.</returns>
        public virtual string ToString(int indent)
        {
            return base.ToString();
        }
    }
}
