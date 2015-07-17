using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoDownloadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool RunHidden { get; set; }

        public DoDownloadAndExecute()
        {
        }

        public DoDownloadAndExecute(string url, bool runhidden)
        {
            this.URL = url;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}