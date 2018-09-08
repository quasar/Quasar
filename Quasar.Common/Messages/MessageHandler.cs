using Quasar.Common.Networking;
using System.Collections.Concurrent;
using System.Linq;

namespace Quasar.Common.Messages
{
    public static class MessageHandler
    {
        private static readonly ConcurrentBag<IMessageProcessor> Processors = new ConcurrentBag<IMessageProcessor>();

        public static void Register(IMessageProcessor cmd)
        {
            if (Processors.Contains(cmd)) return;
            Processors.Add(cmd);
        }

        public static void Unregister(IMessageProcessor cmd)
        {
            if (!Processors.Contains(cmd)) return;
            Processors.TryTake(out cmd);
        }

        public static void Process(ISender sender, IMessage cmd)
        {
            var availableExecutors = Processors.Where(x => x.CanExecute(cmd) && x.CanExecuteFrom(sender));

            foreach (var executor in availableExecutors)
                executor.Execute(sender, cmd);
        }
    }
}
