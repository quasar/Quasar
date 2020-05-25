using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class RemoteExecutionHandler : MessageProcessorBase<string>
    {
        /// <summary>
        /// The client which is associated with this remote execution handler.
        /// </summary>
        private readonly Client _client;

        public RemoteExecutionHandler(Client client) : base(true)
        {
            _client = client;
        }

        public override bool CanExecute(IMessage message) => message is DoRemoteExecutionResponse;

        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoRemoteExecutionResponse execResp:
                    Execute(sender, execResp);
                    break;
            }
        }

        /// <summary>
        /// Starts a new process remotely.
        /// </summary>
        /// <param name="remotePath">The remote path used for starting the new process.</param>
        /// <param name="isUpdate">Decides whether the process is a client update.</param>
        public void StartProcess(string remotePath, bool isUpdate = false)
        {
            _client.Send(new DoRemoteExecution { FilePath = remotePath, IsUpdate = isUpdate });
        }

        /// <summary>
        /// Downloads a file from the web and executes it remotely.
        /// </summary>
        /// <param name="url">The URL to download and execute.</param>
        /// <param name="isUpdate">Decides whether the file is a client update.</param>
        public void StartProcessFromWeb(string url, bool isUpdate = false)
        {
            _client.Send(new DoRemoteExecution { DownloadUrl = url, IsUpdate = isUpdate});
        }

        private void Execute(ISender client, DoRemoteExecutionResponse message)
        {
            OnReport(message.Success ? "Process started successfully" : "Process failed to start");
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
