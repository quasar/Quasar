using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using xServer.Core.Packets;
using xServer.Core.ReverseProxy.Packets;

namespace xServer.Core.ReverseProxy
{
    public class ReverseProxyCommandHandler
    {
        public static void HandleCommand(Client client, IPacket packet)
        {
            var type = packet.GetType();
            if (type == typeof (ReverseProxyConnectResponse))
            {
                ReverseProxyConnectResponse response = (ReverseProxyConnectResponse) packet;
                if (client.Value.ProxyServer != null)
                {
                    ReverseProxyClient socksClient =
                        client.Value.ProxyServer.GetClientByConnectionId(response.ConnectionId);
                    if (socksClient != null)
                    {
                        socksClient.CommandResponse(response);
                    }
                }
            }
            else if (type == typeof (ReverseProxyData))
            {
                ReverseProxyData dataCommand = (ReverseProxyData) packet;
                ReverseProxyClient socksClient =
                    client.Value.ProxyServer.GetClientByConnectionId(dataCommand.ConnectionId);

                if (socksClient != null)
                {
                    socksClient.SendToClient(dataCommand.Data);
                }
            }
            else if (type == typeof (ReverseProxyDisconnect))
            {
                ReverseProxyDisconnect disconnectCommand = (ReverseProxyDisconnect) packet;
                ReverseProxyClient socksClient =
                    client.Value.ProxyServer.GetClientByConnectionId(disconnectCommand.ConnectionId);

                if (socksClient != null)
                {
                    socksClient.Disconnect();
                }
            }
        }
    }
}