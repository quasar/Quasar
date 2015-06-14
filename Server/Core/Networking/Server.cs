using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using xServer.Core.Packets;

namespace xServer.Core.Networking
{
    public class Server
    {
        /// <summary>
        /// The amount of currently connected and authenticated clients.
        /// </summary>
        public int ConnectedAndAuthenticatedClients { get; set; }

        /// <summary>
        /// Occurs when the state of the server changes.
        /// </summary>
        public event ServerStateEventHandler ServerState;

        /// <summary>
        /// Represents a method that will handle a change in the server's state.
        /// </summary>
        /// <param name="s">The server which changed its state.</param>
        /// <param name="listening">The new listening state of the server.</param>
        public delegate void ServerStateEventHandler(Server s, bool listening);

        /// <summary>
        /// Fires an event that informs subscribers that the server has changed it's state.
        /// </summary>
        /// <param name="listening">The new listening state of the server.</param>
        private void OnServerState(bool listening)
        {
            if (ServerState != null)
            {
                ServerState(this, listening);
            }
        }

        /// <summary>
        /// Occurs when the state of a client changes.
        /// </summary>
        public event ClientStateEventHandler ClientState;

        /// <summary>
        /// Represents a method that will handle a change in a client's state.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client which changed its state.</param>
        /// <param name="connected">The new connection state of the client.</param>
        public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

        /// <summary>
        /// Fires an event that informs subscribers that a client has changed its state.
        /// </summary>
        /// <param name="c">The client which changed its state.</param>
        /// <param name="connected">The new connection state of the client.</param>
        private void OnClientState(Client c, bool connected)
        {
            if (ClientState != null)
            {
                ClientState(this, c, connected);
            }
        }

        /// <summary>
        /// Occurs when a packet is received by a client.
        /// </summary>
        public event ClientReadEventHandler ClientRead;

        /// <summary>
        /// Represents a method that will handle a packet received from a client.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client that has received the packet.</param>
        /// <param name="packet">The packet that received by the client.</param>
        public delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

        /// <summary>
        /// Fires an event that informs subscribers that a packet has been
        /// received from the client.
        /// </summary>
        /// <param name="c">The client that has received the packet.</param>
        /// <param name="packet">The packet that received by the client.</param>
        private void OnClientRead(Client c, IPacket packet)
        {
            if (ClientRead != null)
            {
                ClientRead(this, c, packet);
            }
        }

        /// <summary>
        /// Occurs when a packet is sent by a client.
        /// </summary>
        public event ClientWriteEventHandler ClientWrite;

        /// <summary>
        /// Represents the method that will handle the sent packet by a client.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client that has sent the packet.</param>
        /// <param name="packet">The packet that has been sent by the client.</param>
        /// <param name="length">The length of the packet.</param>
        /// <param name="rawData">The packet in raw bytes.</param>
        public delegate void ClientWriteEventHandler(Server s, Client c, IPacket packet, long length, byte[] rawData);

        /// <summary>
        /// Fires an event that informs subscribers that the client has sent a packet.
        /// </summary>
        /// <param name="c">The client that has sent the packet.</param>
        /// <param name="packet">The packet that has been sent by the client.</param>
        /// <param name="length">The length of the packet.</param>
        /// <param name="rawData">The packet in raw bytes.</param>
        private void OnClientWrite(Client c, IPacket packet, long length, byte[] rawData)
        {
            if (ClientWrite != null)
            {
                ClientWrite(this, c, packet, length, rawData);
            }
        }

        /// <summary>
        /// The port on which the server is listening.
        /// </summary>
        public ushort Port { get; private set; }

        /// <summary>
        /// The total amount of received bytes.
        /// </summary>
        public long BytesReceived { get; set; }

        /// <summary>
        /// The total amount of sent bytes.
        /// </summary>
        public long BytesSent { get; set; }

        /// <summary>
        /// The maximum size of one package in byte (it is also the buffer size for receiving data).
        /// </summary>
        public int MAX_PACKET_SIZE { get { return (1024 * 1024) * 1; } } // 1MB

        /// <summary>
        /// The keep-alive time in ms.
        /// </summary>
        public uint KEEP_ALIVE_TIME { get { return 25000; } } // 25s

        /// <summary>
        /// The keep-alive interval in ms.
        /// </summary>
        public uint KEEP_ALIVE_INTERVAL { get { return 25000; } } // 25s

        /// <summary>
        /// The header size in byte.
        /// </summary>
        public int HEADER_SIZE { get { return 4; } } // 4B

        /// <summary>
        /// Gets or sets if the server is currently processing data that should prevent disconnection. 
        /// </summary>
        public bool Processing { get; private set; }

        /// <summary>
        /// The buffer manager to handle the receive buffers for the clients.
        /// </summary>
        public static PooledBufferManager BufferManager { get; private set; }

        /// <summary>
        /// The listening state of the server. True if listening, else False.
        /// </summary>
        public bool Listening { get; private set; }

