using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DesktopResponse : IPacket
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Quality { get; set; }

        [ProtoMember(3)]
        public int Monitor { get; set; }

        public DesktopResponse()
        {
        }

        public DesktopResponse(byte[] image, int quality, int monitor)
        {
            this.Image = image;
            this.Quality = quality;
            this.Monitor = monitor;
        }

        public void Execute(Client client)
        {
            client.Send<DesktopResponse>(this);
        }
    }
}