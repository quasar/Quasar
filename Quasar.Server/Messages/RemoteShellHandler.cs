using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class RemoteShellHandler : MessageProcessorBase<string>
    {
        /// <summary>
        /// The client which is associated with this remote shell handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Represents the method that will command errors.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="errorMessage">The error message.</param>
        public delegate void CommandErrorEventHandler(object sender, string errorMessage);

        /// <summary>
        /// Raised when a command writes to stderr.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event CommandErrorEventHandler CommandError;

        /// <summary>
        /// Reports a command error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void OnCommandError(string errorMessage)
        {
            SynchronizationContext.Post(val =>
            {
                var handler = CommandError;
                handler?.Invoke(this, (string)val);
            }, errorMessage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteShellHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public RemoteShellHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is DoShellExecuteResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoShellExecuteResponse resp:
                    Execute(sender, resp);
                    break;
            }
        }

        /// <summary>
        /// Sends a command to execute in the remote shell of the client.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void SendCommand(string command)
        {
            _client.Send(new DoShellExecute {Command = command});
        }

        private void Execute(ISender client, DoShellExecuteResponse message)
        {
            if (message.IsError)
                OnCommandError(message.Output);
            else
                OnReport(message.Output);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client.Connected)
                {
                    SendCommand("exit");
                }
            }
        }
    }
}
