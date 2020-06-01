using Quasar.Client.Networking;
using Quasar.Common;
using Quasar.Common.Enums;
using Quasar.Common.Extensions;
using Quasar.Common.Helpers;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;

namespace Quasar.Client.Messages
{
    public class FileManagerHandler : NotificationMessageProcessor, IDisposable
    {
        private readonly ConcurrentDictionary<int, FileSplit> _activeTransfers = new ConcurrentDictionary<int, FileSplit>();
        private readonly Semaphore _limitThreads = new Semaphore(2, 2); // maximum simultaneous file downloads

        private readonly QuasarClient _client;

        private CancellationTokenSource _tokenSource;

        private CancellationToken _token;

        public FileManagerHandler(QuasarClient client)
        {
            _client = client;
            _client.ClientState += OnClientStateChange;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
        }

        private void OnClientStateChange(Networking.Client s, bool connected)
        {
            switch (connected)
            {
                case true:

                    _tokenSource?.Dispose();
                    _tokenSource = new CancellationTokenSource();
                    _token = _tokenSource.Token;
                    break;
                case false:
                    // cancel all running transfers on disconnect
                    _tokenSource.Cancel();
                    break;
            }
        }

        public override bool CanExecute(IMessage message) => message is GetDrives ||
                                                             message is GetDirectory ||
                                                             message is FileTransferRequest ||
                                                             message is FileTransferCancel ||
                                                             message is FileTransferChunk ||
                                                             message is DoPathDelete ||
                                                             message is DoPathRename;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetDrives msg:
                    Execute(sender, msg);
                    break;
                case GetDirectory msg:
                    Execute(sender, msg);
                    break;
                case FileTransferRequest msg:
                    Execute(sender, msg);
                    break;
                case FileTransferCancel msg:
                    Execute(sender, msg);
                    break;
                case FileTransferChunk msg:
                    Execute(sender, msg);
                    break;
                case DoPathDelete msg:
                    Execute(sender, msg);
                    break;
                case DoPathRename msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, GetDrives command)
        {
            DriveInfo[] driveInfos;
            try
            {
                driveInfos = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            }
            catch (IOException)
            {
                client.Send(new SetStatusFileManager { Message = "GetDrives I/O error", SetLastDirectorySeen = false });
                return;
            }
            catch (UnauthorizedAccessException)
            {
                client.Send(new SetStatusFileManager { Message = "GetDrives No permission", SetLastDirectorySeen = false });
                return;
            }

            if (driveInfos.Length == 0)
            {
                client.Send(new SetStatusFileManager { Message = "GetDrives No drives", SetLastDirectorySeen = false });
                return;
            }

            Drive[] drives = new Drive[driveInfos.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                try
                {
                    var displayName = !string.IsNullOrEmpty(driveInfos[i].VolumeLabel)
                        ? string.Format("{0} ({1}) [{2}, {3}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].VolumeLabel,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat)
                        : string.Format("{0} [{1}, {2}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat);

                    drives[i] = new Drive
                    { DisplayName = displayName, RootDirectory = driveInfos[i].RootDirectory.FullName };
                }
                catch (Exception)
                {

                }
            }

            client.Send(new GetDrivesResponse { Drives = drives });
        }

        private void Execute(ISender client, GetDirectory message)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(message.RemotePath);

                FileInfo[] files = dicInfo.GetFiles();
                DirectoryInfo[] directories = dicInfo.GetDirectories();

                FileSystemEntry[] items = new FileSystemEntry[files.Length + directories.Length];

                int offset = 0;
                for (int i = 0; i < directories.Length; i++, offset++)
                {
                    items[i] = new FileSystemEntry
                    {
                        EntryType = FileType.Directory,
                        Name = directories[i].Name,
                        Size = 0,
                        LastAccessTimeUtc = directories[i].LastAccessTimeUtc
                    };
                }

                for (int i = 0; i < files.Length; i++)
                {
                    items[i + offset] = new FileSystemEntry
                    {
                        EntryType = FileType.File,
                        Name = files[i].Name,
                        Size = files[i].Length,
                        ContentType = Path.GetExtension(files[i].Name).ToContentType(),
                        LastAccessTimeUtc = files[i].LastAccessTimeUtc
                    };
                }

                client.Send(new GetDirectoryResponse { RemotePath = message.RemotePath, Items = items });
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
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    client.Send(new SetStatusFileManager { Message = statusMessage, SetLastDirectorySeen = true });
            }
        }

