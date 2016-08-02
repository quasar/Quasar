using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetKeyloggerLive : IPacket
    {
        public bool Enable { get; set; }

        public GetKeyloggerLive(bool enable)
        {
            this.Enable = enable;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}