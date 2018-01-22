using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// VS_VERSIONINFO
    /// This structure depicts the organization of data in a file-version resource. It is the root structure
    /// that contains all other file-version information structures.
    /// http://msdn.microsoft.com/en-us/library/aa914916.aspx
    /// </summary>
    public class VersionResource : Resource
    {
        ResourceTableHeader _header = new ResourceTableHeader("VS_VERSION_INFO");
        FixedFileInfo _fixedfileinfo = new FixedFileInfo();
        private OrderedDictionary _resources = new OrderedDictionary();

        /// <summary>
        /// The resource header.
        /// </summary>
        public ResourceTableHeader Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// A dictionary of resource tables.
        /// </summary>
        public OrderedDictionary Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// An existing version resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public VersionResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new language-netural version resource.
        /// </summary>
        public VersionResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_VERSION),
                new ResourceId(1),
                ResourceUtil.USENGLISHLANGID,
                0)
        {
            _header.Header = new Kernel32.RESOURCE_HEADER(_fixedfileinfo.Size);
        }

        /// <summary>
        /// Read a version resource from a previously loaded module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of the resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _resources.Clear();

            IntPtr pFixedFileInfo = _header.Read(lpRes);

            if (_header.Header.wValueLength != 0)
            {
                _fixedfileinfo = new FixedFileInfo();
                _fixedfileinfo.Read(pFixedFileInfo);
            }

            IntPtr pChild = ResourceUtil.Align(pFixedFileInfo.ToInt64() + _header.Header.wValueLength);

            while (pChild.ToInt64() < (lpRes.ToInt64() + _header.Header.wLength))
            {
                ResourceTableHeader rc = new ResourceTableHeader(pChild);
                switch (rc.Key)
                {
                    case "StringFileInfo":
                        StringFileInfo sr = new StringFileInfo(pChild);
                        rc = sr;
                        break;
                    default:
                        rc = new VarFileInfo(pChild);
                        break;
                }

                _resources.Add(rc.Key, rc);
                pChild = ResourceUtil.Align(pChild.ToInt64() + rc.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt64() + _header.Header.wLength);
        }

        /// <summary>
        /// String representation of the file version.
        /// </summary>
        public string FileVersion
        {
            get
            {
                return _fixedfileinfo.FileVersion;
            }
            set
            {
                _fixedfileinfo.FileVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets a bitmask that specifies the Boolean attributes of the file.
        /// </summary>
        public uint FileFlags
        {
          get
          {
            return this._fixedfileinfo.FileFlags;
          }
          set
          {
            this._fixedfileinfo.FileFlags = value;
          }
        }

        /// <summary>
        /// String representation of the protect version.
        /// </summary>
        public string ProductVersion
        {
            get
            {
                return _fixedfileinfo.ProductVersion;
            }
            set
            {
                _fixedfileinfo.ProductVersion = value;
            }
        }

        /// <summary>
        /// Write this version resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            _header.Write(w);

            if (_fixedfileinfo != null)
            {
                _fixedfileinfo.Write(w);
            }

            foreach (DictionaryEntry dictionaryEntry in _resources)
            {
                ((ResourceTableHeader)dictionaryEntry.Value).Write(w);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        /// <summary>
        /// Returns an entry within this resource table.
        /// </summary>
        /// <param name="key">Entry key.</param>
        /// <returns>A resource table.</returns>
        public ResourceTableHeader this[string key]
        {
            get
            {
                return (ResourceTableHeader)Resources[key];
            }
            set
            {
                Resources[key] = value;
            }
        }

        /// <summary>
        /// Returns an entry within this resource table.
        /// </summary>
        /// <param name="index">Entry index.</param>
        /// <returns>A resource table.</returns>
        public ResourceTableHeader this[int index]
        {
            get
            {
                return (ResourceTableHeader)Resources[index];
            }
            set
            {
                Resources[index] = value;
            }
        }

        /// <summary>
        /// Return string representation of the version resource.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_fixedfileinfo != null)
            {
                sb.Append(_fixedfileinfo.ToString());
            }
            sb.AppendLine("BEGIN");
            foreach (DictionaryEntry dictionaryEntry in _resources)
            {
                sb.Append(((ResourceTableHeader)dictionaryEntry.Value).ToString(1));
            }
            sb.AppendLine("END");
            return sb.ToString();
        }
    }
}
