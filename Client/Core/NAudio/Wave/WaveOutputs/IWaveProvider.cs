using xClient.Core.NAudio.Wave.WaveFormats;

namespace xClient.Core.NAudio.Wave.WaveOutputs
{
    /// <summary>
    /// Generic interface for all WaveProviders.
    /// </summary>
    public interface IWaveProvider
    {
        /// <summary>
        /// Gets the WaveFormat of this WaveProvider.
        /// </summary>
        /// <value>The wave format.</value>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Fill the specified buffer with wave data.
        /// </summary>
        /// <param name="buffer">The buffer to fill of wave data.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>the number of bytes written to the buffer.</returns>
        int Read(byte[] buffer, int offset, int count);
    }
}
