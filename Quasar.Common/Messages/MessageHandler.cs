using Quasar.Common.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.Common.Messages
{
    /// <summary>
    /// Handles registration of <see cref="IMessageProcessor"/>s and processing of <see cref="IMessage"/>s.
    /// </summary>
    public static class MessageHandler
    {
        /// <summary>
        /// List of registered <see cref="IMessageProcessor"/>s.
        /// </summary>
        private static readonly List<IMessageProcessor> Processors = new List<IMessageProcessor>();

        /// <summary>
        /// Used in lock statements to synchronize access to <see cref="Processors"/> between threads.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Registers a <see cref="IMessageProcessor"/> to the available <see cref="Processors"/>.
        /// </summary>
        /// <param name="proc">The <see cref="IMessageProcessor"/> to register.</param>
        public static void Register(IMessageProcessor proc)
        {
            lock (SyncLock)
            {
                if (Processors.Contains(proc)) return;
                Processors.Add(proc);
            }
        }

        /// <summary>
        /// Unregisters a <see cref="IMessageProcessor"/> from the available <see cref="Processors"/>.
        /// </summary>
        /// <param name="proc"></param>
        public static void Unregister(IMessageProcessor proc)
        {
            lock (SyncLock)
            {
                Processors.Remove(proc);
            }
        }

        /// <summary>
        /// Forwards the received <see cref="IMessage"/> to the appropriate <see cref="IMessageProcessor"/>s to execute it.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="msg">The received message.</param>
        public static void Process(ISender sender, IMessage msg)
        {
            IEnumerable<IMessageProcessor> availableProcessors;
            lock (SyncLock)
            {
                // select appropriate message processors
                availableProcessors = Processors.Where(x => x.CanExecute(msg) && x.CanExecuteFrom(sender)).ToList();
                // ToList() is required to retrieve a thread-safe enumerator representing a moment-in-time snapshot of the message processors
            }

            foreach (var executor in availableProcessors)
                executor.Execute(sender, msg);
        }
    }
}
