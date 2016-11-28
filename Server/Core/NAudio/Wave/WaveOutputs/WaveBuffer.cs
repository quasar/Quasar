using System;
using System.Runtime.InteropServices;

namespace xServer.Core.NAudio.Wave.WaveOutputs
{
    /// <summary>
    /// WaveBuffer class use to store wave datas. Data can be manipulated with arrays
    /// (<see cref="ByteBuffer"/>,<see cref="FloatBuffer"/>,<see cref="ShortBuffer"/>,<see cref="IntBuffer"/> ) that are pointing to the
    /// same memory buffer. Use the associated Count property based on the type of buffer to get the number of 
    /// data in the buffer.
    /// Implicit casting is now supported to float[], byte[], int[], short[].
    /// You must not use Length on returned arrays.
    /// 
    /// n.b. FieldOffset is 8 now to allow it to work natively on 64 bit
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public class WaveBuffer : IWaveBuffer
    {
        /// <summary>
        /// Number of Bytes
        /// </summary>
        [FieldOffset(0)]
        public int numberOfBytes;
        [FieldOffset(8)]
        private byte[] byteBuffer;
        [FieldOffset(8)]
        private float[] floatBuffer;
        [FieldOffset(8)]
        private short[] shortBuffer;
        [FieldOffset(8)]
        private int[] intBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveBuffer"/> class.
        /// </summary>
        /// <param name="sizeToAllocateInBytes">The number of bytes. The size of the final buffer will be aligned on 4 Bytes (upper bound)</param>
        public WaveBuffer(int sizeToAllocateInBytes)
        {
            int aligned4Bytes = sizeToAllocateInBytes%4;
            sizeToAllocateInBytes = (aligned4Bytes == 0) ? sizeToAllocateInBytes : sizeToAllocateInBytes + 4 - aligned4Bytes;
            // Allocating the byteBuffer is co-allocating the floatBuffer and the intBuffer
            byteBuffer = new byte[sizeToAllocateInBytes];
            numberOfBytes = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveBuffer"/> class binded to a specific byte buffer.
        /// </summary>
        /// <param name="bufferToBoundTo">A byte buffer to bound the WaveBuffer to.</param>
        public WaveBuffer(byte[] bufferToBoundTo)
        {
            BindTo(bufferToBoundTo);
        }

        /// <summary>
        /// Binds this WaveBuffer instance to a specific byte buffer.
        /// </summary>
        /// <param name="bufferToBoundTo">A byte buffer to bound the WaveBuffer to.</param>
        public void BindTo(byte[] bufferToBoundTo)
        {   
            /* WaveBuffer assumes the caller knows what they are doing. We will let this pass
             * if ( (bufferToBoundTo.Length % 4) != 0 )
            {
                throw new ArgumentException("The byte buffer to bound must be 4 bytes aligned");
            }*/
            byteBuffer = bufferToBoundTo;
            numberOfBytes = 0;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[](WaveBuffer waveBuffer)
        {
            return waveBuffer.byteBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float[](WaveBuffer waveBuffer)
        {
            return waveBuffer.floatBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int[](WaveBuffer waveBuffer)
        {
            return waveBuffer.intBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator short[](WaveBuffer waveBuffer)
        {
            return waveBuffer.shortBuffer;
        }

        /// <summary>
        /// Gets the byte buffer.
        /// </summary>
        /// <value>The byte buffer.</value>
        public byte[] ByteBuffer
        {
            get { return byteBuffer; }
        }

        /// <summary>
        /// Gets the float buffer.
        /// </summary>
        /// <value>The float buffer.</value>
        public float[] FloatBuffer
        {
            get { return floatBuffer; }
        }

        /// <summary>
        /// Gets the short buffer.
        /// </summary>
        /// <value>The short buffer.</value>
        public short[] ShortBuffer
        {
            get { return shortBuffer; }
        }

        /// <summary>
        /// Gets the int buffer.
        /// </summary>
        /// <value>The int buffer.</value>
        public int[] IntBuffer
        {
            get { return intBuffer; }
        }


        /// <summary>
        /// Gets the max size in bytes of the byte buffer..
        /// </summary>
        /// <value>Maximum number of bytes in the buffer.</value>
        public int MaxSize
        {
            get { return byteBuffer.Length; }
        }

        /// <summary>
        /// Gets or sets the byte buffer count.
        /// </summary>
        /// <value>The byte buffer count.</value>
        public int ByteBufferCount
        {
            get { return numberOfBytes; }
            set
            {
                numberOfBytes = CheckValidityCount("ByteBufferCount", value, 1);
            }
        }
        /// <summary>
        /// Gets or sets the float buffer count.
        /// </summary>
        /// <value>The float buffer count.</value>
        public int FloatBufferCount
        {
            get { return numberOfBytes / 4; }
            set
            {
                numberOfBytes = CheckValidityCount("FloatBufferCount", value, 4);
            }
        }
        /// <summary>
        /// Gets or sets the short buffer count.
        /// </summary>
        /// <value>The short buffer count.</value>
        public int ShortBufferCount
        {
            get { return numberOfBytes / 2; }
            set
            {
                numberOfBytes = CheckValidityCount("ShortBufferCount", value, 2);
            }
        }
        /// <summary>
        /// Gets or sets the int buffer count.
        /// </summary>
        /// <value>The int buffer count.</value>
        public int IntBufferCount
        {
            get { return numberOfBytes / 4; }
            set
            {
                numberOfBytes = CheckValidityCount("IntBufferCount", value, 4);
            }
        }

        /// <summary>
        /// Clears the associated buffer.
        /// </summary>
        public void Clear()
        {
            Array.Clear(byteBuffer, 0, byteBuffer.Length);
        }

        /// <summary>
        /// Copy this WaveBuffer to a destination buffer up to ByteBufferCount bytes.
        /// </summary>
        public void Copy(Array destinationArray)
        {
            Array.Copy(byteBuffer, destinationArray, numberOfBytes);
        }

        /// <summary>
        /// Checks the validity of the count parameters.
        /// </summary>
        /// <param name="argName">Name of the arg.</param>
        /// <param name="value">The value.</param>
        /// <param name="sizeOfValue">The size of value.</param>
        private int CheckValidityCount(string argName, int value, int sizeOfValue)
        {
            int newNumberOfBytes = value * sizeOfValue;
            if ( (newNumberOfBytes % 4) != 0 )
            {
                throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count ({1}) that is not 4 bytes aligned ", argName, newNumberOfBytes));
            }

            if (value < 0 || value > (byteBuffer.Length / sizeOfValue))
            {
                throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count that exceed max count {1}", argName, byteBuffer.Length / sizeOfValue));
            }
            return newNumberOfBytes;
        }
    }
}
