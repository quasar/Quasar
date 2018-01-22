using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. 
    /// It contains version information not dependent on a particular language and code page combination.
    /// http://msdn.microsoft.com/en-us/library/aa909193.aspx
    /// </summary>
    public class VarFileInfo : ResourceTableHeader
    {
        Dictionary<string, VarTable> _vars = new Dictionary<string, VarTable>();

        /// <summary>
        /// A hardware independent dictionary of language and code page identifier tables.
        /// </summary>
        public Dictionary<string, VarTable> Vars
        {
            get
            {
                return _vars;
            }
        }

        /// <summary>
        /// A new hardware independent dictionary of language and code page identifier tables.
        /// </summary>
        public VarFileInfo()
            : base("VarFileInfo")
        {
            _header.wType = (UInt16)Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        /// <summary>
        /// An existing hardware independent dictionary of language and code page identifier tables.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of data.</param>
        internal VarFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read a hardware independent dictionary of language and code page identifier tables.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of data.</param>
        /// <returns>Pointer to the end of data.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _vars.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt64() < (lpRes.ToInt64() + _header.wLength))
            {
                VarTable res = new VarTable(pChild);
                _vars.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt64() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt64() + _header.wLength);
        }

        /// <summary>
        /// Write the hardware independent dictionary of language and code page identifier tables to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<string, VarTable>.Enumerator varsEnum = _vars.GetEnumerator();
            while (varsEnum.MoveNext())
            {
                varsEnum.Current.Value.Write(w);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        /// <summary>
        /// The default language and code page identifier table.
        /// </summary>
        public VarTable Default
        {
            get
            {
                Dictionary<string, VarTable>.Enumerator varsEnum = _vars.GetEnumerator();
                if (varsEnum.MoveNext()) return varsEnum.Current.Value;
                return null;
            }
        }

        /// <summary>
        /// Returns a language and code page identifier table.
        /// </summary>
        /// <param name="language">Language ID.</param>
        /// <returns>A language and code page identifier table.</returns>
        public UInt16 this[UInt16 language]
        {
            get
            {
                return Default[language];
            }
            set
            {
                Default[language] = value;
            }
        }

        /// <summary>
        /// String representation of VarFileInfo.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String in the VarFileInfo format.</returns>
        public override string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}BEGIN", new String(' ', indent)));
            foreach(VarTable var in _vars.Values)
            {
                sb.Append(var.ToString(indent + 1));
            }
            sb.AppendLine(string.Format("{0}END", new String(' ', indent)));
            return sb.ToString();
        }
    }
}
