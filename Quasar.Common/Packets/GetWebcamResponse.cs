using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetWebcamResponse : IPacket
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Webcam { get; set; }

        [ProtoMember(3)]
        public int Resolution { get; set; }
    }
}
