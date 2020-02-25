using System.Collections.Generic;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class StartupManagerHandler : MessageProcessorBase<List<StartupItem>>
    {
        /// <summary>
        /// The client which is associated with this startup manager handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupManagerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public StartupManagerHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetStartupItemsResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetStartupItemsResponse items:
                    Execute(sender, items);
                    break;
            }
        }

        /// <summary>
        /// Refreshes the current startup items.
        /// </summary>
        public void RefreshStartupItems()
        {
            _client.Send(new GetStartupItems());
        }

        /// <summary>
        /// Removes an item from startup.
        /// </summary>
        /// <param name="item">Startup item to remove.</param>
        public void RemoveStartupItem(StartupItem item)
        {
            _client.Send(new DoStartupItemRemove {StartupItem = item});
        }

        /// <summary>
        /// Adds an item to startup.
        /// </summary>
        /// <param name="item">Startup item to add.</param>
        public void AddStartupItem(StartupItem item)
        {
            _client.Send(new DoStartupItemAdd {StartupItem = item});
        }

        private void Execute(ISender client, GetStartupItemsResponse message)
        {
            OnReport(message.StartupItems);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
