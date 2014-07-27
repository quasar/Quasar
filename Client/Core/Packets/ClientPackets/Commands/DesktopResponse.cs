using ProtoBuf;

namespace Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DesktopResponse : IPacket
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        public DesktopResponse()
        {
        }

        public DesktopResponse(byte[] image)
        {
            this.Image = image;
        }

        public void Execute(Client client)
        {
            client.Send<DesktopResponse>(this);
        }
    }
}