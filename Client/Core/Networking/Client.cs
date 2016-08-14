using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using xClient.Core.Compression;
using xClient.Core.Cryptography;
using xClient.Core.Extensions;
using xClient.Core.NetSerializer;
using xClient.Core.Packets;
using xClient.Core.ReverseProxy;
using xClient.Core.ReverseProxy.Packets;

namespace xClient.Core.Networking
{
    public class Client
    {
        /// <summary>
        /// Occurs as a result of an unrecoverable issue with the client.
        /// </summary>
        public event ClientFailEventHandler ClientFail;

        /// <summary>
        /// Represents a method that will handle failure of the client.
        /// </summary>
        /// <param name="s">The client that has failed.</param>
        /// <param name="ex">The exception containing information about the cause of the client's failure.</param>
        public delegate void ClientFailEventHandler(Client s, Exception ex);

        /// <summary>
        /// Fires an event that informs subscribers that the client has failed.
        /// </summary>
        /// <param name="ex">The exception containing information about the cause of the client's failure.</param>
        private void OnClientFail(Exception ex)
        {
            var handler = ClientFail;
            if (handler != null)
            {
                handler(this, ex);
            }
        }

        /// <summary>
        /// Occurs when the state of the client has changed.
        /// </summary>
        public event ClientStateEventHandler ClientState;

        /// <summary>
        /// Represents the method that will handle a change in the client's state
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
        /// Occurs when a packet is received from the server.
        /// </summary>
        public event ClientReadEventHandler ClientRead;

        /// <summary>
        /// Represents a method that will handle a packet from the server.
        /// </summary>
        /// <param name="s">The client that has received the packet.</param>
        /// <param name="packet">The packet that has been received by the server.</param>
        public delegate void ClientReadEventHandler(Client s, IPacket packet);

        /// <summary>
        /// Fires an event that informs subscribers that a packet has been received by the server.
        /// </summary>
        /// <param name="packet">The packet that has been received by the server.</param>
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
        /// The type of the packet received.
        /// </summary>
        public enum ReceiveType
        {
            Header,
            Payload
        }

        /// <summary>
        /// The buffer size for receiving data in bytes.
        /// </summary>
        public int BUFFER_SIZE { get { return 1024 * 16; } } // 16KB

        /// <summary>
        /// The keep-alive time in ms.
        /// </summary>
        public uint KEEP_ALIVE_TIME { get { return 25000; } } // 25s

        /// <summary>
        /// The keep-alive interval in ms.
        /// </summary>
        public uint KEEP_ALIVE_INTERVAL { get { return 25000; } } // 25s

        /// <summary>
        /// The header size in bytes.
        /// </summary>
        public int HEADER_SIZE { get { return 4; } } // 4B

        /// <summary>
        /// The maximum size of a packet in bytes.
        /// </summary>
        public int MAX_PACKET_SIZE { get { return (1024 * 1024) * 5; } } // 5MB

        /// <summary>
        /// Returns an array containing all of the proxy clients of this client.
        /// </summary>
        public ReverseProxyClient[] ProxyClients
        {
            get
            {
                lock (_proxyClientsLock)
                {
                    return _proxyClients.ToArray();
                }
            }
        }

        /// <summary>
        /// Handle of the Client Socket.
        /// </summary>
        private Socket _handle;

        /// <summary>
        /// A list of all the connected proxy clients that this client holds.
        /// </summary>
        private List<ReverseProxyClient> _proxyClients;

        /// <summary>
        /// Lock object for the list of proxy clients.
        /// </summary>
        private readonly object _proxyClientsLock = new object();

        /// <summary>
        /// The buffer for incoming packets.
        /// </summary>
        private byte[] _readBuffer;

        /// <summary>
        /// The buffer for the client's incoming payload.
        /// </summary>
        private byte[] _payloadBuffer;

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

        // Receive info
        private int _readOffset;
        private int _writeOffset;
        private int _tempHeaderOffset;
        private int _readableDataLen;
        private int _payloadLen;
        private ReceiveType _receiveState = ReceiveType.Header;

        /// <summary>
        /// Gets if the client is currently connected to a server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The packet serializer.
        /// </summary>
        protected Serializer Serializer { get; set; }

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        protected Client()
        {
            _proxyClients = new List<ReverseProxyClient>();
            _readBuffer = new byte[BUFFER_SIZE];
            _tempHeader = new byte[HEADER_SIZE];
        }

