using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class UserStatus : IPacket
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        public UserStatus()
        {
        }

        public UserStatus(string message)
        {
            Message = message;
        }

        public void Execute(Client client)
        {
            client.Send<UserStatus>(this);
        }
    }
}