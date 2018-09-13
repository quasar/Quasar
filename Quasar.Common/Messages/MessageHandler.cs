using Quasar.Common.Networking;
using System.Collections.Concurrent;
using System.Linq;

namespace Quasar.Common.Messages
{
    /// <summary>
    /// Handles registration of <see cref="IMessageProcessor"/>s and processing of <see cref="IMessage"/>s.
    /// </summary>
    public static class MessageHandler
    {
        /// <summary>
        /// Unordered thread-safe list of registered <see cref="IMessageProcessor"/>s.
        /// </summary>
        private static readonly ConcurrentBag<IMessageProcessor> Processors = new ConcurrentBag<IMessageProcessor>();

        /// <summary>
        /// Registers a <see cref="IMessageProcessor"/> to the available <see cref="Processors"/>.
        /// </summary>
        /// <param name="proc">The <see cref="IMessageProcessor"/> to register.</param>
        public static void Register(IMessageProcessor proc)
        {
            if (Processors.Contains(proc)) return;
            Processors.Add(proc);
        }

        /// <summary>
        /// Unregisters a <see cref="IMessageProcessor"/> from the available <see cref="Processors"/>.
        /// </summary>
        /// <param name="proc"></param>
        public static void Unregister(IMessageProcessor proc)
        {
            if (!Processors.Contains(proc)) return;
            Processors.TryTake(out proc);
        }

        /// <summary>
        /// Forwards the received <see cref="IMessage"/> to the appropriate <see cref="IMessageProcessor"/>s to execute it.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="msg">The received message.</param>
        public static void Process(ISender sender, IMessage msg)
        {
            // select appropriate message processors
            var availableExecutors = Processors.Where(x => x.CanExecute(msg) && x.CanExecuteFrom(sender));

            foreach (var executor in availableExecutors)
                executor.Execute(sender, msg);
        }
    }
}