        /// <summary>
        /// Attempts to connect to the specified ip address on the specified port.
        /// </summary>
        /// <param name="ip">The ip address to connect to.</param>
        /// <param name="port">The port of the host.</param>
        protected void Connect(IPAddress ip, ushort port)
        {
            try
            {
                Disconnect();

                _handle = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _handle.SetKeepAliveEx(KEEP_ALIVE_INTERVAL, KEEP_ALIVE_TIME);
                _handle.Connect(ip, port);

                if (_handle.Connected)
                {
                    _handle.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, AsyncReceive, null);
                    OnClientState(true);
                }
            }
            catch (Exception ex)
            {
                OnClientFail(ex);
            }
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

            byte[] received = new byte[bytesTransferred];

            try
            {
                Array.Copy(_readBuffer, received, received.Length);
            }
            catch (Exception ex)
            {
                OnClientFail(ex);
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
            catch (Exception ex)
            {
                OnClientFail(ex);
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
                                if (_readableDataLen + _tempHeaderOffset >= HEADER_SIZE)
                                { // we can read the header
                                    int headerLength = (_appendHeader)
                                        ? HEADER_SIZE - _tempHeaderOffset
                                        : HEADER_SIZE;

                                    try
                                    {
                                        if (_appendHeader)
                                        {
                                            try
                                            {
                                                Array.Copy(readBuffer, _readOffset, _tempHeader, _tempHeaderOffset,
                                                    headerLength);
                                            }
                                            catch (Exception ex)
                                            {
                                                process = false;
                                                OnClientFail(ex);
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

                                        if (_payloadLen <= 0 || _payloadLen > MAX_PACKET_SIZE)
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
                                else // _readableDataLen < HEADER_SIZE
                                {
                                    try
                                    {
                                        Array.Copy(readBuffer, _readOffset, _tempHeader, _tempHeaderOffset, _readableDataLen);
                                    }
                                    catch (Exception ex)
                                    {
                                        process = false;
                                        OnClientFail(ex);
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
                                catch (Exception ex)
                                {
                                    process = false;
                                    OnClientFail(ex);
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
                                            IPacket packet = (IPacket)Serializer.Deserialize(deserialized);

                                            OnClientRead(packet);
                                        }
                                        catch (Exception ex)
                                        {
                                            process = false;
                                            OnClientFail(ex);
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
        /// Sends a packet to the connected server.
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
                        Serializer.Serialize(ms, packet);
                    }
                    catch (Exception ex)
                    {
                        OnClientFail(ex);
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
        /// Sends a packet to the connected server.
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
                    _handle.Send(BuildPacket(payload));
                }
                
                catch (Exception ex)
                {
                    OnClientFail(ex);
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

            byte[] packet = new byte[payload.Length + HEADER_SIZE];
            Array.Copy(BitConverter.GetBytes(payload.Length), packet, HEADER_SIZE);
            Array.Copy(payload, 0, packet, HEADER_SIZE, payload.Length);
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
        /// Disconnect the client from the server, disconnect all proxies that
        /// are held by this client, and dispose of other resources associated
        /// with this client.
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

                if (_proxyClients != null)
                {
                    lock (_proxyClientsLock)
                    {
                        try
                        {
                            foreach (ReverseProxyClient proxy in _proxyClients)
                                proxy.Disconnect();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (Commands.CommandHandler.StreamCodec != null)
                {
                    Commands.CommandHandler.StreamCodec.Dispose();
                    Commands.CommandHandler.StreamCodec = null;
                }
            }

            OnClientState(false);
        }

        public void ConnectReverseProxy(ReverseProxyConnect command)
        {
            lock (_proxyClientsLock)
            {
                _proxyClients.Add(new ReverseProxyClient(command, this));
            }
        }

        public ReverseProxyClient GetReverseProxyByConnectionId(int connectionId)
        {
            lock (_proxyClientsLock)
            {
                return _proxyClients.FirstOrDefault(t => t.ConnectionId == connectionId);
            }
        }

        public void RemoveProxyClient(int connectionId)
        {
            try
            {
                lock (_proxyClientsLock)
                {
                    for (int i = 0; i < _proxyClients.Count; i++)
                    {
                        if (_proxyClients[i].ConnectionId == connectionId)
                        {
                            _proxyClients.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            catch { }
        }
    }
}