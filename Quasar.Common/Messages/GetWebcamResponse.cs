using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetWebcamResponse : IMessage
    {
        [ProtoMember(1)]
        public byte[] Image { get; set; }

        [ProtoMember(2)]
        public int Webcam { get; set; }

        [ProtoMember(3)]
        public int Resolution { get; set; }
    }
}
