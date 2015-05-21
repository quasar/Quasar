using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class MouseClick : IPacket
    {
        [ProtoMember(1)]
        public bool LeftClick { get; set; }

        [ProtoMember(2)]
        public bool DoubleClick { get; set; }

        [ProtoMember(3)]
        public int X { get; set; }

        [ProtoMember(4)]
        public int Y { get; set; }

        [ProtoMember(5)]
        public int MonitorIndex { get; set; }

        public MouseClick()
        {
        }

        public MouseClick(bool leftclick, bool doubleclick, int x, int y, int monitorIndex)
        {
            this.LeftClick = leftclick;
            this.DoubleClick = doubleclick;
            this.X = x;
            this.Y = y;
            this.MonitorIndex = monitorIndex;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}