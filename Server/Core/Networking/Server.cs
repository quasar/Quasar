using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Networking.Utilities;
using xServer.Core.Packets;

namespace xServer.Core.Networking
{
    public class Server
    {
        /// <summary>
        /// Occurs when the state of the server changes.
        /// </summary>
        public event ServerStateEventHandler ServerState;

        /// <summary>
        /// Represents a method that will handle a change in the server's state.
        /// </summary>
        /// <param name="s">The server which changed its state.</param>
        /// <param name="listening">The new listening state of the server.</param>
        /// <param name="port">The port the server is listening on, if listening is True.</param>
        public delegate void ServerStateEventHandler(Server s, bool listening, ushort port);

        /// <summary>
        /// Fires an event that informs subscribers that the server has changed it's state.
        /// </summary>
        /// <param name="listening">The new listening state of the server.</param>
        private void OnServerState(bool listening)
        {
            if (Listening == listening) return;

            Listening = listening;

            if (ServerState != null)
            {
                ServerState(this, listening, Port);
            }
        }

        /// <summary>
        /// Occurs when the state of a client changes.
        /// </summary>
        protected event ClientStateEventHandler ClientState;

        /// <summary>
        /// Represents a method that will handle a change in a client's state.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client which changed its state.</param>
        /// <param name="connected">The new connection state of the client.</param>
        protected delegate void ClientStateEventHandler(Server s, Client c, bool connected);

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
        protected event ClientReadEventHandler ClientRead;

        /// <summary>
        /// Represents a method that will handle a packet received from a client.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client that has received the packet.</param>
        /// <param name="packet">The packet that received by the client.</param>
        protected delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

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
        protected event ClientWriteEventHandler ClientWrite;

        /// <summary>
        /// Represents the method that will handle the sent packet by a client.
        /// </summary>
        /// <param name="s">The server, the client is connected to.</param>
        /// <param name="c">The client that has sent the packet.</param>
        /// <param name="packet">The packet that has been sent by the client.</param>
        /// <param name="length">The length of the packet.</param>
        /// <param name="rawData">The packet in raw bytes.</param>
        protected delegate void ClientWriteEventHandler(Server s, Client c, IPacket packet, long length, byte[] rawData);

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
        /// The buffer size for receiving data in bytes.
        /// </summary>
        public int BUFFER_SIZE { get { return 1024 * 16; } } // 16KB

        /// <summary>
        /// The keep-alive time in ms.
        /// </summary>
        public uint KEEP_ALIVE_TIME { get { return 25000; } } // 25s

        /// <summary>
        /// The keep-alive interval in ms.
        /// </summary>
        public uint KEEP_ALIVE_INTERVAL { get { return 25000; } } // 25s

        /// <summary>
        /// The header size in bytes.
        /// </summary>
        public int HEADER_SIZE { get { return 4; } } // 4B

        /// <summary>
        /// The maximum size of a packet in bytes.
        /// </summary>
        public int MAX_PACKET_SIZE { get { return (1024 * 1024) * 5; } } // 5MB

        /// <summary>
        /// The buffer manager to handle the receive buffers for the clients.
        /// </summary>
        public PooledBufferManager BufferManager { get; private set; }

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
        /// Determines if the server is currently processing Disconnect method. 
        /// </summary>
        private bool _processing;

        /// <summary>
        /// Constructor of the server, initializes variables.
        /// </summary>
        public Server()
        {
            PacketTypes = new List<Type>();
        }

        /// <summary>
        /// Begins listening for clients.
        /// </summary>
        /// <param name="port">Port to listen for clients on.</param>
        public void Listen(ushort port)
        {
            if (PacketTypes.Count == 0) throw new Exception("No packet types added");

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
                        _handle.Close();
                    }

                    if (BufferManager == null)
                        BufferManager = new PooledBufferManager(BUFFER_SIZE, 1) { ClearOnReturn = true };

                    _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _handle.Bind(new IPEndPoint(IPAddress.Any, port));
                    _handle.Listen(1000);

                    _processing = false;

                    OnServerState(true);

                    if (!_handle.AcceptAsync(_item))
                        AcceptClient(null, _item);
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    MessageBox.Show("The port is already in use.", "Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        string.Format(
                            "An unexpected socket error occurred: {0}\n\nError Code: {1}\n\nPlease report this as fast as possible here:\n{2}/issues",
                            ex.Message, ex.ErrorCode, Settings.RepositoryURL), "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Disconnect();
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds Types to the serializer.
        /// </summary>
        /// <param name="types">Types to add.</param>
        public void AddTypesToSerializer(Type[] types)
        {
            PacketTypes.AddRange(types.Where(t => t != null));
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
                            client.ClientState += OnClientState;
                            client.ClientRead += OnClientRead;
                            client.ClientWrite += OnClientWrite;
                            OnClientState(client, true);
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
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds a connected client to the list of clients.
        /// </summary>
        /// <param name="client">The client to add.</param>
        public void AddClient(Client client)
        {
            lock (_clientsLock)
            {
                _clients.Add(client);
            }
        }

        /// <summary>
        /// Removes a disconnected client from the list of clients.
        /// </summary>
        /// <param name="client">The client to remove.</param>
        public void RemoveClient(Client client)
        {
            if (_processing) return;

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
            if (_processing) return;
            _processing = true;

            if (_handle != null)
            {
                _handle.Close();
                _handle = null;
            }

            lock (_clientsLock)
            {
                while (_clients.Count != 0)
                {
                    try
                    {
                        _clients[0].Disconnect();
                        _clients.RemoveAt(0);
                    }
                    catch
                    {
                    }
                }
            }

            _processing = false;
            OnServerState(false);
        }
    }
}