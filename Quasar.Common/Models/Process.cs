using ProtoBuf;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class Process
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public string MainWindowTitle { get; set; }
    }
}