        /// <summary>
        /// Gets the clients currently connected to the server, or an empty array of
        /// clients if the server is currently not listening.
        /// </summary>
        public Client[] Clients
        {
            get
            {
                lock (_clientsLock)
                {
                    return Listening ? _clients.ToArray() : new Client[0];
                }
            }
        }

        /// <summary>
        /// A collection containing all clients that have ever connected to the server.
        /// </summary>
        public Dictionary<string, DateTime> AllTimeConnectedClients { get; set; }

        /// <summary>
        /// Handle of the Server Socket.
        /// </summary>
        private Socket _handle;

        /// <summary>
        /// The event to accept new connections asynchronously.
        /// </summary>
        private SocketAsyncEventArgs _item;

        /// <summary>
        /// List of the clients connected to the server.
        /// </summary>
        private List<Client> _clients;

        /// <summary>
        /// Lock object for the list of clients.
        /// </summary>
        private readonly object _clientsLock = new object();

        /// <summary>
        /// List of all supported Packet Types by the server.
        /// </summary>
        private List<Type> PacketTypes { get; set; }

        /// <summary>
        /// Constructor of the server, initializes variables.
        /// </summary>
        public Server()
        {
            PacketTypes = new List<Type>();
            AllTimeConnectedClients = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Begins listening for clients.
        /// </summary>
        /// <param name="port">Port to listen for clients on.</param>
        public void Listen(ushort port)
        {
            this.Port = port;
            try
            {
                if (!Listening)
                {
                    lock (_clientsLock)
                    {
                        _clients = new List<Client>();
                    }

                    _item = new SocketAsyncEventArgs();
                    _item.Completed += AcceptClient;

                    if (_handle != null)
                    {
                        try
                        {
                            _handle.Close();
                        }
                        catch
                        {
                        }
                    }
                    
                    if (BufferManager == null)
                        BufferManager = new PooledBufferManager(MAX_PACKET_SIZE, 1) {ClearOnReturn = true};

                    _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _handle.Bind(new IPEndPoint(IPAddress.Any, port));
                    _handle.Listen(1000);

                    Processing = false;

                    Listening = true;
                    OnServerState(true);

                    if (!_handle.AcceptAsync(_item))
                        AcceptClient(null, _item);
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds a Type to the serializer so a message can be properly serialized.
        /// </summary>
        /// <param name="parent">The parent type.</param>
        /// <param name="type">Type to be added.</param>
        public void AddTypeToSerializer(Type parent, Type type)
        {
            if (type == null || parent == null)
                throw new ArgumentNullException();

            PacketTypes.Add(type);
        }

        /// <summary>
        /// Adds Types to the serializer.
        /// </summary>
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="types">Types to add.</param>
        public void AddTypesToSerializer(Type parent, params Type[] types)
        {
            foreach (Type type in types)
                AddTypeToSerializer(parent, type);
        }

        /// <summary>
        /// Processes an incoming client; adding the client to the list of clients,
        /// hooking up the client's events, and finally accepts the client.
        /// </summary>
        /// <param name="s">Unused, use null.</param>
        /// <param name="e">Asynchronously Socket Event</param>
        private void AcceptClient(object s, SocketAsyncEventArgs e)
        {
            try
            {
                do
                {
                    switch (e.SocketError)
                    {
                        case SocketError.Success:
                            if (BufferManager.BuffersAvailable == 0)
                                BufferManager.IncreaseBufferCount(1);

                            Client client = new Client(this, e.AcceptSocket, PacketTypes.ToArray());

                            lock (_clientsLock)
                            {
                                _clients.Add(client);
                                client.ClientState += OnClientState;
                                client.ClientRead += OnClientRead;
                                client.ClientWrite += OnClientWrite;

                                OnClientState(client, true);
                            }
                            break;
                        case SocketError.ConnectionReset:
                            break;
                        default:
                            throw new Exception("SocketError");
                    }

                    e.AcceptSocket = null; // enable reuse
                } while (!_handle.AcceptAsync(e));
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Removes a disconnected client from the list of clients.
        /// </summary>
        /// <param name="client">The client to remove.</param>
        public void RemoveClient(Client client)
        {
            lock (_clientsLock)
            {
                int index = -1;
                for (int i = 0; i < _clients.Count; i++)
                    if (_clients[i].Equals(client))
                    {
                        index = i;
                        break;
                    }

                if (index < 0)
                    return;

                try
                {
                    _clients[index].Disconnect();
                    _clients.RemoveAt(index);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Disconnect the server from all of the clients and discontinue
        /// listening (placing the server in an "off" state).
        /// </summary>
        public void Disconnect()
        {
            if (Processing)
                return;

            Processing = true;

            if (_handle != null)
            {
                _handle.Close();
            }

            lock (_clientsLock)
            {
                while (_clients.Count != 0)
                {
                    _clients[0].Disconnect();
                    try
                    {
                        _clients.RemoveAt(0);
                    }
                    catch
                    {
                    }
                }
            }

            Listening = false;
            OnServerState(false);
        }
    }
}