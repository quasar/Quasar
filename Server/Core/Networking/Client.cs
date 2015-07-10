using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ProtoBuf;
using ProtoBuf.Meta;
using xServer.Core.Compression;
using xServer.Core.Encryption;
using xServer.Core.Extensions;
using xServer.Core.Packets;
using xServer.Settings;

namespace xServer.Core.Networking
{
    public class Client
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
            if (ClientState != null)
            {
                ClientState(this, connected);
            }

            if (!connected && !_parentServer.Processing)
                _parentServer.RemoveClient(this);
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
            if (ClientRead != null)
            {
                ClientRead(this, packet);
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
            if (ClientWrite != null)
            {
                ClientWrite(this, packet, length, rawData);
            }
        }

        /// <summary>
        /// Checks whether the clients are equal.
        /// </summary>
        /// <param name="c">Client to compare with.</param>
        /// <returns></returns>
        public bool Equals(Client c)
        {
            return this.EndPoint.Port == c.EndPoint.Port; // this port is always unique for each client
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
        private readonly Socket _handle;

        /// <summary>
        /// The internal index of the packet type.
        /// </summary>
        private int _typeIndex;

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

        //receive info
        private int _readOffset;
        private int _writeOffset;
        private int _readableDataLen;
        private int _payloadLen;
        private ReceiveType _receiveState = ReceiveType.Header;

        //Connection info
        public bool Connected { get; private set; }
        public UserState Value { get; set; }
        public IPEndPoint EndPoint { get; private set; }
        private readonly Server _parentServer;

        /// <summary>
        /// The buffer for the client's incoming packets.
        /// </summary>
        private byte[] _readBuffer;

        /// <summary>
        /// The buffer for the client's incoming payload.
        /// </summary>
        private byte[] _payloadBuffer;

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        public Client()
        {
        }

        internal Client(Server server, Socket sock, Type[] packets)
        {
            try
            {
                _parentServer = server;
                AddTypesToSerializer(typeof(IPacket), packets);
                Initialize();

                _handle = sock;
                _handle.SetKeepAliveEx(_parentServer.KEEP_ALIVE_INTERVAL, _parentServer.KEEP_ALIVE_TIME);
                _handle.NoDelay = true;

                EndPoint = (IPEndPoint)_handle.RemoteEndPoint;

                _readBuffer = Server.BufferManager.GetBuffer();

                _handle.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, AsyncReceive, null);
                OnClientState(true);
            }
            catch
            {
                Disconnect();
            }
        }

        private void Initialize()
        {
            AddTypeToSerializer(typeof(IPacket), typeof(UnknownPacket));
            Value = new UserState();
        }

