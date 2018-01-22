using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. 
    /// It contains version information that can be displayed for a particular language and code page.
    /// http://msdn.microsoft.com/en-us/library/aa908808.aspx
    /// </summary>
    public class StringFileInfo : ResourceTableHeader
    {
        Dictionary<string, StringTable> _strings = new Dictionary<string, StringTable>();

        /// <summary>
        /// Resource strings.
        /// </summary>
        public Dictionary<string, StringTable> Strings
        {
            get
            {
                return _strings;
            }
        }

        /// <summary>
        /// A new string file-version resource.
        /// </summary>
        public StringFileInfo()
            : base("StringFileInfo")
        {
            _header.wType = (UInt16) Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        /// <summary>
        /// An existing string file-version resource.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of a string file-version resource.</param>
        internal StringFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read an existing string file-version resource.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of a string file-version resource.</param>
        /// <returns>Pointer to the end of the string file-version resource.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt64() < (lpRes.ToInt64() + _header.wLength))
            {
                StringTable res = new StringTable(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt64() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt64() + _header.wLength);
        }

        /// <summary>
        /// Write the string file-version resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<string, StringTable>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
            ResourceUtil.PadToDWORD(w);
        }

        /// <summary>
        /// Default (first) string table.
        /// </summary>
        public StringTable Default
        {
            get
            {
                Dictionary<string, StringTable>.Enumerator iter = _strings.GetEnumerator();
                if (iter.MoveNext()) return iter.Current.Value;
                return null;
            }
        }

        /// <summary>
        /// Indexed string table.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>A string table at a given index.</returns>
        public string this[string key]
        {
            get
            {
                return Default[key];
            }
            set
            {
                Default[key] = value;
            }
        }

        /// <summary>
        /// String representation of StringFileInfo.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String in the StringFileInfo format.</returns>
        public override string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}BEGIN", new String(' ', indent)));
            sb.AppendLine(string.Format("{0}BLOCK \"{1}\"", new String(' ', indent + 1), _key));
            foreach(StringTable stringTable in _strings.Values)
            {
                sb.Append(stringTable.ToString(indent + 1));
            }
            sb.AppendLine(string.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }
    }
}
