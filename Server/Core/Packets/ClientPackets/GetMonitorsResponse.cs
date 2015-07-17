using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetMonitorsResponse : IPacket
    {
        [ProtoMember(1)]
        public int Number { get; set; }

        public GetMonitorsResponse()
        {
        }

        public GetMonitorsResponse(int number)
        {
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}