using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetWebcam : IPacket
    {
        [ProtoMember(1)]
        public int Webcam { get; set; }

        [ProtoMember(2)]
        public int Resolution { get; set; }
    }
}
