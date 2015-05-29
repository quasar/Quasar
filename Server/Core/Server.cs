using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using xServer.Core.Packets;

namespace xServer.Core
{
    public class Server
    {
        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }

        /// <summary>
        /// Occurs when the state of the server changes.
        /// </summary>
        public event ServerStateEventHandler ServerState;

        /// <summary>
        /// Represents a method that will handle a change in the server's state.
        /// </summary>
        /// <param name="s">The server to update the state of.</param>
        /// <param name="listening">True if the server is listening; False if the server
        /// is not listening.</param>
        public delegate void ServerStateEventHandler(Server s, bool listening);

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
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <param name="connected"></param>
        public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

        /// <summary>
        /// Fires an event that informs subscribers that the a packet has been
        /// received from the client.
        /// </summary>
        /// <param name="packet"></param>
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
        /// <param name="s">The destination server of the packet; also where the client specified
        /// should reside</param>
        /// <param name="c">The client that has sent the packet.</param>
        /// <param name="packet">The packet that was sent to the server.</param>
        public delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

        /// <summary>
        /// Fires an event that informs subscribers that the a packet has been
        /// received from the client.
        /// </summary>
        /// <param name="packet"></param>
        private void OnClientRead(Client c, IPacket packet)
        {
            if (ClientRead != null)
            {
                ClientRead(this, c, packet);
            }
        }

        public event ClientWriteEventHandler ClientWrite;

        public delegate void ClientWriteEventHandler(Server s, Client c, IPacket packet, long length);

        private void OnClientWrite(Client c, IPacket packet, long length, byte[] rawData)
        {
            if (ClientWrite != null)
            {
                ClientWrite(this, c, packet, length);
            }
        }


        private Socket _handle;
        private SocketAsyncEventArgs _item;

        /// <summary>
        /// Gets or sets if the server is currently processing data that
        /// should prevent disconnection. 
        /// </summary>
        private bool Processing { get; set; }

        /// <summary>
        /// Gets the status of the server. True if the server is currently
        /// listening; False if the server is not currently listening.
        /// </summary>
        public bool Listening { get; private set; }

        /// <summary>
        /// The internal list of the clients connected to the server.
        /// </summary>
        private List<Client> _clients;

        /// <summary>
        /// Gets the clients currently connected to the server, or an empty array of
        /// clients if the server is currently not listening.
        /// </summary>
        public Client[] Clients
        {
            get { return Listening ? _clients.ToArray() : new Client[0]; }
        }

        public int ConnectedClients { get; set; }
        /// <summary>
        /// A collection containing all of the clients that have connected to the server.
        /// </summary>
        public Dictionary<string, DateTime> AllTimeConnectedClients { get; set; }

        private List<Type> PacketTypes { get; set; }

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
            try
            {
                if (!Listening)
                {
                    _clients = new List<Client>();

                    _item = new SocketAsyncEventArgs();
                    _item.Completed += Process;

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

                    _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    _handle.Bind(new IPEndPoint(IPAddress.Any, port));
                    _handle.Listen(1000);

                    Processing = false;
                    Listening = true;

                    OnServerState(true);

                    if (!_handle.AcceptAsync(_item))
                        Process(null, _item);
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
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="type">Type to be added</param>
        public void AddTypeToSerializer(Type parent, Type type)
        {
            if (type == null || parent == null)
                throw new ArgumentNullException();

            PacketTypes.Add(type);
        }

        public void AddTypesToSerializer(Type parent, params Type[] types)
        {
            foreach (Type type in types)
                AddTypeToSerializer(parent, type);
        }

        /// <summary>
        /// Processes an incoming client; adding the client to the list of clients,
        /// hooking up the client's events, and finally accepts the client.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    Client client = new Client(this, e.AcceptSocket, PacketTypes.ToArray());

                    lock (_clients)
                    {
                        _clients.Add(client);
                        client.ClientState += OnClientState;
                        client.ClientRead += OnClientRead;
                        client.ClientWrite += OnClientWrite;

                        OnClientState(client, true);
                    }

                    e.AcceptSocket = null;
                    if (!_handle.AcceptAsync(e))
                        Process(null, e);
                }
                else
                    Disconnect();
            }
            catch
            {
                Disconnect();
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
                _handle.Close();

            lock (_clients)
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