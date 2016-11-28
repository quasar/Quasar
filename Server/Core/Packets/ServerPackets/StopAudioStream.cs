using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets {

    [Serializable]
    public class StopAudioStream : IPacket{

        public StopAudioStream() {
            
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
