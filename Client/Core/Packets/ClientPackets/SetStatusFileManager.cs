using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class SetStatusFileManager : IPacket
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        [ProtoMember(2)]
        public bool SetLastDirectorySeen { get; set; }

        public SetStatusFileManager()
        {
        }

        public SetStatusFileManager(string message, bool setLastDirectorySeen)
        {
            Message = message;
            SetLastDirectorySeen = setLastDirectorySeen;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}