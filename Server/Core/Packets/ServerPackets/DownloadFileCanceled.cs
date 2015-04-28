using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadFileCanceled : IPacket
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        public DownloadFileCanceled()
        {
        }

        public DownloadFileCanceled(int id)
        {
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send<DownloadFileCanceled>(this);
        }
    }
}