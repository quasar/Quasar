using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadFileCanceled : IPacket
    {
        public DownloadFileCanceled()
        {
        }

        public DownloadFileCanceled(int id)
        {
            ID = id;
        }

        [ProtoMember(1)]
        public int ID { get; set; }

        public void Execute(Client client)
        {
            client.Send<DownloadFileCanceled>(this);
        }
    }
}