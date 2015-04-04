﻿using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class AddStartupItem : IPacket
    {
        public AddStartupItem()
        {
        }

        public AddStartupItem(string name, string path, int type)
        {
            Name = name;
            Path = path;
            Type = type;
        }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Path { get; set; }

        [ProtoMember(3)]
        public int Type { get; set; }

        public void Execute(Client client)
        {
            client.Send<AddStartupItem>(this);
        }
    }
}