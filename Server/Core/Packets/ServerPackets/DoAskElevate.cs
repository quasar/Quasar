using System;
using xServer.Core.Networking;
using xServer.Core.Registry;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoAskElevate : IPacket
    {
        public DoAskElevate(string keyPath, RegValueData value)
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
