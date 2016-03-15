using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;
using xClient.Core.Registry;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetRegistryKeysResponse : IPacket
    {
        public RegSeekerMatch[] Matches { get; set; }

        public string RootKey { get; set; }

        public bool IsError { get; set; }
        public string ErrorMsg { get; set; }

        public GetRegistryKeysResponse()
        { }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
