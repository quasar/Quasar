using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoPathDelete : IMessage
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public FileType PathType { get; set; }
    }
}
