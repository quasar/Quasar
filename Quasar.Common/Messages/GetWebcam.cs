using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetWebcam : IMessage
    {
        [ProtoMember(1)]
        public int Webcam { get; set; }

        [ProtoMember(2)]
        public int Resolution { get; set; }
    }
}
