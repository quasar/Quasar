using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// Fixed file information.
    /// </summary>
    public class FixedFileInfo
    {
        Kernel32.VS_FIXEDFILEINFO _fixedfileinfo = Kernel32.VS_FIXEDFILEINFO.GetWindowsDefault();

        /// <summary>
        /// Default Windows fixed file information.
        /// </summary>
        public FixedFileInfo()
        {

        }

        /// <summary>
        /// Fixed file info structure.
        /// </summary>
        public Kernel32.VS_FIXEDFILEINFO Value
        {
            get
            {
                return _fixedfileinfo;
            }
        }

        /// <summary>
        /// Read the fixed file information structure.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        internal void Read(IntPtr lpRes)
        {
            _fixedfileinfo = (Kernel32.VS_FIXEDFILEINFO)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.VS_FIXEDFILEINFO));
        }

        /// <summary>
        /// String representation of the file version.
        /// </summary>
        public string FileVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    ResourceUtil.HiWord(_fixedfileinfo.dwFileVersionMS),
                    ResourceUtil.LoWord(_fixedfileinfo.dwFileVersionMS),
                    ResourceUtil.HiWord(_fixedfileinfo.dwFileVersionLS),
                    ResourceUtil.LoWord(_fixedfileinfo.dwFileVersionLS));
            }
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                string[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.dwFileVersionMS = (major << 16) + minor;
                _fixedfileinfo.dwFileVersionLS = (build << 16) + release;
            }
        }

        /// <summary>
        /// String representation of the protect version.
        /// </summary>
        public string ProductVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    ResourceUtil.HiWord(_fixedfileinfo.dwProductVersionMS),
                    ResourceUtil.LoWord(_fixedfileinfo.dwProductVersionMS),
                    ResourceUtil.HiWord(_fixedfileinfo.dwProductVersionLS),
                    ResourceUtil.LoWord(_fixedfileinfo.dwProductVersionLS));
            }
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                string[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.dwProductVersionMS = (major << 16) + minor;
                _fixedfileinfo.dwProductVersionLS = (build << 16) + release;
            }
        }

        /// <summary>
        /// Gets or sets a bitmask that specifies the Boolean attributes of the file.
        /// </summary>
        public uint FileFlags
        {
          get
          {
            return this._fixedfileinfo.dwFileFlags;
          }
          set
          {
            this._fixedfileinfo.dwFileFlags = value;
          }
        }

        /// <summary>
        /// Write fixed file information to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public void Write(BinaryWriter w)
        {
            w.Write(ResourceUtil.GetBytes<Kernel32.VS_FIXEDFILEINFO>(_fixedfileinfo));
            ResourceUtil.PadToDWORD(w);
        }

        /// <summary>
        /// Size of the VS_FIXEDFILEINFO structure.
        /// </summary>
        public UInt16 Size
        {
            get
            {
                return (UInt16) Marshal.SizeOf(_fixedfileinfo);
            }
        }

        /// <summary>
        /// String representation of the fixed file info.
        /// </summary>
        /// <returns>String representation of the fixed file info.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("FILEVERSION {0},{1},{2},{3}",
                ResourceUtil.HiWord(_fixedfileinfo.dwFileVersionMS),
                ResourceUtil.LoWord(_fixedfileinfo.dwFileVersionMS),
                ResourceUtil.HiWord(_fixedfileinfo.dwFileVersionLS),
                ResourceUtil.LoWord(_fixedfileinfo.dwFileVersionLS)));
            sb.AppendLine(string.Format("PRODUCTVERSION {0},{1},{2},{3}",
                ResourceUtil.HiWord(_fixedfileinfo.dwProductVersionMS),
                ResourceUtil.LoWord(_fixedfileinfo.dwProductVersionMS),
                ResourceUtil.HiWord(_fixedfileinfo.dwProductVersionLS),
                ResourceUtil.LoWord(_fixedfileinfo.dwProductVersionLS)));
            if (_fixedfileinfo.dwFileFlagsMask == Winver.VS_FFI_FILEFLAGSMASK)
            {
                sb.AppendLine("FILEFLAGSMASK VS_FFI_FILEFLAGSMASK");
            }
            else
            {
                sb.AppendLine(string.Format("FILEFLAGSMASK 0x{0:x}",
                    _fixedfileinfo.dwFileFlagsMask.ToString()));
            }
            sb.AppendLine(string.Format("FILEFLAGS {0}",
                _fixedfileinfo.dwFileFlags == 0 ? "0" : ResourceUtil.FlagsToString<Winver.FileFlags>(
                    _fixedfileinfo.dwFileFlags)));
            sb.AppendLine(string.Format("FILEOS {0}",
                ResourceUtil.FlagsToString<Winver.FileOs>(_fixedfileinfo.dwFileFlags)));
            sb.AppendLine(string.Format("FILETYPE {0}",
                ResourceUtil.FlagsToString<Winver.FileType>(_fixedfileinfo.dwFileType)));
            sb.AppendLine(string.Format("FILESUBTYPE {0}",
                ResourceUtil.FlagsToString<Winver.FileSubType>(_fixedfileinfo.dwFileSubtype)));
            return sb.ToString();
        }
    }
}
