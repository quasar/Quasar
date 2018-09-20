using Quasar.Common.Enums;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Enums;
using xServer.Models;

namespace xServer.Core.Commands
{
    public class FileManagerHandler : MessageProcessorBase<string>
    {
        /// <summary>
        /// Represents the method that will handle drive changes.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="drives">All currently available drives.</param>
        public delegate void DrivesChangedEventHandler(object sender, Drive[] drives);

        /// <summary>
        /// Represents the method that will handle directory changes.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="remotePath">The remote path of the directory.</param>
        /// <param name="items">The directory content.</param>
        public delegate void DirectoryChangedEventHandler(object sender, string remotePath, FileSystemEntry[] items);

        /// <summary>
        /// Represents the method that will handle file transfer updates.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="transfer">The updated file transfer.</param>
        public delegate void FileTransferUpdatedEventHandler(object sender, FileTransfer transfer);

        /// <summary>
        /// Raised when drives changed.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event DrivesChangedEventHandler DrivesChanged;

        /// <summary>
        /// Raised when a directory changed.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event DirectoryChangedEventHandler DirectoryChanged;

        /// <summary>
        /// Raised when a file transfer updated.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event FileTransferUpdatedEventHandler FileTransferUpdated;

        /// <summary>
        /// Reports changed remote drives.
        /// </summary>
        /// <param name="drives">The current remote drives.</param>
        private void OnDrivesChanged(Drive[] drives)
        {
            SynchronizationContext.Post(d =>
            {
                var handler = DrivesChanged;
                handler?.Invoke(this, (Drive[])d);
            }, drives);
        }

        /// <summary>
        /// Reports a directory change.
        /// </summary>
        /// <param name="remotePath">The remote path of the directory.</param>
        /// <param name="items">The directory content.</param>
        private void OnDirectoryChanged(string remotePath, FileSystemEntry[] items)
        {
            SynchronizationContext.Post(i =>
            {
                var handler = DirectoryChanged;
                handler?.Invoke(this, remotePath, (FileSystemEntry[])i);
            }, items);
        }

