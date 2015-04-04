using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class MouseClick : IPacket
    {
        public MouseClick()
        {
        }

        public MouseClick(bool leftclick, bool doubleclick, int x, int y)
        {
            LeftClick = leftclick;
            DoubleClick = doubleclick;
            X = x;
            Y = y;
        }

        [ProtoMember(1)]
        public bool LeftClick { get; set; }

        [ProtoMember(2)]
        public bool DoubleClick { get; set; }

        [ProtoMember(3)]
        public int X { get; set; }

        [ProtoMember(4)]
        public int Y { get; set; }

        public void Execute(Client client)
        {
            client.Send<MouseClick>(this);
        }
    }
}