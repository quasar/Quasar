using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using xServer.Core.Compression;

namespace xServer.Core.Helper
{
    public class UnsafeStreamCodec
    {
        private int _imageQuality;

        public int ImageQuality
        {
            get { return _imageQuality; }
            private set
            {
                lock (_imageProcessLock)
                {
                    _imageQuality = value;
                    _jpgCompression = new JpgCompression(_imageQuality);
                }
            }
        }

        public Size CheckBlock { get; private set; }
        private byte[] _encodeBuffer;
        private Bitmap _decodedBitmap;
        private PixelFormat _encodedFormat;
        private int _encodedWidth;
        private int _encodedHeight;
        private object _imageProcessLock = new object();
        private JpgCompression _jpgCompression;

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int memcmp(byte* ptr1, byte* ptr2, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(IntPtr ptr1, IntPtr ptr2, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcpy(IntPtr dst, IntPtr src, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int memcpy(void* dst, void* src, uint count);

        /// <summary>
        /// Initialize a new object of UnsafeStreamCodec
        /// </summary>
        /// <param name="imageQuality">The quality to use between 0-100</param>
        public UnsafeStreamCodec(int imageQuality = 100)
        {
            this.CheckBlock = new Size(50, 1);
        }

        public unsafe void CodeImage(IntPtr scan0, Rectangle scanArea, Size imageSize, PixelFormat format,
            Stream outStream)
        {
            lock (_imageProcessLock)
            {
                byte* pScan0 = (byte*) scan0.ToInt32();
                if (!outStream.CanWrite)
                    throw new Exception("Must have access to Write in the Stream");

                int stride = 0;
                int rawLength = 0;
                int pixelSize = 0;

                switch (format)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                        pixelSize = 3;
                        break;
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                        pixelSize = 4;
                        break;
                    default:
                        throw new NotSupportedException(format.ToString());
                }

                stride = imageSize.Width*pixelSize;
                rawLength = stride*imageSize.Height;

                if (_encodeBuffer == null)
                {
                    this._encodedFormat = format;
                    this._encodedWidth = imageSize.Width;
                    this._encodedHeight = imageSize.Height;
                    this._encodeBuffer = new byte[rawLength];
                    fixed (byte* ptr = _encodeBuffer)
                    {
                        byte[] temp = null;
                        using (Bitmap tmpBmp = new Bitmap(imageSize.Width, imageSize.Height, stride, format, scan0))
                        {
                            temp = _jpgCompression.Compress(tmpBmp);
                        }

                        outStream.Write(BitConverter.GetBytes(temp.Length), 0, 4);
                        outStream.Write(temp, 0, temp.Length);
                        memcpy(new IntPtr(ptr), scan0, (uint) rawLength);
                    }
                    return;
                }

                long oldPos = outStream.Position;
                outStream.Write(new byte[4], 0, 4);
                int totalDataLength = 0;

                if (this._encodedFormat != format)
                    throw new Exception("PixelFormat is not equal to previous Bitmap");

                if (this._encodedWidth != imageSize.Width || this._encodedHeight != imageSize.Height)
                    throw new Exception("Bitmap width/height are not equal to previous bitmap");

                List<Rectangle> blocks = new List<Rectangle>();

                Size s = new Size(scanArea.Width, CheckBlock.Height);
                Size lastSize = new Size(scanArea.Width%CheckBlock.Width, scanArea.Height%CheckBlock.Height);

                int lasty = scanArea.Height - lastSize.Height;
                int lastx = scanArea.Width - lastSize.Width;

                Rectangle cBlock = new Rectangle();
                List<Rectangle> finalUpdates = new List<Rectangle>();

                s = new Size(scanArea.Width, s.Height);
                fixed (byte* encBuffer = _encodeBuffer)
                {
                    var index = 0;

                    for (int y = scanArea.Y; y != scanArea.Height;)
                    {
                        if (y == lasty)
                            s = new Size(scanArea.Width, lastSize.Height);
                        cBlock = new Rectangle(scanArea.X, y, scanArea.Width, s.Height);

                        int offset = (y*stride) + (scanArea.X*pixelSize);
                        if (memcmp(encBuffer + offset, pScan0 + offset, (uint) stride) != 0)
                        {
                            index = blocks.Count - 1;
                            if (blocks.Count != 0 && (blocks[index].Y + blocks[index].Height) == cBlock.Y)
                            {
                                cBlock = new Rectangle(blocks[index].X, blocks[index].Y, blocks[index].Width,
                                    blocks[index].Height + cBlock.Height);
                                blocks[index] = cBlock;
                            }
                            else
                            {
                                blocks.Add(cBlock);
                            }
                        }
                        y += s.Height;
                    }

                    for (int i = 0; i < blocks.Count; i++)
                    {
                        s = new Size(CheckBlock.Width, blocks[i].Height);
                        int x = scanArea.X;
                        while (x != scanArea.Width)
                        {
                            if (x == lastx)
                                s = new Size(lastSize.Width, blocks[i].Height);

                            cBlock = new Rectangle(x, blocks[i].Y, s.Width, blocks[i].Height);
                            bool foundChanges = false;
                            int blockStride = pixelSize*cBlock.Width;

                            for (int j = 0; j < cBlock.Height; j++)
                            {
                                int blockOffset = (stride*(cBlock.Y + j)) + (pixelSize*cBlock.X);
                                if (memcmp(encBuffer + blockOffset, pScan0 + blockOffset, (uint) blockStride) != 0)
                                    foundChanges = true;
                                memcpy(encBuffer + blockOffset, pScan0 + blockOffset, (uint) blockStride);
                                    //copy-changes
                            }

                            if (foundChanges)
                            {
                                index = finalUpdates.Count - 1;
                                if (finalUpdates.Count > 0 &&
                                    (finalUpdates[index].X + finalUpdates[index].Width) == cBlock.X)
                                {
                                    Rectangle rect = finalUpdates[index];
                                    int newWidth = cBlock.Width + rect.Width;
                                    cBlock = new Rectangle(rect.X, rect.Y, newWidth, rect.Height);
                                    finalUpdates[index] = cBlock;
                                }
                                else
                                {
                                    finalUpdates.Add(cBlock);
                                }
                            }
                            x += s.Width;
                        }
                    }
                }

                /*int maxHeight = 0;
                int maxWidth = 0;

                for (int i = 0; i < finalUpdates.Count; i++)
                {
                    if (finalUpdates[i].Height > maxHeight)
                        maxHeight = finalUpdates[i].Height;
                    maxWidth += finalUpdates[i].Width;
                }

                Bitmap bmp = new Bitmap(maxWidth+1, maxHeight+1);
                int XOffset = 0;*/

                for (int i = 0; i < finalUpdates.Count; i++)
                {
                    Rectangle rect = finalUpdates[i];
                    int blockStride = pixelSize*rect.Width;

                    Bitmap tmpBmp = new Bitmap(rect.Width, rect.Height, format);
                    BitmapData tmpData = tmpBmp.LockBits(new Rectangle(0, 0, tmpBmp.Width, tmpBmp.Height),
                        ImageLockMode.ReadWrite, tmpBmp.PixelFormat);
                    for (int j = 0, offset = 0; j < rect.Height; j++)
                    {
                        int blockOffset = (stride*(rect.Y + j)) + (pixelSize*rect.X);
                        memcpy((byte*) tmpData.Scan0.ToPointer() + offset, pScan0 + blockOffset, (uint) blockStride);
                            //copy-changes
                        offset += blockStride;
                    }
                    tmpBmp.UnlockBits(tmpData);

                    /*using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(TmpBmp, new Point(XOffset, 0));
                    }
                    XOffset += TmpBmp.Width;*/

                    outStream.Write(BitConverter.GetBytes(rect.X), 0, 4);
                    outStream.Write(BitConverter.GetBytes(rect.Y), 0, 4);
                    outStream.Write(BitConverter.GetBytes(rect.Width), 0, 4);
                    outStream.Write(BitConverter.GetBytes(rect.Height), 0, 4);
                    outStream.Write(new byte[4], 0, 4);

                    long length = outStream.Length;
                    long OldPos = outStream.Position;

                    _jpgCompression.Compress(tmpBmp, ref outStream);

                    length = outStream.Position - length;

                    outStream.Position = OldPos - 4;
                    outStream.Write(BitConverter.GetBytes((int) length), 0, 4);
                    outStream.Position += length;
                    tmpBmp.Dispose();
                    totalDataLength += (int) length + (4*5);
                }

                /*if (finalUpdates.Count > 0)
                {
                    byte[] lele = base.jpgCompression.Compress(bmp);
                    byte[] compressed = new SafeQuickLZ().compress(lele, 0, lele.Length, 1);
                    bool Won = lele.Length < outStream.Length;
                    bool CompressWon = compressed.Length < outStream.Length;
                    Console.WriteLine(Won + ", " + CompressWon);
                }
                bmp.Dispose();*/

                outStream.Position = oldPos;
                outStream.Write(BitConverter.GetBytes(totalDataLength), 0, 4);
                blocks.Clear();
                finalUpdates.Clear();
            }
        }

        public unsafe Bitmap DecodeData(IntPtr codecBuffer, uint length)
        {
            if (length < 4)
                return _decodedBitmap;

            int dataSize = *(int*) (codecBuffer);
            if (_decodedBitmap == null)
            {
                byte[] temp = new byte[dataSize];
                fixed (byte* tempPtr = temp)
                {
                    memcpy(new IntPtr(tempPtr), new IntPtr(codecBuffer.ToInt32() + 4), (uint) dataSize);
                }

                this._decodedBitmap = (Bitmap) Bitmap.FromStream(new MemoryStream(temp));
                return _decodedBitmap;
            }
            return _decodedBitmap;
        }

        public Bitmap DecodeData(Stream inStream)
        {
            byte[] temp = new byte[4];
            inStream.Read(temp, 0, 4);
            int dataSize = BitConverter.ToInt32(temp, 0);

            if (_decodedBitmap == null)
            {
                temp = new byte[dataSize];
                inStream.Read(temp, 0, temp.Length);
                this._decodedBitmap = (Bitmap) Bitmap.FromStream(new MemoryStream(temp));
                return _decodedBitmap;
            }

            using (Graphics g = Graphics.FromImage(_decodedBitmap))
            {
                while (dataSize > 0)
                {
                    byte[] tempData = new byte[4*5];
                    inStream.Read(tempData, 0, tempData.Length);

                    Rectangle rect = new Rectangle(BitConverter.ToInt32(tempData, 0), BitConverter.ToInt32(tempData, 4),
                        BitConverter.ToInt32(tempData, 8), BitConverter.ToInt32(tempData, 12));
                    int updateLen = BitConverter.ToInt32(tempData, 16);
                    tempData = null;

                    byte[] buffer = new byte[updateLen];
                    inStream.Read(buffer, 0, buffer.Length);

                    using (MemoryStream m = new MemoryStream(buffer))
                    using (Bitmap tmp = (Bitmap) Image.FromStream(m))
                    {
                        g.DrawImage(tmp, rect.Location);
                    }
                    buffer = null;
                    dataSize -= updateLen + (4*5);
                }
            }
            return _decodedBitmap;
        }
    }
}