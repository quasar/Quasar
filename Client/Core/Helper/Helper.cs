using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Core
{
    class Helper
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static Random _rnd = new Random(Environment.TickCount);

        public static string GetRandomFilename(int length, string extension)
        {
            char[] tempChars = new char[length];
            for (int i = 0; i < length; i++)
                tempChars[i] = CHARS[_rnd.Next(CHARS.Length)];

            return new string(tempChars) + extension;
        }

        public static string GetRandomName(int length)
        {
            char[] tempChars = new char[length];
            for (int i = 0; i < length; i++)
                tempChars[i] = CHARS[_rnd.Next(CHARS.Length)];

            return new string(tempChars);
        }

        public static byte[] CImgToByte(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static Bitmap GetDesktop(int Mode, int Number)
        {
            Rectangle bounds;
            Bitmap screenshot;
            Graphics graph;

            bounds = Screen.AllScreens[Number].Bounds;
            screenshot = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            graph = Graphics.FromImage(screenshot);

            if (Mode == 1)
                graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            else if (Mode == 2)
                graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            graph.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);

            return screenshot;
        }

        public static unsafe Bitmap GetDiffDesktop(Bitmap bmp, Bitmap bmp2)
        {
            if (bmp.Width != bmp2.Width || bmp.Height != bmp2.Height)
                throw new Exception("Sizes must be equal.");

            Bitmap bmpRes = null;

            System.Drawing.Imaging.BitmapData bmData = null;
            System.Drawing.Imaging.BitmapData bmData2 = null;
            System.Drawing.Imaging.BitmapData bmDataRes = null;

            try
            {
                bmpRes = new Bitmap(bmp.Width, bmp.Height);

                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmDataRes = bmpRes.LockBits(new Rectangle(0, 0, bmpRes.Width, bmpRes.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr scan0 = bmData.Scan0;
                IntPtr scan02 = bmData2.Scan0;
                IntPtr scan0Res = bmDataRes.Scan0;

                int stride = bmData.Stride;
                int stride2 = bmData2.Stride;
                int strideRes = bmDataRes.Stride;

                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                for (int y = 0; y < nHeight; y++)
                {
                    //define the pointers inside the first loop for parallelizing
                    byte* p = (byte*)scan0.ToPointer();
                    p += y * stride;
                    byte* p2 = (byte*)scan02.ToPointer();
                    p2 += y * stride2;
                    byte* pRes = (byte*)scan0Res.ToPointer();
                    pRes += y * strideRes;

                    for (int x = 0; x < nWidth; x++)
                    {
                        //always get the complete pixel when differences are found
                        if (p[0] != p2[0] || p[1] != p2[1] || p[2] != p2[2])
                        {
                            pRes[0] = p2[0];
                            pRes[1] = p2[1];
                            pRes[2] = p2[2];

                            //alpha (opacity)
                            pRes[3] = p2[3];
                        }

                        p += 4;
                        p2 += 4;
                        pRes += 4;
                    }
                }

                bmp.UnlockBits(bmData);
                bmp2.UnlockBits(bmData2);
                bmpRes.UnlockBits(bmDataRes);
            }
            catch
            {
                if (bmData != null)
                {
                    try
                    {
                        bmp.UnlockBits(bmData);
                    }
                    catch
                    { }
                }

                if (bmData2 != null)
                {
                    try
                    {
                        bmp2.UnlockBits(bmData2);
                    }
                    catch
                    { }
                }

                if (bmDataRes != null)
                {
                    try
                    {
                        bmpRes.UnlockBits(bmDataRes);
                    }
                    catch
                    { }
                }

                if (bmpRes != null)
                {
                    bmpRes.Dispose();
                    bmpRes = null;
                }
            }

            return bmpRes;
        }

        public static bool IsWindowsXP()
        {
            var OsVersion = Environment.OSVersion.Version;
            return OsVersion.Major == 5 && OsVersion.Minor >= 1;
        }
    }
}
