using ProtoBuf;
using Quasar.Common.Messages;
using System;
using System.IO;

namespace Quasar.Common.Networking
{
    public class PayloadReader : MemoryStream
    {
        private readonly Stream _innerStream;
        public bool LeaveInnerStreamOpen { get; }

        public PayloadReader(byte[] payload, int length, bool leaveInnerStreamOpen)
        {
            _innerStream = new MemoryStream(payload, 0, length, false, true);
            LeaveInnerStreamOpen = leaveInnerStreamOpen;
        }

        public PayloadReader(Stream stream, bool leaveInnerStreamOpen)
        {
            _innerStream = stream;
            LeaveInnerStreamOpen = leaveInnerStreamOpen;
        }

        public int ReadInteger()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public byte[] ReadBytes(int length)
        {
            if (_innerStream.Position + length <= _innerStream.Length)
            {
                byte[] result = new byte[length];
                _innerStream.Read(result, 0, result.Length);
                return result;
            }
            throw new OverflowException($"Unable to read {length} bytes from stream");
        }

        /// <summary>
        /// Reads the serialized message of the payload and deserializes it.
        /// </summary>
        /// <returns>The deserialized message of the payload.</returns>
        public IMessage ReadMessage()
        {
            ReadInteger();
            /* Length prefix is ignored here and already handled in Client class,
             * it would cause to much trouble to check here for split or not fully
             * received packets.
             */
            IMessage message = Serializer.Deserialize<IMessage>(_innerStream);
            return message;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (LeaveInnerStreamOpen)
                {
                    _innerStream.Flush();
                }
                else
                {
                    _innerStream.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
