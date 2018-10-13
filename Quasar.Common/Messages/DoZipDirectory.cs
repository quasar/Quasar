using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Messages 
{
    [ProtoContract]
    public class DoZipDirectory : IMessage
    {
        [ProtoMember(1)]
        public string Path { get; set; }
    }
}