        /// <summary>
        /// Reports updated file transfers.
        /// </summary>
        /// <param name="transfer">The updated file transfer.</param>
        private void OnFileTransferUpdated(FileTransfer transfer)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = FileTransferUpdated;
                handler?.Invoke(this, (FileTransfer)t);
            }, transfer);
        }

        /// <summary>
        /// Keeps track of all active file transfers. Finished or canceled transfers get removed.
        /// </summary>
        private readonly List<FileTransfer> _activeFileTransfers = new List<FileTransfer>();

        /// <summary>
        /// Used in lock statements to synchronize access between UI thread and thread pool.
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// The client which is associated with this file manager handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Used to only allow two simultaneous file uploads.
        /// </summary>
        private readonly Semaphore _limitThreads = new Semaphore(2, 2);

        /// <summary>
        /// Path to the base download directory of the client.
        /// </summary>
        private readonly string _baseDownloadPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManagerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public FileManagerHandler(Client client) : base(true)
        {
            _client = client;
            _baseDownloadPath = client.Value.DownloadDirectory;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is DoDownloadFileResponse ||
                                                             message is GetDrivesResponse ||
                                                             message is GetDirectoryResponse ||
                                                             message is SetStatusFileManager;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoDownloadFileResponse file:
                    Execute(sender, file);
                    break;
                case GetDrivesResponse drive:
                    Execute(sender, drive);
                    break;
                case GetDirectoryResponse directory:
                    Execute(sender, directory);
                    break;
                case SetStatusFileManager status:
                    Execute(sender, status);
                    break;
            }
        }

        /// <summary>
        /// Begins downloading a file from the client.
        /// </summary>
        /// <param name="remotePath">The remote path of the file to download.</param>
        public void BeginDownloadFile(string remotePath)
        {
            int id = GetUniqueFileTransferId();
            _client.Send(new DoDownloadFile {RemotePath = remotePath, Id = id});
        }

        /// <summary>
        /// Begins uploading a file to the client.
        /// </summary>
        /// <param name="localPath">The local path of the file to upload.</param>
        /// <param name="remotePath">Save the uploaded file to this remote path.</param>
        public void BeginUploadFile(string localPath, string remotePath)
        {
            new Thread(() =>
            {
                int id = GetUniqueFileTransferId();

                FileTransfer transfer = new FileTransfer
                {
                    Id = id,
                    Type = TransferType.Upload,
                    LocalPath = localPath,
                    RemotePath = remotePath,
                    Status = "Pending..."
                };

                lock (_syncLock)
                {
                    _activeFileTransfers.Add(transfer);
                }

                FileSplit srcFile = new FileSplit(localPath);
                if (srcFile.MaxBlocks < 0)
                {
                    transfer.Status = "Error reading file";
                    OnFileTransferUpdated(transfer);
                    return;
                }

                // TODO: change to real size
                transfer.Size = srcFile.MaxBlocks;
                OnFileTransferUpdated(transfer);

                _limitThreads.WaitOne();
                for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                {
                    decimal progress =
                        Math.Round((decimal)((double)(currentBlock + 1) / (double)srcFile.MaxBlocks * 100.0), 2);

                    transfer.TransferredSize = currentBlock + 1;
                    transfer.Status = $"Uploading...({progress}%)";
                    OnFileTransferUpdated(transfer);

                    bool transferCanceled;
                    lock (_syncLock)
                    {
                        transferCanceled = _activeFileTransfers.Count(f => f.Id == transfer.Id) == 0;
                    }

                    if (transferCanceled)
                    {
                        transfer.Status = "Canceled";
                        OnFileTransferUpdated(transfer);
                        _limitThreads.Release();
                        return;
                    }

                    if (srcFile.ReadBlock(currentBlock, out var block))
                    {
                        // blocking sending might not be required, needs further testing
                        _client.SendBlocking(new DoUploadFile
                        {
                            Id = id,
                            RemotePath = remotePath,
                            Block = block,
                            MaxBlocks = srcFile.MaxBlocks,
                            CurrentBlock = currentBlock
                        });
                    }
                    else
                    {
                        transfer.Status = "Error reading file";
                        OnFileTransferUpdated(transfer);
                        _limitThreads.Release();
                        return;
                    }
                }
                _limitThreads.Release();

                transfer.Status = "Completed";
                OnFileTransferUpdated(transfer);
            }).Start();
        }

        /// <summary>
        /// Cancels a file transfer.
        /// </summary>
        /// <param name="transferId">The Id of the file transfer to cancel.</param>
        public void CancelFileTransfer(int transferId)
        {
            _client.Send(new DoDownloadFileCancel {Id = transferId});
            lock (_syncLock)
            {
                _activeFileTransfers.RemoveAll(s => s.Id == transferId);
            }
        }

        /// <summary>
        /// Renames a remote file or directory.
        /// </summary>
        /// <param name="remotePath">The remote file or directory path to rename.</param>
        /// <param name="newPath">The new name of the remote file or directory path.</param>
        /// <param name="type">The type of the file (file or directory).</param>
        public void RenameFile(string remotePath, string newPath, FileType type)
        {
            _client.Send(new DoPathRename
            {
                Path = remotePath,
                NewPath = newPath,
                PathType = type
            });
        }

        /// <summary>
        /// Deletes a remote file or directory.
        /// </summary>
        /// <param name="remotePath">The remote file or directory path.</param>
        /// <param name="type">The type of the file (file or directory).</param>
        public void DeleteFile(string remotePath, FileType type)
        {
            _client.Send(new DoPathDelete {Path = remotePath, PathType = type});
        }

        /// <summary>
        /// Starts a new process remotely.
        /// </summary>
        /// <param name="remotePath">The remote path used for starting the new process.</param>
        public void StartProcess(string remotePath)
        {
            _client.Send(new DoProcessStart {ApplicationName = remotePath});
        }

        /// <summary>
        /// Adds an item to the startup of the client.
        /// </summary>
        /// <param name="item">The startup item to add.</param>
        public void AddToStartup(StartupItem item)
        {
            _client.Send(new DoStartupItemAdd {StartupItem = item});
        }

        /// <summary>
        /// Gets the directory contents for the remote path.
        /// </summary>
        /// <param name="remotePath">The remote path of the directory.</param>
        public void GetDirectoryContents(string remotePath)
        {
            _client.Send(new GetDirectory {RemotePath = remotePath});
        }

        /// <summary>
        /// Refreshes the remote drives.
        /// </summary>
        public void RefreshDrives()
        {
            _client.Send(new GetDrives());
        }

        private void Execute(ISender client, DoDownloadFileResponse message)
        {
            FileTransfer transfer;
            lock (_syncLock)
            {
                transfer = _activeFileTransfers.FirstOrDefault(t => t.Id == message.Id);
            }

            if (transfer == null)
            {
                // don't escape from download directory
                if (FileHelper.CheckPathForIllegalChars(message.Filename))
                {
                    // disconnect malicious client
                    client.Disconnect();
                    return;
                }

                if (!Directory.Exists(_baseDownloadPath))
                    Directory.CreateDirectory(_baseDownloadPath);

                string downloadPath = Path.Combine(_baseDownloadPath, message.Filename);

                int i = 1;
                while (File.Exists(downloadPath))
                {
                    // rename file if it exists already
                    var newFileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(downloadPath), i, Path.GetExtension(downloadPath));
                    downloadPath = Path.Combine(_baseDownloadPath, newFileName);
                    i++;
                }

                transfer = new FileTransfer
                {
                    Id = message.Id,
                    Type = TransferType.Download,
                    LocalPath = downloadPath,
                    RemotePath = message.Filename, // TODO: Change to absolute path
                    Size = message.MaxBlocks, // TODO: Change to real size
                    TransferredSize = 0
                };

                lock (_syncLock)
                {
                    _activeFileTransfers.Add(transfer);
                }
            }

            // TODO: change to += message.Block.Length
            transfer.TransferredSize = message.CurrentBlock + 1;

            if (!string.IsNullOrEmpty(message.CustomMessage))
            {
                // client-side error
                transfer.Status = message.CustomMessage;
                OnFileTransferUpdated(transfer);
                return;
            }

            FileSplit destFile = new FileSplit(transfer.LocalPath);

            if (!destFile.AppendBlock(message.Block, message.CurrentBlock))
            {
                // server-side error
                transfer.Status = destFile.LastError;
                OnFileTransferUpdated(transfer);
                return;
            }

            if (message.CurrentBlock + 1 == message.MaxBlocks)
            {
                transfer.Status = "Completed";
            }
            else
            {
                decimal progress =
                    Math.Round((decimal) ((double) (message.CurrentBlock + 1) / (double) message.MaxBlocks * 100.0), 2);

                transfer.Status = $"Downloading...({progress}%)";
            }

            OnFileTransferUpdated(transfer);
        }

        private void Execute(ISender client, GetDrivesResponse message)
        {
            if (message.Drives?.Length == 0)
                return;

            OnDrivesChanged(message.Drives);
        }
        
        private void Execute(ISender client, GetDirectoryResponse message)
        {
            OnDirectoryChanged(message.RemotePath, message.Items);
        }

        private void Execute(ISender client, SetStatusFileManager message)
        {
            OnReport(message.Message);
        }

        /// <summary>
        /// Generates a unique file transfer id.
        /// </summary>
        /// <returns>A unique file transfer id.</returns>
        private int GetUniqueFileTransferId()
        {
            int id;
            lock (_syncLock)
            {
                do
                {
                    id = FileHelper.GetNewTransferId();
                    // generate new Id until we have a unique one
                } while (_activeFileTransfers.Any(f => f.Id == id));
            }

            return id;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncLock)
                {
                    foreach (var transfer in _activeFileTransfers)
                    {
                        _client.Send(new DoDownloadFileCancel { Id = transfer.Id });
                    }

                    _activeFileTransfers.Clear();
                }
            }
        }
    }
}
