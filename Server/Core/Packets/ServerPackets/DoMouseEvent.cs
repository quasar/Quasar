using ProtoBuf;
using xServer.Core.Networking;
using xServer.Enums;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoMouseEvent : IPacket
    {
        [ProtoMember(1)]
        public RemoteDesktopAction Action { get; set; }

        [ProtoMember(2)]
        public bool IsMouseDown { get; set; }

        [ProtoMember(3)]
        public int X { get; set; }

        [ProtoMember(4)]
        public int Y { get; set; }

        [ProtoMember(5)]
        public int MonitorIndex { get; set; }

        public DoMouseEvent()
        {
        }

        public DoMouseEvent(RemoteDesktopAction action, bool isMouseDown, int x, int y, int monitorIndex)
        {
            this.Action = action;
            this.IsMouseDown = isMouseDown;
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