using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class Status : IPacket
    {
        public Status()
        {
        }

        public Status(string message)
        {
            Message = message;
        }

        [ProtoMember(1)]
        public string Message { get; set; }

        public void Execute(Client client)
        {
            client.Send<Status>(this);
        }
    }
}