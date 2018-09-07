using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Quasar.Common.Networking;

namespace Quasar.Common.Messages
{
    //public class ShutdownResponse : IMessage
    //{
    //    public int Id { get; set; }
    //}

    //public class RestartResponse : IMessage
    //{
    //    public int Id { get; set; }
    //}

    //public class SystemActions : AbstractMessageProcessor
    //{
    //    public void Execute(ISender sender, RestartResponse message)
    //    {
    //        OnReportProgress("RestartResponse");
    //        Console.WriteLine(message.Id);
    //    }

    //    public void Execute(ISender sender, ShutdownResponse message)
    //    {
    //        OnReportProgress("ShutdownResponse");
    //        Console.WriteLine(message.Id);
    //    }

    //    public override bool CanExecute(IMessage message) => message is RestartResponse || message is ShutdownResponse;

    //    /// <summary>
    //    /// Checks whether this message processor can process messages from a specific sender.
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <returns></returns>
    //    public override bool CanExecuteFrom(ISender sender) => true;

    //    public override void Execute(ISender sender, IMessage message)
    //    {
    //        Console.WriteLine("Generic");
    //        switch (message)
    //        {
    //            case ShutdownResponse s:
    //                Execute(sender, s);
    //                break;
    //            case RestartResponse r:
    //                Execute(sender, r);
    //                break;
    //        }
    //    }
    //}

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

        public static void NotifyDisconnect(ISender sender)
        {
            foreach (var cmd in Processors)
            {
                cmd.NotifyDisconnect(sender);
            }
        }
    }
}
