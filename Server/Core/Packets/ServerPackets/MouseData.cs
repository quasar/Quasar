using ProtoBuf;
using System.Windows.Forms;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class MouseData : IPacket
    {
        [ProtoMember(1)]
        public MouseButtons MouseButton { get; set; }

        [ProtoMember(2)]
        public int PosX { get; set; }

        [ProtoMember(3)]
        public int PosY { get; set; }

        [ProtoMember(4)]
        public bool MouseDown { get; set; }

        public MouseData() { }
        public MouseData(MouseButtons mouseButton, int x, int y, bool mouseDown)
        {
            MouseButton = mouseButton;
            PosX = x;
            PosY = y;
            MouseDown = mouseDown;
        }

        public void Execute(Client client)
        {
            client.Send<MouseData>(this);
        }
    }
}
