using ProtoBuf;
using Quasar.Common.Models;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetProcessesResponse : IMessage
    {
        [ProtoMember(1)]
        public Process[] Processes { get; set; }
    }
}
