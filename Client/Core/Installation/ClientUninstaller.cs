using System;
using System.Diagnostics;
using xClient.Config;
using xClient.Core.Helper;
using xClient.Core.Networking;

namespace xClient.Core.Installation
{
    public static class ClientUninstaller
    {
        public static void Uninstall(Client client)
        {
            try
            {
                if (Settings.STARTUP)
                    Startup.RemoveFromStartup();

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
    }
}
