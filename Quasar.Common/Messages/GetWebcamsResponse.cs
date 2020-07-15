using System.Collections.Generic;
using ProtoBuf;
using Quasar.Common.Video;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetWebcamsResponse : IMessage
    {
        [ProtoMember(1)]
        public Dictionary<string, List<Resolution>> Webcams { get; set; }
    }
}
