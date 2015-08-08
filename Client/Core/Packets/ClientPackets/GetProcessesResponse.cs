using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetProcessesResponse : IPacket
    {
        public string[] Processes { get; set; }

        public int[] IDs { get; set; }

        public string[] Titles { get; set; }

        public GetProcessesResponse()
        {
        }

        public GetProcessesResponse(string[] processes, int[] ids, string[] titles)
        {
            this.Processes = processes;
            this.IDs = ids;
            this.Titles = titles;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}