using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using xServer.Core.Compression;
using xServer.Core.Cryptography;
using xServer.Core.Extensions;
using xServer.Core.Packets;

namespace xServer.Core.Networking
{
    public class Client : IEquatable<Client>
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
            if (handler != null)
            {
                handler(this, connected);
            }
        }

        /// <summary>
        /// Occurs when a packet is received from the client.
        /// </summary>
        public event ClientReadEventHandler ClientRead;

        /// <summary>
        /// Represents the method that will handle a packet received from the client.
        /// </summary>
        /// <param name="s">The client that has received the packet.</param>
        /// <param name="packet">The packet that received by the client.</param>
        public delegate void ClientReadEventHandler(Client s, IPacket packet);

        /// <summary>
        /// Fires an event that informs subscribers that a packet has been
        /// received from the client.
        /// </summary>
        /// <param name="packet">The packet that received by the client.</param>
        private void OnClientRead(IPacket packet)
        {
            var handler = ClientRead;
            if (handler != null)
            {
                handler(this, packet);
            }
        }

        /// <summary>
        /// Occurs when a packet is sent by the client.
        /// </summary>
        public event ClientWriteEventHandler ClientWrite;

        /// <summary>
        /// Represents the method that will handle the sent packet.
        /// </summary>
        /// <param name="s">The client that has sent the packet.</param>
        /// <param name="packet">The packet that has been sent by the client.</param>
        /// <param name="length">The length of the packet.</param>
        /// <param name="rawData">The packet in raw bytes.</param>
        public delegate void ClientWriteEventHandler(Client s, IPacket packet, long length, byte[] rawData);

        /// <summary>
        /// Fires an event that informs subscribers that the client has sent a packet.
        /// </summary>
        /// <param name="packet">The packet that has been sent by the client.</param>
        /// <param name="length">The length of the packet.</param>
        /// <param name="rawData">The packet in raw bytes.</param>
        private void OnClientWrite(IPacket packet, long length, byte[] rawData)
        {
            var handler = ClientWrite;
            if (handler != null)
            {
                handler(this, packet, length, rawData);
            }
        }

        /// <summary>
        /// Checks whether the clients are equal.
        /// </summary>
        /// <param name="c">Client to compare with.</param>
        /// <returns>True if equal, else False.</returns>
        public bool Equals(Client c)
        {
            try
            {
                // the port is always unique for each client
                return this.EndPoint.Port.Equals(c.EndPoint.Port);
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
            return this.EndPoint.Port.GetHashCode();
        }  

        /// <summary>
        /// The type of the packet received.
        /// </summary>
        public enum ReceiveType
        {
            Header,
            Payload
        }

        /// <summary>
        /// Handle of the Client Socket.
        /// </summary>
        private Socket _handle;

        /// <summary>
        /// The Queue which holds buffers to send.
        /// </summary>
        private readonly Queue<byte[]> _sendBuffers = new Queue<byte[]>();

        /// <summary>
        /// Determines if the client is currently sending packets.
        /// </summary>
        private bool _sendingPackets;

        /// <summary>
        /// Lock object for the sending packets boolean.
        /// </summary>
        private readonly object _sendingPacketsLock = new object();

        /// <summary>
        /// The Queue which holds buffers to read.
        /// </summary>
        private readonly Queue<byte[]> _readBuffers = new Queue<byte[]>();

        /// <summary>
        /// Determines if the client is currently reading packets.
        /// </summary>
        private bool _readingPackets;

        /// <summary>
        /// Lock object for the reading packets boolean.
        /// </summary>
        private readonly object _readingPacketsLock = new object();

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
        /// Determines if the client is authenticated.
        /// </summary>
        public bool Authenticated { get; set; }

        /// <summary>
        /// Stores values of the user.
        /// </summary>
        public UserState Value { get; set; }

        /// <summary>
        /// The Endpoint which the client is connected to.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// The parent server of the client.
        /// </summary>
        private readonly Server _parentServer;

        /// <summary>
        /// The buffer for the client's incoming packets.
        /// </summary>
        private byte[] _readBuffer;

        /// <summary>
        /// The buffer for the client's incoming payload.
        /// </summary>
        private byte[] _payloadBuffer;

        /// <summary>
        /// The temporary header to store parts of the header.
        /// </summary>
        /// <remarks>
        /// This temporary header is used when we have i.e.
        /// only 2 bytes left to read from the buffer but need more
        /// which can only be read in the next Receive callback
        /// </remarks>
        private byte[] _tempHeader;

        /// <summary>
        /// Decides if we need to append bytes to the header.
        /// </summary>
        private bool _appendHeader;

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        public Client(Server parentServer, Socket socket)
        {
            try
            {
                _parentServer = parentServer;
                Initialize();

                _handle = socket;
                _handle.SetKeepAliveEx(_parentServer.KEEP_ALIVE_INTERVAL, _parentServer.KEEP_ALIVE_TIME);

                EndPoint = (IPEndPoint)_handle.RemoteEndPoint;
                ConnectedTime = DateTime.UtcNow;

                _readBuffer = _parentServer.BufferManager.GetBuffer();
                _tempHeader = new byte[_parentServer.HEADER_SIZE];

                _handle.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, AsyncReceive, null);
                OnClientState(true);
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private void Initialize()
        {
            Authenticated = false;
            Value = new UserState();
        }

        private void AsyncReceive(IAsyncResult result)
        {
            int bytesTransferred;

            try
            {
                bytesTransferred = _handle.EndReceive(result);

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

            _parentServer.BytesReceived += bytesTransferred;

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

            lock (_readingPacketsLock)
            {
                if (!_readingPackets)
                {
                    _readingPackets = true;
                    ThreadPool.QueueUserWorkItem(AsyncReceive);
                }
            }

            try
            {
                _handle.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, AsyncReceive, null);
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
                        lock (_readingPacketsLock)
                        {
                            _readingPackets = false;
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
                                if (_readableDataLen + _tempHeaderOffset >= _parentServer.HEADER_SIZE)
                                { // we can read the header
                                    int headerLength = (_appendHeader)
                                        ? _parentServer.HEADER_SIZE - _tempHeaderOffset
                                        : _parentServer.HEADER_SIZE;

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

                                        if (_payloadLen <= 0 || _payloadLen > _parentServer.MAX_PACKET_SIZE)
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
                                        if (encryptionEnabled)
                                            _payloadBuffer = AES.Decrypt(_payloadBuffer);

                                        isError = _payloadBuffer.Length == 0; // check if payload decryption failed
                                    }

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
                                            IPacket packet = (IPacket)_parentServer.Serializer.Deserialize(deserialized);

                                            OnClientRead(packet);
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
                    _writeOffset = 0; // prepare for next packet
                }
                _readOffset = 0;
                _readableDataLen = 0;
            }
        }

        /// <summary>
        /// Sends a packet to the connected client.
        /// </summary>
        /// <typeparam name="T">The type of the packet.</typeparam>
        /// <param name="packet">The packet to be send.</param>
        public void Send<T>(T packet) where T : IPacket
        {
            if (!Connected || packet == null) return;

            lock (_sendBuffers)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        _parentServer.Serializer.Serialize(ms, packet);
                    }
                    catch (Exception)
                    {
                        Disconnect();
                        return;
                    }

                    byte[] payload = ms.ToArray();

                    _sendBuffers.Enqueue(payload);

                    OnClientWrite(packet, payload.LongLength, payload);

                    lock (_sendingPacketsLock)
                    {
                        if (_sendingPackets) return;

                        _sendingPackets = true;
                    }
                    ThreadPool.QueueUserWorkItem(Send);
                }
            }
        }

        /// <summary>
        /// Sends a packet to the connected client.
        /// Blocks the thread until all packets have been sent.
        /// </summary>
        /// <typeparam name="T">The type of the packet.</typeparam>
        /// <param name="packet">The packet to be send.</param>
        public void SendBlocking<T>(T packet) where T : IPacket
        {
            Send(packet);
            while (_sendingPackets)
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
                    var packet = BuildPacket(payload);
                    _parentServer.BytesSent += packet.Length;
                    _handle.Send(packet);
                }
                catch (Exception)
                {
                    Disconnect();
                    SendCleanup(true);
                    return;
                }
            }
        }

        private byte[] BuildPacket(byte[] payload)
        {
            if (compressionEnabled)
                payload = SafeQuickLZ.Compress(payload);

            if (encryptionEnabled)
                payload = AES.Encrypt(payload);

            byte[] packet = new byte[payload.Length + _parentServer.HEADER_SIZE];
            Array.Copy(BitConverter.GetBytes(payload.Length), packet, _parentServer.HEADER_SIZE);
            Array.Copy(payload, 0, packet, _parentServer.HEADER_SIZE, payload.Length);
            return packet;
        }

        private void SendCleanup(bool clear = false)
        {
            lock (_sendingPacketsLock)
            {
                _sendingPackets = false;
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
            if (_handle != null)
            {
                _handle.Close();
                _handle = null;
                _readOffset = 0;
                _writeOffset = 0;
                _tempHeaderOffset = 0;
                _readableDataLen = 0;
                _payloadLen = 0;
                _payloadBuffer = null;
                _receiveState = ReceiveType.Header;

                if (Value != null)
                {
                    Value.Dispose();
                    Value = null;
                }
                
                _parentServer.BufferManager.ReturnBuffer(_readBuffer);
            }

            OnClientState(false);
        }
    }
}