using System;
using System.Linq;
using xServer.Core.Commands;
using xServer.Core.NetSerializer;
using xServer.Core.Packets;

namespace xServer.Core.Networking
{
    public class QuasarServer : Server
    {
        /// <summary>
        /// Gets the clients currently connected and authenticated to the server.
        /// </summary>
        public Client[] ConnectedClients
        {
            get { return Clients.Where(c => c != null && c.Authenticated).ToArray(); }
        }

        /// <summary>
        /// Occurs when a client connected.
        /// </summary>
        public event ClientConnectedEventHandler ClientConnected;

        /// <summary>
        /// Represents the method that will handle the connected client.
        /// </summary>
        /// <param name="client">The connected client.</param>
        public delegate void ClientConnectedEventHandler(Client client);

        /// <summary>
        /// Fires an event that informs subscribers that the client is connected.
        /// </summary>
        /// <param name="client">The connected client.</param>
        private void OnClientConnected(Client client)
        {
            if (ProcessingDisconnect || !Listening) return;
            var handler = ClientConnected;
            if (handler != null)
            {
                handler(client);
            }
        }

        /// <summary>
        /// Occurs when a client disconnected.
        /// </summary>
        public event ClientDisconnectedEventHandler ClientDisconnected;

        /// <summary>
        /// Represents the method that will handle the disconnected client.
        /// </summary>
        /// <param name="client">The disconnected client.</param>
        public delegate void ClientDisconnectedEventHandler(Client client);

        /// <summary>
        /// Fires an event that informs subscribers that the client is disconnected.
        /// </summary>
        /// <param name="client">The disconnected client.</param>
        private void OnClientDisconnected(Client client)
        {
            if (ProcessingDisconnect || !Listening) return;
            var handler = ClientDisconnected;
            if (handler != null)
            {
                handler(client);
            }
        }

        /// <summary>
        /// Constructor, initializes required objects and subscribes to events of the server.
        /// </summary>
        public QuasarServer() : base()
        {
            base.Serializer = new Serializer(PacketRegistery.GetPacketTypes());

            base.ClientState += OnClientState;
            base.ClientRead += OnClientRead;
        }

        /// <summary>
        /// Decides if the client connected or disconnected.
        /// </summary>
        /// <param name="server">The server the client is connected to.</param>
        /// <param name="client">The client which changed its state.</param>
        /// <param name="connected">True if the client connected, false if disconnected.</param>
        private void OnClientState(Server server, Client client, bool connected)
        {
            switch (connected)
            {
                case true:
                    new Packets.ServerPackets.GetAuthentication().Execute(client); // begin handshake
                    break;
                case false:
                    if (client.Authenticated)
                    {
                        OnClientDisconnected(client);
                    }
                    break;
            }
        }

        /// <summary>
        /// Forwards received packets from the client to the PacketHandler.
        /// </summary>
        /// <param name="server">The server the client is connected to.</param>
        /// <param name="client">The client which has received the packet.</param>
        /// <param name="packet">The received packet.</param>
        private void OnClientRead(Server server, Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (!client.Authenticated)
            {
                if (type == typeof (Packets.ClientPackets.GetAuthenticationResponse))
                {
                    client.Authenticated = true;
                    new Packets.ServerPackets.SetAuthenticationSuccess().Execute(client); // finish handshake
                    CommandHandler.HandleGetAuthenticationResponse(client,
                        (Packets.ClientPackets.GetAuthenticationResponse) packet);
                    OnClientConnected(client);
                }
                else
                {
                    client.Disconnect();
                }
                return;
            }

            PacketHandler.HandlePacket(client, packet);
        }
    }
}
