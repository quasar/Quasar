using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetSystemInfoResponse : IPacket
    {
        public string[] SystemInfos { get; set; }

        public GetSystemInfoResponse()
        {
        }

        public GetSystemInfoResponse(string[] systeminfos)
        {
            this.SystemInfos = systeminfos;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}