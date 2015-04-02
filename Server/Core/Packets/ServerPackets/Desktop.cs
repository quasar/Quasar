using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Desktop : IPacket
    {
        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Number { get; set; }

        public Desktop() { }
        public Desktop(int quality, int number)
        {
            this.Quality = quality;
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send<Desktop>(this);
        }
    }
}
