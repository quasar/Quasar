using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetDeleteRegistryKeyResponse : IPacket
    {
        public string ParentPath { get; set; }
        public string KeyName { get; set; }

        public bool IsError { get; set; }
        public string ErrorMsg { get; set; }

        public GetDeleteRegistryKeyResponse() { }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
