using System;
using System.Collections.Generic;

namespace xServer.Core.Networking.Utilities
{
    /// <summary>
    /// Implements a pool of byte arrays to improve allocation performance when parsing data.
    /// </summary>
    /// <threadsafety>This type is safe for multithreaded operations.</threadsafety>
    public class PooledBufferManager
    {
        private readonly int _bufferLength;
        private int _bufferCount;
        private readonly Stack<byte[]> _buffers;

        #region events
        /// <summary>
        /// Informs listeners when a new buffer beyond the initial length has been allocated.
        /// </summary>
        public event EventHandler NewBufferAllocated;
        /// <summary>
        /// Fires the <see>NewBufferAllocated</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnNewBufferAllocated(EventArgs e)
        {
            var handler = NewBufferAllocated;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Informs listeners that a buffer has been allocated.
        /// </summary>
        public event EventHandler BufferRequested;
        /// <summary>
        /// Raises the <see>BufferRequested</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnBufferRequested(EventArgs e)
        {
            var handler =BufferRequested;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Informs listeners that a buffer has been returned.
        /// </summary>
        public event EventHandler BufferReturned;
        /// <summary>
        /// Raises the <see>BufferReturned</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnBufferReturned(EventArgs e)
        {
            var handler = BufferReturned; 
            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the size of the buffers allocated from this pool.
        /// </summary>
        public int BufferLength
        {
            get { return _bufferLength; }
        }

        /// <summary>
        /// Gets the maximum number of buffers available at any given time from this pool.
        /// </summary>
        public int MaxBufferCount
        {
            get { return _bufferCount; }
        }

        /// <summary>
        /// Gets the current number of buffers available for use.
        /// </summary>
        public int BuffersAvailable
        {
            get { return _buffers.Count; }
        }

        /// <summary>
        /// Gets or sets whether to zero the contents of a buffer when it is returned.  
        /// </summary>
        public bool ClearOnReturn { get; set; }
        #endregion

        #region constructor
        /// <summary>
        /// Creates a new buffer pool with the specified name, buffer sizes, and buffer count.
        /// </summary>
        /// <param name="baseBufferLength">The size of the preallocated buffers.</param>
        /// <param name="baseBufferCount">The number of preallocated buffers that should be available.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="baseBufferLength"/> or
        /// <paramref name="baseBufferCount"/> are zero or negative.</exception>
        public PooledBufferManager(int baseBufferLength, int baseBufferCount)
        {
            if (baseBufferLength <= 0)
                throw new ArgumentOutOfRangeException("baseBufferLength", baseBufferLength, "Buffer length must be a positive integer value.");
            if (baseBufferCount <= 0)
                throw new ArgumentOutOfRangeException("baseBufferCount", baseBufferCount, "Buffer count must be a positive integer value.");

            _bufferLength = baseBufferLength;
            _bufferCount = baseBufferCount;

            _buffers = new Stack<byte[]>(baseBufferCount);

            for (int i = 0; i < baseBufferCount; i++)
            {
                _buffers.Push(new byte[baseBufferLength]);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Gets a buffer from the available pool if one is available, or else allocates a new one.
        /// </summary>
        /// <remarks>
        /// <para>Buffers retrieved with this method should be returned to the pool by using the
        /// <see>ReturnBuffer</see> method.</para>
        /// </remarks>
        /// <returns>A <see>byte</see>[] from the pool.</returns>
        public byte[] GetBuffer()
        {
            if (_buffers.Count > 0)
            {
                lock (_buffers)
                {
                    if (_buffers.Count > 0)
                    {
                        byte[] buffer = _buffers.Pop();
                        return buffer;
                    }
                }
            }

            return AllocateNewBuffer();
        }

        private byte[] AllocateNewBuffer()
        {
            byte[] newBuffer = new byte[_bufferLength];
            _bufferCount++;
            OnNewBufferAllocated(EventArgs.Empty);

            return newBuffer;
        }

        /// <summary>
        /// Returns the specified buffer to the pool.
        /// </summary>
        /// <returns><see langword="true" /> if the buffer belonged to this pool and was freed; otherwise <see langword="false" />.</returns>
        /// <remarks>
        /// <para>If the <see>ClearOnFree</see> property is <see langword="true" />, then the buffer will be zeroed before 
        /// being restored to the pool.</para>
        /// </remarks>
        /// <param name="buffer">The buffer to return to the pool.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer" /> is <see langword="null" />.</exception>
        public bool ReturnBuffer(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length != _bufferLength)
                return false;

            if (ClearOnReturn)
                Array.Clear(buffer, 0, buffer.Length);

            lock (_buffers)
            {
                if (!_buffers.Contains(buffer))
                    _buffers.Push(buffer);
            }
            return true;
        }

        /// <summary>
        /// Increases the number of buffers available in the pool by a given size.
        /// </summary>
        /// <param name="buffersToAdd">The number of buffers to preallocate.</param>
        /// <exception cref="OutOfMemoryException">Thrown if the system is unable to preallocate the requested number of buffers.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="buffersToAdd"/> is less than or equal to 0.</exception>
        /// <remarks>
        /// <para>This method does not cause the <see>NewBufferAllocated</see> event to be raised.</para>
        /// </remarks>
        public void IncreaseBufferCount(int buffersToAdd)
        {
            if (buffersToAdd <= 0)
                throw new ArgumentOutOfRangeException("buffersToAdd", buffersToAdd, "The number of buffers to add must be a nonnegative, nonzero integer.");

            List<byte[]> newBuffers = new List<byte[]>(buffersToAdd);
            for (int i = 0; i < buffersToAdd; i++)
            {
                newBuffers.Add(new byte[_bufferLength]);
            }

            lock (_buffers)
            {
                _bufferCount += buffersToAdd;
                for (int i = 0; i < buffersToAdd; i++)
                {
                    _buffers.Push(newBuffers[i]);
                }
            }
        }

        /// <summary>
        /// Removes up to the specified number of buffers from the pool.
        /// </summary>
        /// <param name="buffersToRemove">The number of buffers to attempt to remove.</param>
        /// <returns>The number of buffers actually removed.</returns>
        /// <remarks>
        /// <para>The number of buffers removed may actually be lower than the number requested if the specified number of buffers are not free.
        /// For example, if the number of buffers free is 15, and the callee requests the removal of 20 buffers, only 15 will be freed, and so the
        /// returned value will be 15.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="buffersToRemove"/> is less than or equal to 0.</exception>
        public int DecreaseBufferCount(int buffersToRemove)
        {
            if (buffersToRemove <= 0)
                throw new ArgumentOutOfRangeException("buffersToRemove", buffersToRemove, "The number of buffers to remove must be a nonnegative, nonzero integer.");

            int numRemoved = 0;

            lock (_buffers)
            {
                for (int i = 0; i < buffersToRemove && _buffers.Count > 0; i++)
                {
                    _buffers.Pop();
                    numRemoved++;
                    _bufferCount--;
                }
            }

            return numRemoved;
        }
        #endregion
    }
}