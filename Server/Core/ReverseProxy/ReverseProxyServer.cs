using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xServer.Core.ReverseProxy
{
    public class ReverseProxyServer
    {
        public delegate void ConnectionEstablishedCallback(ReverseProxyClient ProxyClient);
        public delegate void UpdateConnectionCallback(ReverseProxyClient ProxyClient);

        public event ConnectionEstablishedCallback onConnectionEstablished;
        public event UpdateConnectionCallback onUpdateConnection;

        private Socket socket;
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

        public Client Client { get; private set; }

        public ReverseProxyServer()
        {
            _clients = new List<ReverseProxyClient>();
        }

        public void StartServer(Client client, string IpAddress, int Port)
        {
            Stop();

            this.Client = client;
            this.Client.Value.ProxyServer = this;

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Bind(new IPEndPoint(IPAddress.Parse(IpAddress), Port));
            this.socket.Listen(100);
            this.socket.BeginAccept(socket_BeginAccept, null);
        }

        private void socket_BeginAccept(IAsyncResult ar)
        {
            try
            {
                lock (_clients)
                {
                    _clients.Add(new ReverseProxyClient(Client, this.socket.EndAccept(ar), this));
                }
            }
            catch { }

            try
            {
                this.socket.BeginAccept(socket_BeginAccept, null);
            }
            catch { /* Server stopped listening */ }
        }

        public void Stop()
        {
            if (socket != null)
                socket.Close();

            lock (_clients)
            {
                foreach (ReverseProxyClient client in new List<ReverseProxyClient>(_clients))
                    client.Disconnect();
                _clients.Clear();
            }
        }

        public ReverseProxyClient GetClientByConnectionId(int ConnectionId)
        {
            lock (_clients)
            {
                for (int i = 0; i < _clients.Count; i++)
                {
                    if (_clients[i].ConnectionId == ConnectionId)
                        return _clients[i];
                }
                return null;
            }
        }

        internal void CallonConnectionEstablished(ReverseProxyClient ProxyClient)
        {
            try
            {
                if (onConnectionEstablished != null)
                    onConnectionEstablished(ProxyClient);
            }
            catch { }
        }
        internal void CallonUpdateConnection(ReverseProxyClient ProxyClient)
        {
            //remove a client that has been disconnected
            try
            {
                if (!ProxyClient.IsConnected)
                {
                    lock (_clients)
                    {
                        for (int i = 0; i < _clients.Count; i++)
                        {
                            if (_clients[i].ConnectionId == ProxyClient.ConnectionId)
                            {
                                _clients.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            catch { }

            try
            {
                if (onUpdateConnection != null)
                    onUpdateConnection(ProxyClient);
            }
            catch { }
        }
    }
}
