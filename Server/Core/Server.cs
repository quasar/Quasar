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
        private readonly object locker = new object();

        private ServerStateEventHandler serverState;
        public event ServerStateEventHandler ServerState
        {
            add
            {
                lock (locker)
                    serverState += value;
            }
            remove
            {
                lock (locker)
                    serverState -= value;
            }
        }

        public delegate void ServerStateEventHandler(Server s, bool listening);

        private void OnServerState(bool listening)
        {
            ServerStateEventHandler handler;
            lock (locker)
            {
                handler = serverState;
            }
            if (handler != null)
            {
                handler(this, listening);
            }
        }

        private ClientStateEventHandler clientState;
        public event ClientStateEventHandler ClientState
        {
            add
            {
                lock (locker)
                    clientState += value;
            }
            remove
            {
                lock (locker)
                    clientState -= value;
            }
        }

        public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

        private void OnClientState(Client c, bool connected)
        {
            ClientStateEventHandler handler;
            lock (locker)
            {
                handler = clientState;
            }
            if (handler != null)
            {
                handler(this, c, connected);
            }
        }

        private ClientReadEventHandler clientRead;
        public event ClientReadEventHandler ClientRead
        {
            add
            {
                lock (locker)
                    clientRead += value;
            }
            remove
            {
                lock (locker)
                    clientRead -= value;
            }
        }

        public delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

        private void OnClientRead(Client c, IPacket packet)
        {
            ClientReadEventHandler handler;
            lock (locker)
            {
                handler = clientRead;
            }
            if (handler != null)
            {
                handler(this, c, packet);
            }
        }

        private ClientWriteEventHandler clientWrite;
        public event ClientWriteEventHandler ClientWrite
        {
            add
            {
                lock (locker)
                    clientWrite += value;
            }
            remove
            {
                lock (locker)
                    clientWrite -= value;
            }
        }

        public delegate void ClientWriteEventHandler(Server s, Client c, IPacket packet, long length);

        private void OnClientWrite(Client c, IPacket packet, long length, byte[] rawData)
        {
            ClientWriteEventHandler handler;
            lock (locker)
            {
                handler = clientWrite;
            }
            if (handler != null)
            {
                handler(this, c, packet, length);
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

            lock (locker)
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
                    lock (locker)
                    {
                        Client client = new Client(this, e.AcceptSocket, PacketTypes.ToArray());

                        client.ClientState += OnClientState;
                        client.ClientRead += OnClientRead;
                        client.ClientWrite += OnClientWrite;

                        _clients.Add(client);

                        OnClientState(client, true);

                        e.AcceptSocket = null;
                    }

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

            lock (locker)
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

                if (_handle != null)
                    _handle.Close();
            }

            Listening = false;
            OnServerState(false);
        }
    }
}