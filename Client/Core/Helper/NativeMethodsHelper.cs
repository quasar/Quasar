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

        public static void DoMouseClickLeft(Point p, bool doubleClick)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
            NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
            NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
            if (doubleClick)
            {
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
            }
        }

        public static void DoMouseClickRight(Point p, bool doubleClick)
        {
            NativeMethods.SetCursorPos(p.X, p.Y);
            NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
            NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
            if (doubleClick)
            {
                NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
                NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
            }
        }
    }
}
