using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Messages {
    [ProtoContract]
    public class DoZipDirectory : IMessage {

        [ProtoMember(1)]
        public string ArchivePath { get; set; }

        [ProtoMember(2)]
        public string[] PathList { get; set; }

        [ProtoMember(3)]
        public FileType[] TypeList { get; set; }

    }
}
