using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetSystemInfoResponse : IMessage
    {
        [ProtoMember(1)]
        public List<Tuple<string, string>> SystemInfos { get; set; }
    }
}
