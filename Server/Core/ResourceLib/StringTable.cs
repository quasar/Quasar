using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. It contains language 
    /// and code page formatting information for the strings. A code page is an ordered character set.
    /// See http://msdn.microsoft.com/en-us/library/aa909192.aspx for more information.
    /// </summary>
    public class StringTable : ResourceTableHeader
    {
        Dictionary<string, StringTableEntry> _strings = new Dictionary<string,StringTableEntry>();

        /// <summary>
        /// Resource strings.
        /// </summary>
        public Dictionary<string, StringTableEntry> Strings
        {
            get
            {
                return _strings;
            }
        }

        /// <summary>
        /// A new string table.
        /// </summary>
        public StringTable()
        {
        
        }

        /// <summary>
        /// A new string table.
        /// </summary>
        /// <param name="key">String table key.</param>
        public StringTable(string key)
            : base(key)
        {
            _header.wType = (UInt16)Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        /// <summary>
        /// An existing string table.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the table.</param>
        internal StringTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read a string table.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the string table.</param>
        /// <returns>Pointer to the end of the string table.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt64() < (lpRes.ToInt64() + _header.wLength))
            {
                StringTableEntry res = new StringTableEntry(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt64() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt64() + _header.wLength);
        }

        /// <summary>
        /// Write the string table to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <returns>Last unpadded position.</returns>
        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            int total = _strings.Count;
            Dictionary<string, StringTableEntry>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
                ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
                // total parent structure size must not include padding
                if (-- total != 0) ResourceUtil.PadToDWORD(w);
            }
        }
        
        /// <summary>
        /// The four most significant digits of the key represent the language identifier.
        /// Each Microsoft Standard Language identifier contains two parts: the low-order 10 bits 
        /// specify the major language, and the high-order 6 bits specify the sublanguage.
        /// </summary>
        public UInt16 LanguageID
        {
            get
            {
                if (string.IsNullOrEmpty(_key)) 
                    return 0;

                return Convert.ToUInt16(_key.Substring(0, 4), 16);
            }
            set
            {
                _key = string.Format("{0:x4}{1:x4}", value, CodePage);
            }
        }

        /// <summary>
        /// The four least significant digits of the key represent the code page for which the data is formatted.
        /// </summary>
        public UInt16 CodePage
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                    return 0;

                return Convert.ToUInt16(_key.Substring(4, 4), 16);
            }
            set
            {
                _key = string.Format("{0:x4}{1:x4}", LanguageID, value);
            }
        }

        /// <summary>
        /// Returns an entry within the string table.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>An entry within the string table.</returns>
        public string this[string key]
        {
            get
            {
                return _strings[key].Value;
            }
            set
            {
                StringTableEntry sr = null;
                if (!_strings.TryGetValue(key, out sr))
                {
                    sr = new StringTableEntry(key);
                    _strings.Add(key, sr);
                }

                sr.Value = value;
            }
        }

        /// <summary>
        /// String representation of the string table.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String representation of the strings table.</returns>
        public override string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}BEGIN", new String(' ', indent)));
            sb.AppendLine(string.Format("{0}BLOCK \"{1}\"", new String(' ', indent + 1), _key));
            sb.AppendLine(string.Format("{0}BEGIN", new String(' ', indent + 1)));
            foreach (StringTableEntry stringResource in _strings.Values)
            {
                sb.AppendLine(string.Format("{0}VALUE \"{1}\", \"{2}\"",
                    new String(' ', indent + 2),
                    stringResource.Key, stringResource.StringValue));
            }
            sb.AppendLine(string.Format("{0}END", new String(' ', indent + 1)));
            sb.AppendLine(string.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }
    }
}
