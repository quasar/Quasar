using ProtoBuf;
using Quasar.Common.Models;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetDirectoryResponse : IMessage
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public FileSystemEntry[] Items { get; set; }
    }
}