        private void Execute(ISender client, FileTransferRequest message)
        {
            new Thread(() =>
            {
                _limitThreads.WaitOne();
                try
                {
                    using (var srcFile = new FileSplit(message.RemotePath, FileAccess.Read))
                    {
                        _activeTransfers[message.Id] = srcFile;
                        OnReport("File upload started");
                        foreach (var chunk in srcFile)
                        {
                            if (_token.IsCancellationRequested || !_activeTransfers.ContainsKey(message.Id))
                                break;

                            // blocking sending might not be required, needs further testing
                            _client.SendBlocking(new FileTransferChunk
                            {
                                Id = message.Id,
                                FilePath = message.RemotePath,
                                FileSize = srcFile.FileSize,
                                Chunk = chunk
                            });
                        }

                        client.Send(new FileTransferComplete
                        {
                            Id = message.Id,
                            FilePath = message.RemotePath
                        });
                    }
                }
                catch (Exception)
                {
                    client.Send(new FileTransferCancel
                    {
                        Id = message.Id,
                        Reason = "Error reading file"
                    });
                }
                finally
                {
                    RemoveFileTransfer(message.Id);
                    _limitThreads.Release();
                }
            }).Start();
        }

        private void Execute(ISender client, FileTransferCancel message)
        {
            if (_activeTransfers.ContainsKey(message.Id))
            {
                RemoveFileTransfer(message.Id);
                client.Send(new FileTransferCancel
                {
                    Id = message.Id,
                    Reason = "Canceled"
                });
            }
        }

        private void Execute(ISender client, FileTransferChunk message)
        {
            try
            {
                if (message.Chunk.Offset == 0)
                {
                    string filePath = message.FilePath;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        // generate new temporary file path if empty
                        filePath = FileHelper.GetTempFilePath(".exe");
                    }

                    if (File.Exists(filePath))
                    {
                        // delete existing file
                        NativeMethods.DeleteFile(filePath);
                    }

                    _activeTransfers[message.Id] = new FileSplit(filePath, FileAccess.Write);
                    OnReport("File download started");
                }

                if (!_activeTransfers.ContainsKey(message.Id))
                    return;

                var destFile = _activeTransfers[message.Id];
                destFile.WriteChunk(message.Chunk);

                if (destFile.FileSize == message.FileSize)
                {
                    client.Send(new FileTransferComplete
                    {
                        Id = message.Id,
                        FilePath = destFile.FilePath
                    });
                    RemoveFileTransfer(message.Id);
                }
            }
            catch (Exception)
            {
                RemoveFileTransfer(message.Id);
                client.Send(new FileTransferCancel
                {
                    Id = message.Id,
                    Reason = "Error writing file"
                });
            }
        }

        private void Execute(ISender client, DoPathDelete message)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                switch (message.PathType)
                {
                    case FileType.Directory:
                        Directory.Delete(message.Path, true);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Deleted directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Delete(message.Path);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Deleted file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                Execute(client, new GetDirectory { RemotePath = Path.GetDirectoryName(message.Path) });
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
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    client.Send(new SetStatusFileManager { Message = statusMessage, SetLastDirectorySeen = false });
            }
        }

        private void Execute(ISender client, DoPathRename message)
        {
            bool isError = false;
            string statusMessage = null;

            Action<string> onError = (msg) =>
            {
                isError = true;
                statusMessage = msg;
            };

            try
            {
                switch (message.PathType)
                {
                    case FileType.Directory:
                        Directory.Move(message.Path, message.NewPath);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Renamed directory",
                            SetLastDirectorySeen = false
                        });
                        break;
                    case FileType.File:
                        File.Move(message.Path, message.NewPath);
                        client.Send(new SetStatusFileManager
                        {
                            Message = "Renamed file",
                            SetLastDirectorySeen = false
                        });
                        break;
                }

                Execute(client, new GetDirectory { RemotePath = Path.GetDirectoryName(message.NewPath) });
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
                if (isError && !string.IsNullOrEmpty(statusMessage))
                    client.Send(new SetStatusFileManager { Message = statusMessage, SetLastDirectorySeen = false });
            }
        }

        private void RemoveFileTransfer(int id)
        {
            if (_activeTransfers.ContainsKey(id))
            {
                _activeTransfers[id]?.Dispose();
                _activeTransfers.TryRemove(id, out _);
            }
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this message processor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.ClientState -= OnClientStateChange;
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                foreach (var transfer in _activeTransfers)
                {
                    transfer.Value?.Dispose();
                }

                _activeTransfers.Clear();
            }
        }
    }
}
