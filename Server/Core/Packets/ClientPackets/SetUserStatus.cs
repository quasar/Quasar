using ProtoBuf;
using xServer.Core.Networking;
using UserStatus = xServer.Core.Commands.CommandHandler.UserStatus;

namespace xServer.Core.Packets.ClientPackets
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