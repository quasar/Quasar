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

        public static void DoMouseEventLeft(Point p, bool isMouseDown)
        {
            NativeMethods.mouse_event(isMouseDown ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseEventRight(Point p, bool isMouseDown)
        {
            NativeMethods.mouse_event(isMouseDown ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
        }

        public static void DoMouseMoveCursor(Point p)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
        }
    }
}
