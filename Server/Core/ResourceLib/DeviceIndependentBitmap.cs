using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// A device-independent image consists of a BITMAPINFOHEADER where
    /// bmWidth is the width of the image andbmHeight is double the height 
    /// of the image, followed by the bitmap color table, followed by the image
    /// pixels, followed by the mask pixels.
    /// </summary>
    public class DeviceIndependentBitmap
    {
        private Gdi32.BITMAPINFOHEADER _header = new Gdi32.BITMAPINFOHEADER();
        private byte[] _data = null;
        private Bitmap _mask = null;
        private Bitmap _color = null;
        private Bitmap _image = null;

        /// <summary>
        /// Raw image data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;

                IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(_header));
                try
                {
                    Marshal.Copy(_data, 0, pData, Marshal.SizeOf(_header));
                    _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                        pData, typeof(Gdi32.BITMAPINFOHEADER));
                }
                finally
                {
                    Marshal.FreeHGlobal(pData);
                }
            }
        }

        /// <summary>
        /// Bitmap info header.
        /// </summary>
        public Gdi32.BITMAPINFOHEADER Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Bitmap size in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                return _data.Length;
            }
        }

        /// <summary>
        /// A new icon image.
        /// </summary>
        public DeviceIndependentBitmap()
        {

        }

        /// <summary>
        /// A device-independent bitmap.
        /// </summary>
        /// <param name="data">Bitmap data.</param>
        public DeviceIndependentBitmap(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Create a copy of an image.
        /// </summary>
        /// <param name="image">Source image.</param>
        public DeviceIndependentBitmap(DeviceIndependentBitmap image)
        {
            _data = new byte[image._data.Length];
            Buffer.BlockCopy(image._data, 0, _data, 0, image._data.Length);
            _header = image._header;
        }

        /// <summary>
        /// Read icon data.
        /// </summary>
        /// <param name="lpData">Pointer to the beginning of icon data.</param>
        /// <param name="size">Icon data size.</param>
        internal void Read(IntPtr lpData, uint size)
        {
            _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                lpData, typeof(Gdi32.BITMAPINFOHEADER));

            _data = new byte[size];
            Marshal.Copy(lpData, _data, 0, _data.Length);
        }

        /// <summary>
        /// Size of the image mask.
        /// </summary>
        private Int32 MaskImageSize
        {
            get
            {
                return (Int32)(_header.biHeight / 2 * GetDIBRowWidth(_header.biWidth));
            }
        }

        private Int32 XorImageSize
        {
            get
            {
                return (Int32)(_header.biHeight / 2 *
                    GetDIBRowWidth(_header.biWidth * _header.biBitCount * _header.biPlanes));
            }
        }

        /// <summary>
        /// Position of the DIB bitmap bits within a DIB bitmap array.
        /// </summary>
        private Int32 XorImageIndex
        {
            get
            {
                return (Int32)(Marshal.SizeOf(_header) +
                    ColorCount * Marshal.SizeOf(new Gdi32.RGBQUAD()));
            }
        }

        /// <summary>
        /// Number of colors in the palette.
        /// </summary>
        private UInt32 ColorCount
        {
            get
            {
                if (_header.biClrUsed != 0)
                    return _header.biClrUsed;

                if (_header.biBitCount <= 8)
                    return (UInt32)(1 << _header.biBitCount);

                return 0;
            }
        }

        private Int32 MaskImageIndex
        {
            get
            {
                return XorImageIndex + XorImageSize;
            }
        }

        /// <summary>
        /// Returns the width of a row in a DIB Bitmap given the number of bits. DIB Bitmap rows always align on a DWORD boundary.
        /// </summary>
        /// <param name="width">Number of bits.</param>
        /// <returns>Width of a row in bytes.</returns>
        private Int32 GetDIBRowWidth(int width)
        {
            return (Int32)((width + 31) / 32) * 4;
        }

        /// <summary>
        /// Bitmap monochrome mask.
        /// </summary>
        public Bitmap Mask
        {
            get
            {
                if (_mask == null)
                {
                    IntPtr hdc = IntPtr.Zero;
                    IntPtr hBmp = IntPtr.Zero;
                    IntPtr hBmpOld = IntPtr.Zero;
                    IntPtr bitsInfo = IntPtr.Zero;
                    IntPtr bits = IntPtr.Zero;

                    try
                    {
                        // extract monochrome mask
                        hdc = Gdi32.CreateCompatibleDC(IntPtr.Zero);
                        if (hdc == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        hBmp = Gdi32.CreateCompatibleBitmap(hdc, _header.biWidth, _header.biHeight / 2);
                        if (hBmp == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        hBmpOld = Gdi32.SelectObject(hdc, hBmp);

                        // prepare BitmapInfoHeader for mono bitmap:
                        int monoBmHdrSize = (int)_header.biSize + Marshal.SizeOf(new Gdi32.RGBQUAD()) * 2;

                        bitsInfo = Marshal.AllocCoTaskMem(monoBmHdrSize);
                        Marshal.WriteInt32(bitsInfo, Marshal.SizeOf(_header));
                        Marshal.WriteInt32(bitsInfo, 4, _header.biWidth);
                        Marshal.WriteInt32(bitsInfo, 8, _header.biHeight / 2);
                        Marshal.WriteInt16(bitsInfo, 12, 1);
                        Marshal.WriteInt16(bitsInfo, 14, 1);
                        Marshal.WriteInt32(bitsInfo, 16, (Int32)Gdi32.BitmapCompression.BI_RGB);
                        Marshal.WriteInt32(bitsInfo, 20, 0);
                        Marshal.WriteInt32(bitsInfo, 24, 0);
                        Marshal.WriteInt32(bitsInfo, 28, 0);
                        Marshal.WriteInt32(bitsInfo, 32, 0);
                        Marshal.WriteInt32(bitsInfo, 36, 0);
                        // black and white color indices
                        Marshal.WriteInt32(bitsInfo, 40, 0);
                        Marshal.WriteByte(bitsInfo, 44, 255);
                        Marshal.WriteByte(bitsInfo, 45, 255);
                        Marshal.WriteByte(bitsInfo, 46, 255);
                        Marshal.WriteByte(bitsInfo, 47, 0);
                        // prepare mask bits
                        bits = Marshal.AllocCoTaskMem(MaskImageSize);
                        Marshal.Copy(_data, MaskImageIndex, bits, MaskImageSize);

                        if (0 == Gdi32.SetDIBitsToDevice(hdc, 0, 0, (UInt32)_header.biWidth, (UInt32)_header.biHeight / 2,
                            0, 0, 0, (UInt32)_header.biHeight / 2, bits, bitsInfo, (UInt32)Gdi32.DIBColors.DIB_RGB_COLORS))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }

                        _mask = Bitmap.FromHbitmap(hBmp);
                    }
                    finally
                    {
                        if (bits != IntPtr.Zero) Marshal.FreeCoTaskMem(bits);
                        if (bitsInfo != IntPtr.Zero) Marshal.FreeCoTaskMem(bitsInfo);
                        if (hdc != IntPtr.Zero) Gdi32.SelectObject(hdc, hBmpOld);
                        if (hdc != IntPtr.Zero) Gdi32.DeleteObject(hdc);
                    }
                }

                return _mask;
            }
        }

        /// <summary>
        /// Bitmap color (XOR) part of the image.
        /// </summary>
        public Bitmap Color
        {
            get
            {
                if (_color == null)
                {
                    IntPtr hdcDesktop = IntPtr.Zero;
                    IntPtr hdc = IntPtr.Zero;
                    IntPtr hBmp = IntPtr.Zero;
                    IntPtr hBmpOld = IntPtr.Zero;
                    IntPtr bitsInfo = IntPtr.Zero;
                    IntPtr bits = IntPtr.Zero;

                    try
                    {
                        hdcDesktop = User32.GetDC(IntPtr.Zero); // Gdi32.CreateDC("DISPLAY", null, null, IntPtr.Zero);
                        if (hdcDesktop == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        hdc = Gdi32.CreateCompatibleDC(hdcDesktop);
                        if (hdc == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        hBmp = Gdi32.CreateCompatibleBitmap(hdcDesktop, _header.biWidth, _header.biHeight / 2);
                        if (hBmp == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        hBmpOld = Gdi32.SelectObject(hdc, hBmp);

                        // bitmap header
                        bitsInfo = Marshal.AllocCoTaskMem(XorImageIndex);
                        if (bitsInfo == null)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        Marshal.Copy(_data, 0, bitsInfo, XorImageIndex);
                        // fix the height
                        Marshal.WriteInt32(bitsInfo, 8, _header.biHeight / 2);
                        // XOR bits
                        bits = Marshal.AllocCoTaskMem(XorImageSize);
                        Marshal.Copy(_data, XorImageIndex, bits, XorImageSize);

                        if (0 == Gdi32.SetDIBitsToDevice(hdc, 0, 0, (UInt32)_header.biWidth, (UInt32)_header.biHeight / 2,
                           0, 0, 0, (UInt32)(_header.biHeight / 2), bits, bitsInfo, (Int32)Gdi32.DIBColors.DIB_RGB_COLORS))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }

                        _color = Bitmap.FromHbitmap(hBmp);
                    }
                    finally
                    {
                        if (hdcDesktop != IntPtr.Zero) Gdi32.DeleteDC(hdcDesktop);
                        if (bits != IntPtr.Zero) Marshal.FreeCoTaskMem(bits);
                        if (bitsInfo != IntPtr.Zero) Marshal.FreeCoTaskMem(bitsInfo);
                        if (hdc != IntPtr.Zero) Gdi32.SelectObject(hdc, hBmpOld);
                        if (hdc != IntPtr.Zero) Gdi32.DeleteObject(hdc);
                    }
                }

                return _color;
            }
        }

        /// <summary>
        /// Complete image.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                if (_image == null)
                {
                    IntPtr hDCScreen = IntPtr.Zero;
                    IntPtr bits = IntPtr.Zero;
                    IntPtr hDCScreenOUTBmp = IntPtr.Zero;
                    IntPtr hBitmapOUTBmp = IntPtr.Zero;

                    try
                    {
                        hDCScreen = User32.GetDC(IntPtr.Zero);
                        if (hDCScreen == IntPtr.Zero)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        // Image
                        Gdi32.BITMAPINFO bitmapInfo = new Gdi32.BITMAPINFO();
                        bitmapInfo.bmiHeader = _header;
                        // bitmapInfo.bmiColors = Tools.StandarizePalette(mEncoder.Colors);
                        hDCScreenOUTBmp = Gdi32.CreateCompatibleDC(hDCScreen);
                        hBitmapOUTBmp = Gdi32.CreateDIBSection(hDCScreenOUTBmp, ref bitmapInfo, 0, out bits, IntPtr.Zero, 0);
                        Marshal.Copy(_data, XorImageIndex, bits, XorImageSize);
                        _image = Bitmap.FromHbitmap(hBitmapOUTBmp);                        
                    }
                    finally
                    {
                        if (hDCScreen != IntPtr.Zero) User32.ReleaseDC(IntPtr.Zero, hDCScreen);
                        if (hBitmapOUTBmp != IntPtr.Zero) Gdi32.DeleteObject(hBitmapOUTBmp);
                        if (hDCScreenOUTBmp != IntPtr.Zero) Gdi32.DeleteDC(hDCScreenOUTBmp);
                    }
                }

                return _image;
            }
        }
    }
}
