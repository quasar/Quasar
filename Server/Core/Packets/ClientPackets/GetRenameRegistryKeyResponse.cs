using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetRenameRegistryKeyResponse : IPacket
    {
        public string ParentPath { get; set; }
        public string OldKeyName { get; set; }
        public string NewKeyName { get; set; }

        public bool IsError { get; set; }
        public string ErrorMsg { get; set; }

        public GetRenameRegistryKeyResponse() { }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
