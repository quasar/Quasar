using ProtoBuf;
using Quasar.Common.IO.Compression;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;

namespace Quasar.Server.Networking
{
    public class Client : IEquatable<Client>, ISender
    {
        /// <summary>
        /// Occurs when the state of the client changes.
        /// </summary>
        public event ClientStateEventHandler ClientState;

        /// <summary>
        /// Represents the method that will handle a change in a client's state.
        /// </summary>
        /// <param name="s">The client which changed its state.</param>
        /// <param name="connected">The new connection state of the client.</param>
        public delegate void ClientStateEventHandler(Client s, bool connected);

        /// <summary>
        /// Fires an event that informs subscribers that the state of the client has changed.
        /// </summary>
        /// <param name="connected">The new connection state of the client.</param>
        private void OnClientState(bool connected)
        {
            if (Connected == connected) return;

            Connected = connected;

            var handler = ClientState;
            handler?.Invoke(this, connected);
        }

        /// <summary>
        /// Occurs when a message is received from the client.
        /// </summary>
        public event ClientReadEventHandler ClientRead;

        /// <summary>
        /// Represents the method that will handle a message received from the client.
        /// </summary>
        /// <param name="s">The client that has received the message.</param>
        /// <param name="message">The message that received by the client.</param>
        /// <param name="messageLength">The length of the message.</param>
        public delegate void ClientReadEventHandler(Client s, IMessage message, int messageLength);

        /// <summary>
        /// Fires an event that informs subscribers that a message has been
        /// received from the client.
        /// </summary>
        /// <param name="message">The message that received by the client.</param>
        /// <param name="messageLength">The length of the message.</param>
        private void OnClientRead(IMessage message, int messageLength)
        {
            var handler = ClientRead;
            handler?.Invoke(this, message, messageLength);
        }

        /// <summary>
        /// Occurs when a message is sent by the client.
        /// </summary>
        public event ClientWriteEventHandler ClientWrite;

        /// <summary>
        /// Represents the method that will handle the sent message.
        /// </summary>
        /// <param name="s">The client that has sent the message.</param>
        /// <param name="message">The message that has been sent by the client.</param>
        /// <param name="messageLength">The length of the message.</param>
        public delegate void ClientWriteEventHandler(Client s, IMessage message, int messageLength);

        /// <summary>
        /// Fires an event that informs subscribers that the client has sent a message.
        /// </summary>
        /// <param name="message">The message that has been sent by the client.</param>
        /// <param name="messageLength">The length of the message.</param>
        private void OnClientWrite(IMessage message, int messageLength)
        {
            var handler = ClientWrite;
            handler?.Invoke(this, message, messageLength);
        }

        public static bool operator ==(Client c1, Client c2)
        {
            if (ReferenceEquals(c1, null))
                return ReferenceEquals(c2, null);

            return c1.Equals(c2);
        }

        public static bool operator !=(Client c1, Client c2)
        {
            return !(c1 == c2);
        }

        /// <summary>
        /// Checks whether the clients are equal.
        /// </summary>
        /// <param name="other">Client to compare with.</param>
        /// <returns>True if equal, else False.</returns>
        public bool Equals(Client other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            try
            {
                // the port is always unique for each client
                return this.EndPoint.Port.Equals(other.EndPoint.Port);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Client);
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return this.EndPoint.GetHashCode();
        }  

        /// <summary>
        /// The type of the message received.
        /// </summary>
        public enum ReceiveType
        {
            Header,
            Payload
        }

        /// <summary>
        /// The stream used for communication.
        /// </summary>
        private readonly SslStream _stream;

        /// <summary>
        /// The buffer pool to hold the receive-buffers for the clients.
        /// </summary>
        private readonly BufferPool _bufferPool;

        /// <summary>
        /// The Queue which holds buffers to send.
        /// </summary>
        private readonly Queue<byte[]> _sendBuffers = new Queue<byte[]>();

        /// <summary>
        /// Determines if the client is currently sending messages.
        /// </summary>
        private bool _sendingMessages;

        /// <summary>
        /// Lock object for the sending messages boolean.
        /// </summary>
        private readonly object _sendingMessagesLock = new object();

        /// <summary>
        /// The Queue which holds buffers to read.
        /// </summary>
        private readonly Queue<byte[]> _readBuffers = new Queue<byte[]>();

        /// <summary>
        /// Determines if the client is currently reading messages.
        /// </summary>
        private bool _readingMessages;

        /// <summary>
        /// Lock object for the reading messages boolean.
        /// </summary>
        private readonly object _readingMessagesLock = new object();

        // receive info
        private int _readOffset;
        private int _writeOffset;
        private int _tempHeaderOffset;
        private int _readableDataLen;
        private int _payloadLen;
        private ReceiveType _receiveState = ReceiveType.Header;

        /// <summary>
        /// The time when the client connected.
        /// </summary>
        public DateTime ConnectedTime { get; private set; }

        /// <summary>
        /// The connection state of the client.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Determines if the client is identified.
        /// </summary>
        public bool Identified { get; set; }

