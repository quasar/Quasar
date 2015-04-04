using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using xServer.Core.Packets;
using xServer.Core.Packets.ClientPackets;
using xServer.Core.Packets.ServerPackets;

namespace xServer.Core
{
    public class Server
    {
        public delegate void ClientReadEventHandler(Server s, Client c, IPacket packet);

        public delegate void ClientStateEventHandler(Server s, Client c, bool connected);

        public delegate void ClientWriteEventHandler(Server s, Client c, IPacket packet, long length);

        public delegate void ServerStateEventHandler(Server s, bool listening);

        private List<Client> _clients;
        private Socket _handle;
        private SocketAsyncEventArgs _item;
        private List<KeepAlive> _keepAlives;

        public Server()
        {
            PacketTypes = new List<Type>();
            AllTimeConnectedClients = new Dictionary<string, DateTime>();
        }

        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }
        private bool Processing { get; set; }
        public bool Listening { get; private set; }

        public Client[] Clients
        {
            get { return Listening ? _clients.ToArray() : new Client[0]; }
        }

        public int ConnectedClients { get; set; }
        public Dictionary<string, DateTime> AllTimeConnectedClients { get; set; }
        private List<Type> PacketTypes { get; set; }
        public event ServerStateEventHandler ServerState;

        private void OnServerState(bool listening)
        {
            if (ServerState != null)
            {
                ServerState(this, listening);
            }
        }

        public event ClientStateEventHandler ClientState;

        private void OnClientState(Client c, bool connected)
        {
            if (ClientState != null)
            {
                ClientState(this, c, connected);
            }
        }

        public event ClientReadEventHandler ClientRead;

        private void OnClientRead(Client c, IPacket packet)
        {
            if (ClientRead != null)
            {
                ClientRead(this, c, packet);
            }
        }

        public event ClientWriteEventHandler ClientWrite;

        private void OnClientWrite(Client c, IPacket packet, long length, byte[] rawData)
        {
            if (ClientWrite != null)
            {
                ClientWrite(this, c, packet, length);
            }
        }

        public void Listen(ushort port)
        {
            try
            {
                if (!Listening)
                {
                    _keepAlives = new List<KeepAlive>();

                    _clients = new List<Client>();

                    _item = new SocketAsyncEventArgs();
                    _item.Completed += Process;

                    _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    _handle.Bind(new IPEndPoint(IPAddress.Any, port));
                    _handle.Listen(1000);

                    Processing = false;
                    Listening = true;

                    OnServerState(true);

                    SendKeepAlives();

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
        ///     Adds a Type to the serializer so a message can be properly serialized.
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
            foreach (var type in types)
                AddTypeToSerializer(parent, type);
        }

        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    var T = new Client(this, e.AcceptSocket, PacketTypes.ToArray());

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

        private void SendKeepAlives()
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var client in Clients)
                        {
                            var keepAlive = new KeepAlive();
                            lock (_keepAlives)
                            {
                                _keepAlives.Add(keepAlive);
                            }
                            keepAlive.Execute(client);
                            var timer = new Timer(KeepAliveCallback, keepAlive, 15000, Timeout.Infinite);
                        }
                    }
                    catch
                    {
                    }
                    Thread.Sleep(10000);
                }
            }) {IsBackground = true}.Start();
        }

        private void KeepAliveCallback(object state)
        {
            var keepAlive = (KeepAlive) state;

            if (_keepAlives != null)
            {
                if (_keepAlives.Contains(keepAlive))
                {
                    keepAlive.Client.Disconnect();
                    _keepAlives.Remove(keepAlive);
                }
            }
        }

        internal void HandleKeepAlivePacket(KeepAliveResponse packet, Client client)
        {
            foreach (var keepAlive in _keepAlives)
            {
                if (keepAlive.TimeSent == packet.TimeSent && keepAlive.Client == client)
                {
                    _keepAlives.Remove(keepAlive);
                    break;
                }
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

            _keepAlives = null;

            Listening = false;
            OnServerState(false);
        }

        private void HandleState(Client s, bool open)
        {
            lock (_clients)
            {
                _clients.Remove(s);
                OnClientState(s, false);
            }
        }
    }
}