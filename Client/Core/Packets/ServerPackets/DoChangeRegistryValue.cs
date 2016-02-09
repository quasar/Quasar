using System;
using System.Linq;
using System.Text;
using xClient.Core.Networking;
using xClient.Core.Registry;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoChangeRegistryValue : IPacket
    {
        public string KeyPath { get; set; }
        public RegValueData Value { get; set; }

        public DoChangeRegistryValue(string keyPath, RegValueData value)
        {
            KeyPath = keyPath;
            Value = value;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
