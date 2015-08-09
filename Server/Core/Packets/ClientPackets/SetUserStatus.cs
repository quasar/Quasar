using System;
using xServer.Core.Networking;
using xServer.Enums;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class SetUserStatus : IPacket
    {
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