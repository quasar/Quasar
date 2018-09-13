using Microsoft.Win32;
using ProtoBuf;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class RegValueData
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public RegistryValueKind Kind { get; set; }

        [ProtoMember(3)]
        public object Data { get; set; }
        // TODO: Fix Object

        public override string ToString()
        {
            return $"({Name}:{Kind}:{Data})";
        }
    }
}
