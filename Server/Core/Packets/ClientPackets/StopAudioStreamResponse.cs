using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets {

    [Serializable]
    public class StopAudioStreamResponse : IPacket{

        public bool StreamStopped { get; set; }

        public StopAudioStreamResponse() {
            
        }

        public StopAudioStreamResponse(bool streamStopped) {
            StreamStopped = streamStopped;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