        private void AsyncReceive(IAsyncResult result)
        {
            try
            {
                int bytesTransferred;

                try
                {
                    bytesTransferred = _handle.EndReceive(result);

                    if (bytesTransferred <= 0)
                    {
                        OnClientState(false);
                        return;
                    }
                }
                catch (Exception)
                {
                    OnClientState(false);
                    return;
                }

                _parentServer.BytesReceived += bytesTransferred;

                byte[] received = new byte[bytesTransferred];
                Array.Copy(_readBuffer, received, received.Length);
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
            }
            catch
            {
            }

            try
            {
                _handle.BeginReceive(_readBuffer, 0, _readBuffer.Length, SocketFlags.None, AsyncReceive, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch
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
                                process = _readableDataLen >= _parentServer.HEADER_SIZE;
                                if (process)
                                {
                                    try
                                    {
                                        _payloadLen = (int)readBuffer[_readOffset] | readBuffer[_readOffset + 1] << 8 | readBuffer[_readOffset + 2] << 16;

                                        if (_payloadLen <= 0)
                                            throw new Exception("invalid header");
                                    }
                                    catch (Exception)
                                    {
                                        process = false;
                                        break;
                                    }

                                    _readableDataLen -= _parentServer.HEADER_SIZE;
                                    _readOffset += _parentServer.HEADER_SIZE;
                                    _receiveState = ReceiveType.Payload;
                                }
                                break;
                            }
                        case ReceiveType.Payload:
                            {
                                if (_payloadBuffer == null || _payloadBuffer.Length != _payloadLen)
                                    _payloadBuffer = new byte[_payloadLen];

                                int length = _readableDataLen;
                                if (_writeOffset + _readableDataLen >= _payloadLen)
                                {
                                    length = _payloadLen - _writeOffset;
                                }

                                try
                                {
                                    Array.Copy(readBuffer, _readOffset, _payloadBuffer, _writeOffset, length);
                                }
                                catch
                                {
                                    Disconnect();
                                }

                                _writeOffset += length;
                                _readOffset += length;
                                _readableDataLen -= length;

                                if (_writeOffset == _payloadLen)
                                {
                                    if (encryptionEnabled)
                                        _payloadBuffer = AES.Decrypt(_payloadBuffer, Encoding.UTF8.GetBytes(XMLSettings.Password));

                                    if (_payloadBuffer.Length > 0)
                                    {
                                        if (compressionEnabled)
                                            _payloadBuffer = new SafeQuickLZ().Decompress(_payloadBuffer, 0, _payloadBuffer.Length);

                                        using (MemoryStream deserialized = new MemoryStream(_payloadBuffer))
                                        {
                                            IPacket packet =
                                                Serializer.DeserializeWithLengthPrefix<IPacket>(deserialized, PrefixStyle.Fixed32);

                                            OnClientRead(packet);
                                        }
                                    }
                                    else // payload decryption failed
                                        process = false;

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
            if (!Connected) return;

            lock (_sendBuffers)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Serializer.SerializeWithLengthPrefix<T>(ms, packet, PrefixStyle.Fixed32);

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
                catch
                {
                }
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

                if (compressionEnabled)
                    payload = new SafeQuickLZ().Compress(payload, 0, payload.Length, 3);

                if (encryptionEnabled)
                    payload = AES.Encrypt(payload, Encoding.UTF8.GetBytes(XMLSettings.Password));

                byte[] header = new byte[]
                {
                    (byte)payload.Length,
                    (byte)(payload.Length >> 8),
                    (byte)(payload.Length >> 16)
                };

                byte[] data = new byte[payload.Length + _parentServer.HEADER_SIZE];
                Array.Copy(header, data, header.Length);
                Array.Copy(payload, 0, data, _parentServer.HEADER_SIZE, payload.Length);

                _parentServer.BytesSent += data.Length;

                try
                {
                    _handle.Send(data);
                }
                catch (Exception ex)
                {
                    Disconnect();
                    SendCleanup(true);
                    return;
                }
            }
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
            OnClientState(false);

            if (_handle != null)
            {
                _handle.Close();
                _readOffset = 0;
                _writeOffset = 0;
                _readableDataLen = 0;
                _payloadLen = 0;
                _payloadBuffer = null;
                if (Server.BufferManager != null)
                    Server.BufferManager.ReturnBuffer(_readBuffer);
            }
        }

        /// <summary>
        /// Adds a Type to the serializer so a message can be properly serialized.
        /// </summary>
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="type">Type to be added</param>
        public void AddTypeToSerializer(Type parent, Type type)
        {
            if (type == null || parent == null)
                throw new ArgumentNullException();

            bool isAlreadyAdded = RuntimeTypeModel.Default[parent].GetSubtypes().Any(subType => subType.DerivedType.Type == type);

            if (!isAlreadyAdded)
                RuntimeTypeModel.Default[parent].AddSubType(_typeIndex += 1, type);
        }

        /// <summary>
        /// Adds Types to the serializer.
        /// </summary>
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="types">Types to add.</param>
        public void AddTypesToSerializer(Type parent, params Type[] types)
        {
            foreach (Type type in types)
                AddTypeToSerializer(parent, type);
        }
    }
}