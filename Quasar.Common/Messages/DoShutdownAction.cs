using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoShutdownAction : IMessage
    {
        [ProtoMember(1)]
        public ShutdownAction Action { get; set; }
    }
}
