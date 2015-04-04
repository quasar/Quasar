using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class UserStatus : IPacket
    {
        public UserStatus()
        {
        }

        public UserStatus(string message)
        {
            Message = message;
        }

        [ProtoMember(1)]
        public string Message { get; set; }

        public void Execute(Client client)
        {
            client.Send<UserStatus>(this);
        }
    }
}