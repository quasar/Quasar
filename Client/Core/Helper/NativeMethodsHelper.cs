using System.Drawing;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class NativeMethodsHelper
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;
        private const int MOUSEEVENTF_WHEEL = 0x0800;

        public static void DoMouseEventLeft(Point p, bool isMouseDown)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
            if (isMouseDown)
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
            else
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseEventRight(Point p, bool isMouseDown)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
            if (isMouseDown)
                NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
            else
                NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseMoveCursor(Point p)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
        }
    }
}
