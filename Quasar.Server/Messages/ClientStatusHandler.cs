using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class ClientStatusHandler : MessageProcessorBase<object>
    {
        /// <summary>
        /// Represents the method that will handle status updates.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="client">The client which updated the status.</param>
        /// <param name="statusMessage">The new status.</param>
        public delegate void StatusUpdatedEventHandler(object sender, Client client, string statusMessage);

        /// <summary>
        /// Represents the method that will handle user status updates.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="client">The client which updated the user status.</param>
        /// <param name="userStatusMessage">The new user status.</param>
        public delegate void UserStatusUpdatedEventHandler(object sender, Client client, UserStatus userStatusMessage);

        /// <summary>
        /// Raised when a client updated its status.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event StatusUpdatedEventHandler StatusUpdated;

        /// <summary>
        /// Raised when a client updated its user status.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event UserStatusUpdatedEventHandler UserStatusUpdated;

        /// <summary>
        /// Reports an updated status.
        /// </summary>
        /// <param name="client">The client which updated the status.</param>
        /// <param name="statusMessage">The new status.</param>
        private void OnStatusUpdated(Client client, string statusMessage)
        {
            SynchronizationContext.Post(c =>
            {
                var handler = StatusUpdated;
                handler?.Invoke(this, (Client) c, statusMessage);
            }, client);
        }

        /// <summary>
        /// Reports an updated user status.
        /// </summary>
        /// <param name="client">The client which updated the user status.</param>
        /// <param name="userStatusMessage">The new user status.</param>
        private void OnUserStatusUpdated(Client client, UserStatus userStatusMessage)
        {
            SynchronizationContext.Post(c =>
            {
                var handler = UserStatusUpdated;
                handler?.Invoke(this, (Client) c, userStatusMessage);
            }, client);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStatusHandler"/> class.
        /// </summary>
        public ClientStatusHandler() : base(true)
        {
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is SetStatus || message is SetUserStatus;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => true;

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case SetStatus status:
                    Execute((Client) sender, status);
                    break;
                case SetUserStatus userStatus:
                    Execute((Client) sender, userStatus);
                    break;
            }
        }

        private void Execute(Client client, SetStatus message)
        {
            OnStatusUpdated(client, message.Message);
        }

        private void Execute(Client client, SetUserStatus message)
        {
            OnUserStatusUpdated(client, message.Message);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
