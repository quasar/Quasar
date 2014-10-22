using ProtoBuf;

namespace Core.Packets.ServerPackets
{
    [ProtoContract]
    public class TextToSpeech : IPacket
    {
        [ProtoMember(1)]
        public string Speech { get; set; }

        public TextToSpeech() {}

        public TextToSpeech(string speech)
        {
            this.Speech = speech;
        }

        public void Execute(Client client)
        {
            client.Send<TextToSpeech>(this);
        }
    }
}
