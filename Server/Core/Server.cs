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

        public event ServerStateEventHandler ServerState;

        public delegate void ServerStateEventHandler(Server s, bool listening);

        private void OnServerState(bool listening)
        {
            if (ServerState != null)
            {
                ServerState(this, listening);
            }
        }

        public event ClientStateEventHandler ClientState;

        public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

        private void OnClientState(Client c, bool connected)
        {
            if (ClientState != null)
            {
                ClientState(this, c, connected);
            }
        }

        public event ClientReadEventHandler ClientRead;

        public delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

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

        private bool Processing { get; set; }

        public bool Listening { get; private set; }

        private List<Client> _clients;

        public Client[] Clients
        {
            get { return Listening ? _clients.ToArray() : new Client[0]; }
        }

        public int ConnectedClients { get; set; }
        public Dictionary<string, DateTime> AllTimeConnectedClients { get; set; }

        private List<Type> PacketTypes { get; set; }

        public Server()
        {
            PacketTypes = new List<Type>();
            AllTimeConnectedClients = new Dictionary<string, DateTime>();
        }

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

        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    Client T = new Client(this, e.AcceptSocket, PacketTypes.ToArray());

                    lock (_clients)
                    {
                        _clients.Add(T);
                        T.ClientState += HandleState;
                        T.ClientRead += OnClientRead;
                        T.ClientWrite += OnClientWrite;

                        OnClientState(T, true);
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

        private void HandleState(Client s, bool open)
        {
            lock (_clients)
            {
                _clients.Remove(s);
                OnClientState(s, false);

                s.Value.DisposeForms();
            }
        }
    }
}