using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoCreateDirectory : IPacket
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public DoCreateDirectory(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
