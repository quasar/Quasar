using System.Drawing;
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

        public static void DoMouseEventScroll(Point p, bool scrollDown)
        {
            NativeMethods.mouse_event(MOUSEEVENTF_WHEEL, p.X, p.Y, scrollDown ? -120 : 120, 0);
        }
    }
}
