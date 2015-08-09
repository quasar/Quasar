using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetDirectory : IPacket
    {
        public string RemotePath { get; set; }

        public GetDirectory()
        {
        }

        public GetDirectory(string remotepath)
        {
            this.RemotePath = remotepath;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}