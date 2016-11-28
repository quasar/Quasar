using xServer.Core.NAudio.Wave.WaveFormats;

namespace xServer.Core.NAudio.Wave.WaveOutputs
{
    /// <summary>
    /// Like IWaveProvider, but makes it much simpler to put together a 32 bit floating
    /// point mixing engine
    /// </summary>
    public interface ISampleProvider
    {
        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>The wave format.</value>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        int Read(float[] buffer, int offset, int count);
    }
}
