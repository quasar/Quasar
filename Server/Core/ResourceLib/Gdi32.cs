using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// Gdi32.dll interop functions.
    /// </summary>
    public abstract class Gdi32
    {
        /// <summary>
        /// Bitmap compression options.
        /// </summary>
        public enum BitmapCompression
        {
            /// <summary>
            /// An uncompressed format. 
            /// </summary>
            BI_RGB = 0,
            /// <summary>
            /// A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format is a 2-byte format consisting of a count byte followed by a byte containing a color index. For more information, see Bitmap Compression.
            /// </summary>
            BI_RLE8 = 1,
            /// <summary>
            /// An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format consisting of a count byte followed by two word-length color indexes. For more information, see Bitmap Compression.
            /// </summary>
            BI_RLE4 = 2,
            /// <summary>
            /// Specifies that the bitmap is not compressed and that the color table consists of three DWORD color masks that specify the red, green, and blue components, respectively, of each pixel. This is valid when used with 16- and 32-bpp bitmaps.
            /// </summary>
            BI_BITFIELDS = 3,
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a JPEG image.
            /// </summary>
            BI_JPEG = 4,
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a PNG image.
            /// </summary>
            BI_PNG = 5,
        }

        /// <summary>
        /// A bitmap info header.
        /// See http://msdn.microsoft.com/en-us/library/ms532290.aspx for more information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPINFOHEADER
        {
            /// <summary>
            /// Bitmap information size.
            /// </summary>
            public UInt32 biSize;
            /// <summary>
            /// Bitmap width.
            /// </summary>
            public Int32 biWidth;
            /// <summary>
            /// Bitmap height.
            /// </summary>
            public Int32 biHeight;
            /// <summary>
            /// Number of logical planes.
            /// </summary>
            public UInt16 biPlanes;
            /// <summary>
            /// Bitmap bitrate.
            /// </summary>
            public UInt16 biBitCount;
            /// <summary>
            /// Bitmap compression.
            /// </summary>
            public UInt32 biCompression;
            /// <summary>
            /// Image size.
            /// </summary>
            public UInt32 biSizeImage;
            /// <summary>
            /// Horizontal pixel resolution.
            /// </summary>
            public Int32 biXPelsPerMeter;
            /// <summary>
            /// Vertical pixel resolution.
            /// </summary>
            public Int32 biYPelsPerMeter;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 biClrUsed;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 biClrImportant;

            /// <summary>
            /// Returns the current bitmap compression.
            /// </summary>
            public BitmapCompression BitmapCompression
            {
                get
                {
                    return (BitmapCompression)biCompression;
                }
            }

            /// <summary>
            /// Bitmap pixel format.
            /// </summary>
            public PixelFormat PixelFormat
            {
                get
                {
                    switch (biBitCount)
                    {
                        case 1:
                            return PixelFormat.Format1bppIndexed;
                        case 4:
                            return PixelFormat.Format4bppIndexed;
                        case 8:
                            return PixelFormat.Format8bppIndexed;
                        case 16:
                            return PixelFormat.Format16bppRgb565;
                        case 24:
                            return PixelFormat.Format24bppRgb;
                        case 32:
                            return PixelFormat.Format32bppArgb;
                        default:
                            return PixelFormat.Undefined;
                    }
                }
            }

            /// <summary>
            /// Bitmap pixel format English standard string.
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
        }

        /// <summary>
        /// Defines the dimensions and color information of a Windows-based device-independent bitmap (DIB). 
        /// http://msdn.microsoft.com/en-us/library/dd183375(VS.85).aspx.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            /// <summary>
            /// Specifies a bitmap information header structure that contains information about the dimensions of color format.
            /// </summary>
            public BITMAPINFOHEADER bmiHeader;
            /// <summary>
            /// An array of RGBQUAD. The elements of the array make up the color table.
            /// </summary>
            public RGBQUAD bmiColors;
        }

        /// <summary>
        /// Store colors in a paletised icon (2, 4 or 8 bit).
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RGBQUAD
        {
            /// <summary>
            /// Blue.
            /// </summary>
            public Byte rgbBlue;
            /// <summary>
            /// Green.
            /// </summary>
            public Byte rgbGreen;
            /// <summary>
            /// Red.
            /// </summary>
            public Byte rgbRed;
            /// <summary>
            /// Reserved.
            /// </summary>
            public Byte rgbReserved;
        }

        /// <summary>
        /// The BITMAPFILEHEADER structure contains information about the type, size, and layout of a file that contains a DIB.
        /// http://msdn.microsoft.com/en-us/library/dd183374(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPFILEHEADER
        {
            /// <summary>
            /// The file type; must be BM.
            /// </summary>
            public UInt16 bfType;
            /// <summary>
            /// The size, in bytes, of the bitmap file.
            /// </summary>
            public UInt32 bfSize;
            /// <summary>
            /// Reserved; must be zero.
            /// </summary>
            public UInt16 bfReserved1;
            /// <summary>
            /// Reserved; must be zero.
            /// </summary>
            public UInt16 bfReserved2;
            /// <summary>
            /// The offset, in bytes, from the beginning of the BITMAPFILEHEADER structure to the bitmap bits.
            /// </summary>
            public UInt32 bfOffBits;
        }

        /// <summary>
        /// Set the pixels in the specified rectangle on the device that is associated with the destination device 
        /// context using color data from a DIB, JPEG, or PNG image.
        /// http://msdn.microsoft.com/en-us/library/dd162974(VS.85).aspx
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <param name="XDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="YDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="dwWidth">The width, in logical units, of the image.</param>
        /// <param name="dwHeight">The height, in logical units, of the image.</param>
        /// <param name="XSrc">The x-coordinate, in logical units, of the lower-left corner of the image.</param>
        /// <param name="YSrc">The y-coordinate, in logical units, of the lower-left corner of the image.</param>
        /// <param name="uStartScan">The starting scan line in the image.</param>
        /// <param name="cScanLines">The number of DIB scan lines contained in the array pointed to by the lpvBits parameter.</param>
        /// <param name="lpvBits">A pointer to the color data stored as an array of bytes.</param>
        /// <param name="lpbmi">A pointer to a BITMAPINFOHEADER structure that contains information about the DIB.</param>
        /// <param name="fuColorUse">Indicates whether the bmiColors member of the BITMAPINFOHEADER structure contains explicit red, green, blue (RGB) values or indexes into a palette.</param>
        /// <returns>
        /// If the function succeeds, the return value is the number of scan lines set.
        /// If zero scan lines are set (such as when dwHeight is 0) or the function fails, the function returns zero.
        /// If the driver cannot support the JPEG or PNG file image passed to SetDIBitsToDevice, the function will fail and return GDI_ERROR. 
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int SetDIBitsToDevice(IntPtr hdc, Int32 XDest, Int32 YDest, UInt32 dwWidth, UInt32 dwHeight,
            Int32 XSrc, Int32 YSrc, UInt32 uStartScan, UInt32 cScanLines, byte[] lpvBits, [In] ref BITMAPINFO lpbmi, UInt32 fuColorUse);

        /// <summary>
        /// Set the pixels in the specified rectangle on the device that is associated with the destination device 
        /// context using color data from a DIB, JPEG, or PNG image.
        /// http://msdn.microsoft.com/en-us/library/dd162974(VS.85).aspx
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <param name="XDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="YDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="dwWidth">The width, in logical units, of the image.</param>
        /// <param name="dwHeight">The height, in logical units, of the image.</param>
        /// <param name="XSrc">The x-coordinate, in logical units, of the lower-left corner of the image.</param>
        /// <param name="YSrc">The y-coordinate, in logical units, of the lower-left corner of the image.</param>
        /// <param name="uStartScan">The starting scan line in the image.</param>
        /// <param name="cScanLines">The number of DIB scan lines contained in the array pointed to by the lpvBits parameter.</param>
        /// <param name="lpvBits">A pointer to the color data stored as an array of bytes.</param>
        /// <param name="lpbmi">A pointer to a BITMAPINFOHEADER structure that contains information about the DIB.</param>
        /// <param name="fuColorUse">Indicates whether the bmiColors member of the BITMAPINFOHEADER structure contains explicit red, green, blue (RGB) values or indexes into a palette.</param>
        /// <returns>
        /// If the function succeeds, the return value is the number of scan lines set.
        /// If zero scan lines are set (such as when dwHeight is 0) or the function fails, the function returns zero.
        /// If the driver cannot support the JPEG or PNG file image passed to SetDIBitsToDevice, the function will fail and return GDI_ERROR. 
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int SetDIBitsToDevice(IntPtr hdc, Int32 XDest, Int32 YDest, UInt32 dwWidth, UInt32 dwHeight,
            Int32 XSrc, Int32 YSrc, UInt32 uStartScan, UInt32 cScanLines, IntPtr lpvBits, IntPtr lpbmi, UInt32 fuColorUse);

        /// <summary>
        /// Retrieves the bits of the specified compatible bitmap and copies them into a buffer as 
        /// a DIB using the specified format
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <param name="hbmp">A handle to the bitmap. This must be a compatible bitmap (DDB).</param>
        /// <param name="uStartScan">The first scan line to retrieve.</param>
        /// <param name="cScanLines">The number of scan lines to retrieve.</param>
        /// <param name="lpvBits">A pointer to a buffer to receive the bitmap data.</param>
        /// <param name="lpbmi">A pointer to a BITMAPINFO structure that specifies the desired format for the DIB data.</param>
        /// <param name="uUsage">The format of the bmiColors member of the BITMAPINFO structure.</param>
        /// <returns>
        /// If the lpvBits parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.
        /// If the lpvBits parameter is NULL and GetDIBits successfully fills the BITMAPINFO structure, the return value is non-zero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines,
            [Out] byte[] lpvBits, [In] ref BITMAPINFO lpbmi, uint uUsage);

        /// <summary>
        /// Create a DIB that applications can write to directly. The function gives you a pointer to the location 
        /// of the bitmap bit values. You can supply a handle to a file-mapping object that the function will use 
        /// to create the bitmap, or you can let the system allocate the memory for the bitmap.
        /// </summary>
        /// <param name="hdc">Handle to a device context.</param>
        /// <param name="pbmi">A pointer to a BITMAPINFO structure that specifies various attributes of the DIB, including the bitmap dimensions and colors.</param>
        /// <param name="iUsage">The type of data contained in the bmiColors array member of the BITMAPINFO structure pointed to by pbmi (either logical palette indexes or literal RGB values).</param>
        /// <param name="ppvBits">A pointer to a variable that receives a pointer to the location of the DIB bit values.</param>
        /// <param name="hSection">A handle to a file-mapping object that the function will use to create the DIB. This parameter can be NULL.</param>
        /// <param name="dwOffset">The offset from the beginning of the file-mapping object referenced by hSection where storage for the bitmap bit values is to begin.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the newly created DIB, and *ppvBits points to the bitmap bit values.
        /// If the function fails, the return value is NULL, and *ppvBits is NULL.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi,
            uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        /// <summary>
        /// Defines how to interpret the values in the color table of a DIB.
        /// </summary>
        internal enum DIBColors
        {
            /// <summary>
            /// The color table contains literal RGB values.
            /// </summary>
            DIB_RGB_COLORS = 0,
            /// <summary>
            /// The color table consists of an array of 16-bit indexes into the LogPalette 
            /// object that is currently defined in the playback device context.
            /// </summary>
            DIB_PAL_COLORS = 1,
            /// <summary>
            /// No color table exists. The pixels in the DIB are indices into the current logical 
            /// palette in the playback device context.
            /// </summary>
            DIB_PAL_INDICES = 2,
            /// <summary>
            /// 
            /// </summary>
            DIB_PAL_LOGINDICES = 4
        }

        /// <summary>
        /// Creates a memory device context (DC) compatible with the specified device.
        /// </summary>
        /// <param name="hdc">Handle to an existing device context.</param>
        /// <returns>
        /// The handle to a memory device context indicates success.
        /// NULL indicates failure.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpDriverName">Specifies either DISPLAY or the name of a specific display device or the name of a print provider, which is usually WINSPOOL.</param>
        /// <param name="lpDeviceName">Specifies the name of the specific output device being used, as shown by the Print Manager (for example, Epson FX-80).</param>
        /// <param name="lpOutput">This parameter is ignored and should be set to NULL. It is provided only for compatibility with 16-bit Windows.</param>
        /// <param name="lpInitData">A pointer to a DEVMODE structure containing device-specific initialization data for the device driver.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, IntPtr lpInitData);

        /// <summary>
        /// Creates a bitmap compatible with the device that is associated with the specified device context.
        /// </summary>
        /// <param name="hdc">A handle to a device context.</param>
        /// <param name="nWidth">The bitmap width, in pixels.</param>
        /// <param name="nHeight">The bitmap height, in pixels.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the compatible bitmap (DDB).
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, Int32 nWidth, Int32 nHeight);

        /// <summary>
        /// Selects an object into a specified device context. The new object replaces the previous object of the same type. 
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <param name="hgdiobj">Handle to the object to be selected.</param>
        /// <returns>
        /// If the selected object is not a region, the handle of the object being replaced indicates success. 
        /// If the selected object is a region, one of the following values indicates success. 
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        /// <summary>
        /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. 
        /// </summary>
        /// <param name="hObject">Handle to a logical pen, brush, font, bitmap, region, or palette.</param>
        /// <returns>
        /// Nonzero indicates success. 
        /// Zero indicates that the specified handle is not valid or that the handle is currently selected into a device context.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int DeleteObject(IntPtr hObject);

        /// <summary>
        /// Deletes the specified device context.
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern int DeleteDC(IntPtr hdc);
    }
}
