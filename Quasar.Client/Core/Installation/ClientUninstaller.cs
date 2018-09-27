using System;
using System.Diagnostics;
using Quasar.Client.Config;
using Quasar.Client.Core.Helper;
using Quasar.Common.Messages;

namespace Quasar.Client.Core.Installation
{
    public static class ClientUninstaller
    {
        public static void Uninstall(Networking.Client client)
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
                client.Send(new SetStatus {Message = $"Uninstallation failed: {ex.Message}"});
            }
        }
    }
}
