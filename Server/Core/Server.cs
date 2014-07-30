using Core.Packets;
using Core.Packets.ClientPackets;
using Core.Packets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Core
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
        public int BufferSize { get; set; }

        public bool Listening { get; private set; }

        private List<KeepAlive> _keepAlives;

        private List<Client> _clients;
        public Client[] Clients
        {
            get
            {
                return Listening ? _clients.ToArray() : new Client[0];
            }
        }

        public int ConnectedClients { get; set; }
        public int AllTimeConnectedClients { get; set; }

        private List<Type> PacketTypes { get; set; }

        public Server(int bufferSize)
        {
            PacketTypes = new List<Type>();
            BufferSize = bufferSize;
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
                    Client T = new Client(this, e.AcceptSocket, BufferSize, PacketTypes.ToArray());

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
                        foreach (Client client in Clients)
                        {
                            KeepAlive keepAlive = new KeepAlive();
                            lock (_keepAlives)
                            {
                                _keepAlives.Add(keepAlive);
                            }
                            keepAlive.Execute(client);
                            Timer timer = new Timer(KeepAliveCallback, keepAlive, 15000, Timeout.Infinite);
                        }
                    }
                    catch
                    {

                    }
                    Thread.Sleep(10000);
                }

            }) { IsBackground = true }.Start();
        }

        private void KeepAliveCallback(object state)
        {
            KeepAlive keepAlive = (KeepAlive)state;

            if (_keepAlives.Contains(keepAlive))
            {
                keepAlive.Client.Disconnect();
                _keepAlives.Remove(keepAlive);
            }
        }

        internal void HandleKeepAlivePacket(KeepAliveResponse packet, Client client)
        {
            foreach (KeepAlive keepAlive in _keepAlives)
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
                    _clients.RemoveAt(0);
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
            }
        }
    }
}
