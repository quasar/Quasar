using Quasar.Common.Enums;
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
using xServer.Core.Utilities;
using xServer.Enums;
using xServer.Models;

namespace xServer.Core.Commands
{
    public class FileManagerHandler : MessageProcessorBase<string>
    {
        public delegate void DrivesChangedEventHandler(object sender, Drive[] drives);
        public delegate void DirectoryChangedEventHandler(object sender, string remotePath, FileSystemEntry[] items);
        public delegate void FileTransferUpdatedEventHandler(object sender, FileTransfer transfer);

        public event DrivesChangedEventHandler DrivesChanged;
        public event DirectoryChangedEventHandler DirectoryChanged;
        public event FileTransferUpdatedEventHandler FileTransferUpdated;

        private void OnDrivesChanged(Drive[] drives)
        {
            SynchronizationContext.Post(d =>
            {
                var handler = DrivesChanged;
                handler?.Invoke(this, (Drive[])d);
            }, drives);
        }

        private void OnDirectoryChanged(string remotePath, FileSystemEntry[] items)
        {
            SynchronizationContext.Post(i =>
            {
                var handler = DirectoryChanged;
                handler?.Invoke(this, remotePath, (FileSystemEntry[])i);
            }, items);
        }

        private void OnFileTransferUpdated(FileTransfer transfer)
        {
            SynchronizationContext.Post(t =>
            {
                var handler = FileTransferUpdated;
                handler?.Invoke(this, (FileTransfer)t);
            }, transfer);
        }

        private readonly List<FileTransfer> _activeTransfers;

        /// <summary>
        /// Used in lock statements to synchronize access between UI thread and thread pool.
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// The client which is associated with this file manager handler.
        /// </summary>
        private readonly Client _client;

        private readonly Semaphore _limitThreads = new Semaphore(2, 2); // maximum simultaneous file uploads

        private readonly string _baseDownloadPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManagerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public FileManagerHandler(Client client) : base(true)
        {
            _client = client;
            _activeTransfers = new List<FileTransfer>();
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

        public void BeginDownloadFile(string remotePath)
        {
            int id = GetUniqueFileTransferId();
            _client.Send(new DoDownloadFile {RemotePath = remotePath, Id = id});
        }

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

                _activeTransfers.Add(transfer);

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

                    if (_activeTransfers.Count(f => f.Id == transfer.Id) == 0)
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

        public void CancelFileTransfer(int transferId)
        {
            _client.Send(new DoDownloadFileCancel {Id = transferId});
            _activeTransfers.RemoveAll(s => s.Id == transferId);
        }

        public void RenameFile(string remotePath, string newPath, FileType type)
        {
            _client.Send(new DoPathRename
            {
                Path = remotePath,
                NewPath = newPath,
                PathType = type
            });
        }

        public void DeleteFile(string remotePath, FileType type)
        {
            _client.Send(new DoPathDelete {Path = remotePath, PathType = type});
        }

        public void StartProcess(string remotePath)
        {
            _client.Send(new DoProcessStart {ApplicationName = remotePath});
        }

        public void AddToStartup(StartupItem item)
        {
            _client.Send(new DoStartupItemAdd {StartupItem = item});
        }

        public void RequestDirectoryContents(string remotePath)
        {
            _client.Send(new GetDirectory {RemotePath = remotePath});
        }

        public void RequestDrives()
        {
            _client.Send(new GetDrives());
        }

        private void Execute(ISender client, DoDownloadFileResponse message)
        {
            FileTransfer transfer = _activeTransfers.FirstOrDefault(t => t.Id == message.Id);

            if (transfer == null)
            {
                if (message.CurrentBlock != 0)
                {
                    // TODO: disconnect client
                }

                // don't escape from download directory
                if (FileHelper.CheckPathForIllegalChars(message.Filename))
                {
                    // disconnect malicious client
                    // TODO: client.Disconnect();
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

                _activeTransfers.Add(transfer);
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

        private int GetUniqueFileTransferId()
        {
            int id;
            do
            {
                id = FileHelper.GetNewTransferId();
                // generate new Id until we have a unique one
            } while (_activeTransfers.Any(f => f.Id == id));

            return id;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var transfer in _activeTransfers)
                {
                    _client.Send(new DoDownloadFileCancel { Id = transfer.Id });
                }
                _activeTransfers.Clear();
            }
        }
    }
}
