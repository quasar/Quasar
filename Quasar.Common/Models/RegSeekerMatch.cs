using ProtoBuf;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class RegSeekerMatch
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public RegValueData[] Data { get; set; }

        [ProtoMember(3)]
        public bool HasSubKeys { get; set; }

        public override string ToString()
        {
            return $"({Key}:{Data})";
        }
    }
}
