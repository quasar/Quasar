using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Desktop : IPacket
    {
        public Desktop()
        {
        }

        public Desktop(int quality, int number)
        {
            Quality = quality;
            Number = number;
        }

        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Number { get; set; }

        public void Execute(Client client)
        {
            client.Send<Desktop>(this);
        }
    }
}