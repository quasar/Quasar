using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core.Packets;
using Core.Packets.ClientPackets;
using Core.Packets.ServerPackets;

namespace Core
{
    public class Server
    {
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

        private bool Processing { get; set; }
        public int BufferSize { get; set; }

        public bool Listening { get; private set; }

        private List<KeepAlive> _keepAlives;

        private List<Type> PacketTypes { get; set; }

        public Server(int bufferSize)
        {
            PacketTypes = new List<Type>();
            BufferSize = bufferSize;
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
    }
}