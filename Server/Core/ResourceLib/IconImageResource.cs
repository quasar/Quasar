using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in an icon resource.
    /// </summary>
    public class IconImageResource : Resource
    {
        /// <summary>
        /// Directory header.
        /// </summary>
        protected Kernel32.GRPICONDIRENTRY _header;

        /// <summary>
        /// Actual image.
        /// </summary>
        protected DeviceIndependentBitmap _image = new DeviceIndependentBitmap();

        /// <summary>
        /// Hardware-independent icon directory header.
        /// </summary>
        public Kernel32.GRPICONDIRENTRY Header
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
        /// Embedded icon Id.
        /// </summary>
        public ushort Id
        {
            get
            {
                return _header.nID;
            }
            set
            {
                _header.nID = value;
            }
        }

        /// <summary>
        /// An icon image.
        /// </summary>
        public DeviceIndependentBitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        /// <summary>
        /// An existing icon resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal IconImageResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new icon resource.
        /// </summary>
        public IconImageResource(ResourceId type)
            : base(IntPtr.Zero,
                IntPtr.Zero,
                type,
                new ResourceId(IntPtr.Zero),
                ResourceUtil.NEUTRALLANGID, 
                Marshal.SizeOf(typeof(Kernel32.GRPICONDIRENTRY)))
        {

        }

        /// <summary>
        /// Create a new icon image resource from a file icon.
        /// </summary>
        /// <param name="icon">File icon.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource id.</param>
        /// <param name="language">Resource language.</param>
        public IconImageResource(IconFileIcon icon, ResourceId type, ResourceId name, UInt16 language)
        {
            _name = name;
            _type = type;
            _language = language;
            _header.bColors = icon.Header.bColors;
            _header.bHeight = icon.Header.bHeight;
            _header.bReserved = icon.Header.bReserved;
            _header.bWidth = icon.Header.bWidth;
            _header.dwImageSize = icon.Header.dwImageSize;
            _header.wBitsPerPixel = icon.Header.wBitsPerPixel;
            _header.wPlanes = icon.Header.wPlanes;
            _header.nID = (UInt16) name.Id;
            _image = new DeviceIndependentBitmap(icon.Image);
        }

        /// <summary>
        /// Icon width in pixels.
        /// </summary>
        public Byte Width
        {
            get
            {
                return _header.bWidth;
            }
            set
            {
                _header.bWidth = value;
            }
        }

        /// <summary>
        /// Icon height in pixels.
        /// </summary>
        public Byte Height
        {
            get
            {
                return _header.bHeight;
            }
            set
            {
                _header.bHeight = value;
            }
        }

        /// <summary>
        /// Image size in bytes.
        /// </summary>
        public UInt32 ImageSize
        {
            get
            {
                return _header.dwImageSize;
            }
            set
            {
                _header.dwImageSize = value;
            }
        }

        /// <summary>
        /// Read an icon resource from a previously loaded module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to a directory entry in the hardware-independent icon resource.</param>
        /// <returns>Pointer to the end of the icon resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _header = (Kernel32.GRPICONDIRENTRY)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.GRPICONDIRENTRY));

            IntPtr hIconInfo = Kernel32.FindResourceEx(
                hModule, _type.Id, (IntPtr) _header.nID, _language);

            if (hIconInfo == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hIconRes = Kernel32.LoadResource(hModule, hIconInfo);
            if (hIconRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr dibBits = Kernel32.LockResource(hIconRes);
            if (dibBits == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            ReadImage(dibBits, (UInt32) Kernel32.SizeofResource(hModule, hIconInfo));

            return new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_header));
        }

        /// <summary>
        /// Read the bitmap image.
        /// </summary>
        /// <param name="dibBits">DIB bits.</param>
        /// <param name="size">Size of image.</param>
        internal virtual void ReadImage(IntPtr dibBits, UInt32 size)
        {
            _image.Read(dibBits, size);
        }

        /// <summary>
        /// Icon pixel format.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get
            {
                switch (_header.wBitsPerPixel)
                {
                    case 1:
                        return PixelFormat.Format1bppIndexed;
                    case 4:
                        return PixelFormat.Format4bppIndexed;
                    case 8:
                        return PixelFormat.Format8bppIndexed;
                    case 16:
                        return PixelFormat.Format16bppRgb565;
                    case 24: // not actually supported in icons
                        return PixelFormat.Format24bppRgb;
                    case 32: // 0RGB 32bpp bitmap or ARGB (alpha-blended)
                        return PixelFormat.Format32bppArgb;
                    default:
                        return PixelFormat.Undefined;
                }
            }
        }

        /// <summary>
        /// Icon pixel format English standard string.
        /// </summary>
        public string PixelFormatString
        {
            get
            {
                switch (PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                        return "1-bit B/W";
                    case PixelFormat.Format24bppRgb:
                        return "24-bit True Colors";
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppRgb:
                        return "32-bit Alpha Channel";
                    case PixelFormat.Format8bppIndexed:
                        return "8-bit 256 Colors";
                    case PixelFormat.Format4bppIndexed:
                        return "4-bit 16 Colors";
                }
                return "Unknown";
            }
        }

        /// <summary>
        /// String representation of the icon.
        /// </summary>
        /// <returns>A string in a format of width x height followed by the pixel format.</returns>
        public override string ToString()
        {
            return string.Format("{0}x{1} {2}",
                Width, Height, PixelFormatString);
        }

        /// <summary>
        /// Write icon resource data to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write(_header.bWidth);
            w.Write(_header.bHeight);
            w.Write(_header.bColors);
            w.Write(_header.bReserved);
            w.Write(_header.wPlanes);
            w.Write(_header.wBitsPerPixel);
            w.Write(_header.dwImageSize);
            w.Write(_header.nID);
            ResourceUtil.PadToWORD(w);
        }

        /// <summary>
        /// Save icon to a file.
        /// </summary>
        /// <param name="filename">Target executable file.</param>
        public virtual void SaveIconTo(string filename)
        {
            SaveTo(filename,
                _type,
                new ResourceId(_header.nID), 
                _language,
                _image.Data);
        }
    }
}
