using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using xServer.Core.Misc;

namespace xServer.Core.Helper
{
    public class UnsafeStreamCodec
    {
        private int _ImageQuality;
        public int ImageQuality
        {
            get { return _ImageQuality; }
            private set
            {
                lock (ImageProcessLock)
                {
                    _ImageQuality = value;
                    jpgCompression = new JpgCompression(_ImageQuality);
                }
            }
        }


        public Size CheckBlock { get; private set; }
        private byte[] EncodeBuffer;
        private Bitmap decodedBitmap;
        private PixelFormat EncodedFormat;
        private int EncodedWidth;
        private int EncodedHeight;
        private object ImageProcessLock = new object();
        private JpgCompression jpgCompression;

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
        /// <param name="ImageQuality">The quality to use between 0-100</param>
        public UnsafeStreamCodec(int ImageQuality = 100, bool UseJPEG = true)
        {
            this.CheckBlock = new Size(50, 1);
        }

        public unsafe void CodeImage(IntPtr Scan0, Rectangle ScanArea, Size ImageSize, PixelFormat Format, Stream outStream)
        {
            lock (ImageProcessLock)
            {
                byte* pScan0 = (byte*)Scan0.ToInt32();
                if (!outStream.CanWrite)
                    throw new Exception("Must have access to Write in the Stream");

                int Stride = 0;
                int RawLength = 0;
                int PixelSize = 0;

                switch (Format)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                        PixelSize = 3;
                        break;
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                        PixelSize = 4;
                        break;
                    default:
                        throw new NotSupportedException(Format.ToString());
                }

                Stride = ImageSize.Width * PixelSize;
                RawLength = Stride * ImageSize.Height;

                if (EncodeBuffer == null)
                {
                    this.EncodedFormat = Format;
                    this.EncodedWidth = ImageSize.Width;
                    this.EncodedHeight = ImageSize.Height;
                    this.EncodeBuffer = new byte[RawLength];
                    fixed (byte* ptr = EncodeBuffer)
                    {
                        byte[] temp = null;
                        using (Bitmap TmpBmp = new Bitmap(ImageSize.Width, ImageSize.Height, Stride, Format, Scan0))
                        {
                            temp = jpgCompression.Compress(TmpBmp);
                        }

                        outStream.Write(BitConverter.GetBytes(temp.Length), 0, 4);
                        outStream.Write(temp, 0, temp.Length);
                        memcpy(new IntPtr(ptr), Scan0, (uint)RawLength);
                    }
                    return;
                }

                long oldPos = outStream.Position;
                outStream.Write(new byte[4], 0, 4);
                int TotalDataLength = 0;

                if (this.EncodedFormat != Format)
                    throw new Exception("PixelFormat is not equal to previous Bitmap");

                if (this.EncodedWidth != ImageSize.Width || this.EncodedHeight != ImageSize.Height)
                    throw new Exception("Bitmap width/height are not equal to previous bitmap");

                List<Rectangle> Blocks = new List<Rectangle>();
                int index = 0;

                Size s = new Size(ScanArea.Width, CheckBlock.Height);
                Size lastSize = new Size(ScanArea.Width % CheckBlock.Width, ScanArea.Height % CheckBlock.Height);

                int lasty = ScanArea.Height - lastSize.Height;
                int lastx = ScanArea.Width - lastSize.Width;

                Rectangle cBlock = new Rectangle();
                List<Rectangle> finalUpdates = new List<Rectangle>();

                s = new Size(ScanArea.Width, s.Height);
                fixed (byte* encBuffer = EncodeBuffer)
                {
                    for (int y = ScanArea.Y; y != ScanArea.Height; )
                    {
                        if (y == lasty)
                            s = new Size(ScanArea.Width, lastSize.Height);
                        cBlock = new Rectangle(ScanArea.X, y, ScanArea.Width, s.Height);
                        
                        int offset = (y * Stride) + (ScanArea.X * PixelSize);
                        if (memcmp(encBuffer + offset, pScan0 + offset, (uint)Stride) != 0)
                        {
                            index = Blocks.Count - 1;
                            if (Blocks.Count != 0 && (Blocks[index].Y + Blocks[index].Height) == cBlock.Y)
                            {
                                cBlock = new Rectangle(Blocks[index].X, Blocks[index].Y, Blocks[index].Width, Blocks[index].Height + cBlock.Height);
                                Blocks[index] = cBlock;
                            }
                            else
                            {
                                Blocks.Add(cBlock);
                            }
                        }
                        y += s.Height;
                    }

                    for (int i = 0, x = ScanArea.X; i < Blocks.Count; i++)
                    {
                        s = new Size(CheckBlock.Width, Blocks[i].Height);
                        x = ScanArea.X;
                        while (x != ScanArea.Width)
                        {
                            if (x == lastx)
                                s = new Size(lastSize.Width, Blocks[i].Height);

                            cBlock = new Rectangle(x, Blocks[i].Y, s.Width, Blocks[i].Height);
                            bool FoundChanges = false;
                            int blockStride = PixelSize * cBlock.Width;

                            for (int j = 0; j < cBlock.Height; j++)
                            {
                                int blockOffset = (Stride * (cBlock.Y + j)) + (PixelSize * cBlock.X);
                                if (memcmp(encBuffer + blockOffset, pScan0 + blockOffset, (uint)blockStride) != 0)
                                    FoundChanges = true;
                                memcpy(encBuffer + blockOffset, pScan0 + blockOffset, (uint)blockStride); //copy-changes
                            }

                            if (FoundChanges)
                            {
                                index = finalUpdates.Count - 1;
                                if (finalUpdates.Count > 0 && (finalUpdates[index].X + finalUpdates[index].Width) == cBlock.X)
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
                    int blockStride = PixelSize * rect.Width;

                    Bitmap TmpBmp = new Bitmap(rect.Width, rect.Height, Format);
                    BitmapData TmpData = TmpBmp.LockBits(new Rectangle(0, 0, TmpBmp.Width, TmpBmp.Height), ImageLockMode.ReadWrite, TmpBmp.PixelFormat);
                    for (int j = 0, offset = 0; j < rect.Height; j++)
                    {
                        int blockOffset = (Stride * (rect.Y + j)) + (PixelSize * rect.X);
                        memcpy((byte*)TmpData.Scan0.ToPointer() + offset, pScan0 + blockOffset, (uint)blockStride); //copy-changes
                        offset += blockStride;
                    }
                    TmpBmp.UnlockBits(TmpData);

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

                    jpgCompression.Compress(TmpBmp, ref outStream);

                    length = outStream.Position - length;

                    outStream.Position = OldPos - 4;
                    outStream.Write(BitConverter.GetBytes((int)length), 0, 4);
                    outStream.Position += length;
                    TmpBmp.Dispose();
                    TotalDataLength += (int)length + (4 * 5);
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
                outStream.Write(BitConverter.GetBytes(TotalDataLength), 0, 4);
                Blocks.Clear();
                finalUpdates.Clear();
            }
        }

        public unsafe Bitmap DecodeData(IntPtr CodecBuffer, uint Length)
        {
            if (Length < 4)
                return decodedBitmap;

            int DataSize = *(int*)(CodecBuffer);
            if (decodedBitmap == null)
            {
                byte[] temp = new byte[DataSize];
                fixed (byte* tempPtr = temp)
                {
                    memcpy(new IntPtr(tempPtr), new IntPtr(CodecBuffer.ToInt32() + 4), (uint)DataSize);
                }

                this.decodedBitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(temp));
                return decodedBitmap;
            }
            return decodedBitmap;
            byte* bufferPtr = (byte*)CodecBuffer.ToInt32();
            if (DataSize > 0)
            {
                Graphics g = Graphics.FromImage(decodedBitmap);
                for (int i = 4; DataSize > 0; )
                {
                    Rectangle rect = new Rectangle(*(int*)(bufferPtr + i), *(int*)(bufferPtr + i + 4),
                                                   *(int*)(bufferPtr + i + 8), *(int*)(bufferPtr + i + 12));
                    int UpdateLen = *(int*)(bufferPtr + i + 16);
                    byte[] temp = new byte[UpdateLen];

                    fixed (byte* tempPtr = temp)
                    {
                        memcpy(new IntPtr(tempPtr), new IntPtr(CodecBuffer.ToInt32() + i + 20), (uint)UpdateLen);
                        using (Bitmap TmpBmp = new Bitmap(rect.Width, rect.Height, rect.Width * 3, decodedBitmap.PixelFormat, new IntPtr(tempPtr)))
                        {
                            g.DrawImage(TmpBmp, new Point(rect.X, rect.Y));
                        }
                    }
                    DataSize -= UpdateLen + (4 * 5);
                    i += UpdateLen + (4 * 5);
                }
                g.Dispose();
            }
            return decodedBitmap;
        }

        public Bitmap DecodeData(Stream inStream)
        {
            byte[] temp = new byte[4];
            inStream.Read(temp, 0, 4);
            int DataSize = BitConverter.ToInt32(temp, 0);

            if (decodedBitmap == null)
            {
                temp = new byte[DataSize];
                inStream.Read(temp, 0, temp.Length);
                this.decodedBitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(temp));
                return decodedBitmap;
            }

            using (Graphics g = Graphics.FromImage(decodedBitmap))
            {
                while (DataSize > 0)
                {
                    byte[] tempData = new byte[4 * 5];
                    inStream.Read(tempData, 0, tempData.Length);

                    Rectangle rect = new Rectangle(BitConverter.ToInt32(tempData, 0), BitConverter.ToInt32(tempData, 4),
                                         BitConverter.ToInt32(tempData, 8), BitConverter.ToInt32(tempData, 12));
                    int UpdateLen = BitConverter.ToInt32(tempData, 16);
                    tempData = null;

                    byte[] buffer = new byte[UpdateLen];
                    inStream.Read(buffer, 0, buffer.Length);

                    using (MemoryStream m = new MemoryStream(buffer))
                    using (Bitmap tmp = (Bitmap)Image.FromStream(m))
                    {
                        g.DrawImage(tmp, rect.Location);
                    }
                    buffer = null;
                    DataSize -= UpdateLen + (4 * 5);
                }
            }
            return decodedBitmap;
        }
    }
}
