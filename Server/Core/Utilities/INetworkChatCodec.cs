using System;
using xServer.Core.NAudio.Wave.WaveFormats;

namespace xServer.Core.Utilities {
    public interface INetworkChatCodec : IDisposable {

        /// <summary>
        /// Friendly Name for this codec
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Tests whether the codec is available on this system
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// Bitrate
        /// </summary>
        int BitsPerSecond { get; }

        /// <summary>
        /// Preferred PCM format for recording in (usually 8kHz mono 16 bit)
        /// </summary>
        WaveFormat RecordFormat { get; }
        /// <summary>
        /// Encodes a block of audio
        /// </summary>
        byte[] Encode(byte[] data, int offset, int length);
        /// <summary>
        /// Decodes a block of audio
        /// </summary>
        byte[] Decode(byte[] data, int offset, int length);
    }
}
