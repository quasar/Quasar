using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class SetStatus : IPacket
    {
        [ProtoMember(1)]
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