using System;
using xServer.Core.Networking;
using xServer.Enums;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoMouseEvent : IPacket
    {
        public MouseAction Action { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int MonitorIndex { get; set; }

        public int DwData { get; set; }

        public DoMouseEvent()
        {
        }

        public DoMouseEvent(MouseAction action, int x, int y, int delta, int monitorIndex)
        {
            this.Action = action;
            this.X = x;
            this.Y = y;
            this.DwData = delta;
            this.MonitorIndex = monitorIndex;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}