        /// <summary>
        /// Stores values of the user.
        /// </summary>
        public UserState Value { get; set; }

        /// <summary>
        /// The Endpoint which the client is connected to.
        /// </summary>
        public IPEndPoint EndPoint { get; }

        /// <summary>
        /// The buffer for the client's incoming messages.
        /// </summary>
        private readonly byte[] _readBuffer;

        /// <summary>
        /// The buffer for the client's incoming payload.
        /// </summary>
        private byte[] _payloadBuffer;

        /// <summary>
        /// The header size in bytes.
        /// </summary>
        private const int HeaderSize = 4;  // 4 B

        /// <summary>
        /// The maximum size of a message in bytes.
        /// </summary>
        private const int MaxMessageSize = (1024 * 1024) * 5; // 5 MB

        /// <summary>
        /// The temporary header to store parts of the header.
        /// </summary>
        /// <remarks>
        /// This temporary header is used when we have i.e.
        /// only 2 bytes left to read from the buffer but need more
        /// which can only be read in the next Receive callback
        /// </remarks>
        private readonly byte[] _tempHeader;

        /// <summary>
        /// Decides if we need to append bytes to the header.
        /// </summary>
        private bool _appendHeader;

        private const bool compressionEnabled = true;

        public Client(BufferPool bufferPool, SslStream stream, IPEndPoint endPoint)
        {
            try
            {
                Identified = false;
                Value = new UserState();
                EndPoint = endPoint;
                ConnectedTime = DateTime.UtcNow;
                _stream = stream;
                _bufferPool = bufferPool;
                _readBuffer = _bufferPool.GetBuffer();
                _tempHeader = new byte[HeaderSize];

                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, AsyncReceive, null);
                OnClientState(true);
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private void AsyncReceive(IAsyncResult result)
        {
            int bytesTransferred;

            try
            {
                bytesTransferred = _stream.EndRead(result);

                if (bytesTransferred <= 0)
                    throw new Exception("no bytes transferred");
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception)
            {
                Disconnect();
                return;
            }

            byte[] received = new byte[bytesTransferred];

            try
            {
                Array.Copy(_readBuffer, received, received.Length);
            }
            catch (Exception)
            {
                Disconnect();
                return;
            }

            lock (_readBuffers)
            {
                _readBuffers.Enqueue(received);
            }

            lock (_readingMessagesLock)
            {
                if (!_readingMessages)
                {
                    _readingMessages = true;
                    ThreadPool.QueueUserWorkItem(AsyncReceive);
                }
            }

            try
            {
                _stream.BeginRead(_readBuffer, 0, _readBuffer.Length, AsyncReceive, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private void AsyncReceive(object state)
        {
            while (true)
            {
                byte[] readBuffer;
                lock (_readBuffers)
                {
                    if (_readBuffers.Count == 0)
                    {
                        lock (_readingMessagesLock)
                        {
                            _readingMessages = false;
                        }
                        return;
                    }

                    readBuffer = _readBuffers.Dequeue();
                }

                _readableDataLen += readBuffer.Length;
                bool process = true;
                while (process)
                {
                    switch (_receiveState)
                    {
                        case ReceiveType.Header:
                            {
                                if (_readableDataLen + _tempHeaderOffset >= HeaderSize)
                                { // we can read the header
                                    int headerLength = (_appendHeader)
                                        ? HeaderSize - _tempHeaderOffset
                                        : HeaderSize;

                                    try
                                    {
                                        if (_appendHeader)
                                        {
                                            try
                                            {
                                                Array.Copy(readBuffer, _readOffset, _tempHeader, _tempHeaderOffset,
                                                    headerLength);
                                            }
                                            catch (Exception)
                                            {
                                                process = false;
                                                Disconnect();
                                                break;
                                            }
                                            _payloadLen = BitConverter.ToInt32(_tempHeader, 0);
                                            _tempHeaderOffset = 0;
                                            _appendHeader = false;
                                        }
                                        else
                                        {
                                            _payloadLen = BitConverter.ToInt32(readBuffer, _readOffset);
                                        }

                                        if (_payloadLen <= 0 || _payloadLen > MaxMessageSize)
                                            throw new Exception("invalid header");
                                    }
                                    catch (Exception)
                                    {
                                        process = false;
                                        Disconnect();
                                        break;
                                    }

                                    _readableDataLen -= headerLength;
                                    _readOffset += headerLength;
                                    _receiveState = ReceiveType.Payload;
                                }
                                else // _readableDataLen < _parentServer.HEADER_SIZE
                                {
                                    try
                                    {
                                        Array.Copy(readBuffer, _readOffset, _tempHeader, _tempHeaderOffset, _readableDataLen);
                                    }
                                    catch (Exception)
                                    {
                                        process = false;
                                        Disconnect();
                                        break;
                                    }
                                    _tempHeaderOffset += _readableDataLen;
                                    _appendHeader = true;
                                    process = false;
                                }
                                break;
                            }
                        case ReceiveType.Payload:
                            {
                                if (_payloadBuffer == null || _payloadBuffer.Length != _payloadLen)
                                    _payloadBuffer = new byte[_payloadLen];

                                int length = (_writeOffset + _readableDataLen >= _payloadLen)
                                    ? _payloadLen - _writeOffset
                                    : _readableDataLen;

                                try
                                {
                                    Array.Copy(readBuffer, _readOffset, _payloadBuffer, _writeOffset, length);
                                }
                                catch (Exception)
                                {
                                    process = false;
                                    Disconnect();
                                    break;
                                }

                                _writeOffset += length;
                                _readOffset += length;
                                _readableDataLen -= length;

                                if (_writeOffset == _payloadLen)
                                {
                                    bool isError = _payloadBuffer.Length == 0;

                                    if (!isError)
                                    {
                                        if (compressionEnabled)
                                        {
                                            try
                                            {
                                                _payloadBuffer = SafeQuickLZ.Decompress(_payloadBuffer);
                                            }
                                            catch (Exception)
                                            {
                                                process = false;
                                                Disconnect();
                                                break;
                                            }
                                        }

                                        isError = _payloadBuffer.Length == 0; // check if payload decompression failed
                                    }

                                    if (isError)
                                    {
                                        process = false;
                                        Disconnect();
                                        break;
                                    }

                                    using (MemoryStream deserialized = new MemoryStream(_payloadBuffer))
                                    {
                                        try
                                        {
                                            IMessage message = Serializer.Deserialize<IMessage>(deserialized);

                                            OnClientRead(message, _payloadBuffer.Length);
                                        }
                                        catch (Exception)
                                        {
                                            process = false;
                                            Disconnect();
                                            break;
                                        }
                                    }

                                    _receiveState = ReceiveType.Header;
                                    _payloadBuffer = null;
                                    _payloadLen = 0;
                                    _writeOffset = 0;
                                }

                                if (_readableDataLen == 0)
                                    process = false;

                                break;
                            }
                    }
                }

                if (_receiveState == ReceiveType.Header)
                {
                    _writeOffset = 0; // prepare for next message
                }
                _readOffset = 0;
                _readableDataLen = 0;
            }
        }

        /// <summary>
        /// Sends a message to the connected client.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message to be send.</param>
        public void Send<T>(T message) where T : IMessage
        {
            if (!Connected || message == null) return;

            lock (_sendBuffers)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        Serializer.Serialize(ms, message);
                    }
                    catch (Exception)
                    {
                        Disconnect();
                        return;
                    }

                    byte[] payload = ms.ToArray();

                    _sendBuffers.Enqueue(payload);

                    OnClientWrite(message, payload.Length);

                    lock (_sendingMessagesLock)
                    {
                        if (_sendingMessages) return;

                        _sendingMessages = true;
                    }
                    ThreadPool.QueueUserWorkItem(Send);
                }
            }
        }

        /// <summary>
        /// Sends a message to the connected client.
        /// Blocks the thread until all messages have been sent.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message to be send.</param>
        public void SendBlocking<T>(T message) where T : IMessage
        {
            Send(message);
            while (_sendingMessages)
            {
                Thread.Sleep(10);
            }
        }

        private void Send(object state)
        {
            while (true)
            {
                if (!Connected)
                {
                    SendCleanup(true);
                    return;
                }

                byte[] payload;
                lock (_sendBuffers)
                {
                    if (_sendBuffers.Count == 0)
                    {
                        SendCleanup();
                        return;
                    }

                    payload = _sendBuffers.Dequeue();
                }

                try
                {
                    var message = BuildMessage(payload);
                    _stream.Write(message);
                }
                catch (Exception)
                {
                    Disconnect();
                    SendCleanup(true);
                    return;
                }
            }
        }

        private byte[] BuildMessage(byte[] payload)
        {
            if (compressionEnabled)
                payload = SafeQuickLZ.Compress(payload);

            byte[] message = new byte[payload.Length + HeaderSize];
            Array.Copy(BitConverter.GetBytes(payload.Length), message, HeaderSize);
            Array.Copy(payload, 0, message, HeaderSize, payload.Length);
            return message;
        }

        private void SendCleanup(bool clear = false)
        {
            lock (_sendingMessagesLock)
            {
                _sendingMessages = false;
            }

            if (!clear) return;

            lock (_sendBuffers)
            {
                _sendBuffers.Clear();
            }
        }

        /// <summary>
        /// Disconnect the client from the server and dispose of
        /// resources associated with the client.
        /// </summary>
        public void Disconnect()
        {
            if (_stream != null)
            {
                _stream.Close();
                _readOffset = 0;
                _writeOffset = 0;
                _tempHeaderOffset = 0;
                _readableDataLen = 0;
                _payloadLen = 0;
                _payloadBuffer = null;
                _receiveState = ReceiveType.Header;
                
                _bufferPool.ReturnBuffer(_readBuffer);
            }

            OnClientState(false);
        }
    }
}