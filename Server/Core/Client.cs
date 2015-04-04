using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
using xServer.Core.Compression;
using xServer.Core.Encryption;
using xServer.Core.Packets;
using xServer.Core.Packets.ClientPackets;
using xServer.Core.Packets.ServerPackets;
using xServer.Settings;

namespace xServer.Core
{
    public class Client
    {
        public delegate void ClientReadEventHandler(Client s, IPacket packet);

        public delegate void ClientStateEventHandler(Client s, bool connected);

        public delegate void ClientWriteEventHandler(Client s, IPacket packet, long length, byte[] rawData);

        public enum ReceiveType
        {
            Header,
            Payload
        }

        private const uint KEEP_ALIVE_TIME = 5000;
        private const uint KEEP_ALIVE_INTERVAL = 5000;

        public const int HEADER_SIZE = 4;
        public const int MAX_PACKET_SIZE = (1024*1024)*1; //1MB
        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;
        private readonly byte[] _buffer = new byte[MAX_PACKET_SIZE];
        private readonly Socket _handle;
        private readonly Server _parentServer;
        private int _payloadLen;
        private int _readableDataLen;
        //receive info
        private int _readOffset;
        private ReceiveType _receiveState = ReceiveType.Header;
        private int _typeIndex;
        private int _writeOffset;

        public Client()
        {
        }

        internal Client(Server server, Socket sock, Type[] packets)
        {
            try
            {
                AddTypesToSerializer(typeof (IPacket), packets);
                _parentServer = server;
                Initialize();

                _handle = sock;

                //_handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Misc.KeepAliveEx.SetKeepAliveEx(_handle, KEEP_ALIVE_INTERVAL, KEEP_ALIVE_TIME);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                _handle.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, AsyncReceive, null);
                EndPoint = (IPEndPoint) _handle.RemoteEndPoint;
                OnClientState(true);
            }
            catch
            {
                Disconnect();
            }
        }

        //Connection info
        public bool Connected { get; private set; }
        public UserState Value { get; set; }
        public IPEndPoint EndPoint { get; private set; }
        public event ClientStateEventHandler ClientState;

        private void OnClientState(bool connected)
        {
            if (Connected == connected) return;

            Connected = connected;
            if (ClientState != null)
            {
                ClientState(this, connected);
            }
        }

        public event ClientReadEventHandler ClientRead;

        private void OnClientRead(IPacket packet)
        {
            if (ClientRead != null)
            {
                ClientRead(this, packet);
            }
        }

        public event ClientWriteEventHandler ClientWrite;

        private void OnClientWrite(IPacket packet, long length, byte[] rawData)
        {
            if (ClientWrite != null)
            {
                ClientWrite(this, packet, length, rawData);
            }
        }

        private void Initialize()
        {
            AddTypesToSerializer(typeof (IPacket), typeof (UnknownPacket), typeof (KeepAlive),
                typeof (KeepAliveResponse));
        }

        private void AsyncReceive(IAsyncResult result)
        {
            var bytesTransferred = -1;
            try
            {
                bytesTransferred = _handle.EndReceive(result);

                if (bytesTransferred <= 0)
                {
                    OnClientState(false);
                    return;
                }
            }
            catch
            {
                OnClientState(false);
                return;
            }

            _parentServer.BytesReceived += bytesTransferred;

            _readableDataLen += bytesTransferred;
            var process = true;

            while (process)
            {
                if (_receiveState == ReceiveType.Header)
                {
                    process = _readableDataLen >= HEADER_SIZE;
                    if (process)
                    {
                        _payloadLen = BitConverter.ToInt32(_buffer, _readOffset);

                        _readableDataLen -= HEADER_SIZE;
                        _readOffset += HEADER_SIZE;
                        _receiveState = ReceiveType.Payload;
                    }
                }
                else if (_receiveState == ReceiveType.Payload)
                {
                    process = _readableDataLen >= _payloadLen;
                    if (process)
                    {
                        var payload = new byte[_payloadLen];
                        Array.Copy(_buffer, _readOffset, payload, 0, payload.Length);

                        if (encryptionEnabled)
                            payload = AES.Decrypt(payload, Encoding.UTF8.GetBytes(XMLSettings.Password));

                        if (payload.Length > 0)
                        {
                            if (compressionEnabled)
                                payload = new SafeQuickLZ().Decompress(payload, 0, payload.Length);

                            using (var deserialized = new MemoryStream(payload))
                            {
                                var packet = Serializer.DeserializeWithLengthPrefix<IPacket>(deserialized,
                                    PrefixStyle.Fixed32);

                                if (packet.GetType() == typeof (KeepAlive))
                                    new KeepAliveResponse {TimeSent = ((KeepAlive) packet).TimeSent}.Execute(this);
                                else if (packet.GetType() == typeof (KeepAliveResponse))
                                    _parentServer.HandleKeepAlivePacket((KeepAliveResponse) packet, this); // HERE
                                else
                                    OnClientRead(packet);
                            }
                        }

                        _readOffset += _payloadLen;
                        _readableDataLen -= _payloadLen;
                        _receiveState = ReceiveType.Header;
                    }
                }
            }

            var len = _receiveState == ReceiveType.Header ? HEADER_SIZE : _payloadLen;
            if (_readOffset + len >= _buffer.Length)
            {
                //copy the buffer to the beginning
                Array.Copy(_buffer, _readOffset, _buffer, 0, _readableDataLen);
                _writeOffset = _readableDataLen;
                _readOffset = 0;
            }
            else
            {
                //payload fits in the buffer from the current offset
                //use BytesTransferred to write at the end of the payload
                //so that the data is not split
                _writeOffset += bytesTransferred;
            }

            try
            {
                if (_buffer.Length - _writeOffset > 0)
                {
                    _handle.BeginReceive(_buffer, _writeOffset, _buffer.Length - _writeOffset, SocketFlags.None,
                        AsyncReceive, null);
                }
                else
                {
                    //Shoudln't be even possible... very strange
                    Disconnect();
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Send<T>(IPacket packet) where T : IPacket
        {
            lock (_handle)
            {
                if (!Connected)
                    return;

                try
                {
                    using (var ms = new MemoryStream())
                    {
                        Serializer.SerializeWithLengthPrefix(ms, (T) packet, PrefixStyle.Fixed32);

                        var data = ms.ToArray();

                        Send(data);
                        OnClientWrite(packet, data.LongLength, data);
                    }
                }
                catch
                {
                }
            }
        }

        private void Send(byte[] data)
        {
            if (!Connected)
                return;

            if (compressionEnabled)
                data = new SafeQuickLZ().Compress(data, 0, data.Length, 3);

            if (encryptionEnabled)
                data = AES.Encrypt(data, Encoding.UTF8.GetBytes(XMLSettings.Password));

            var temp = BitConverter.GetBytes(data.Length);

            var payload = new byte[data.Length + 4];
            Array.Copy(temp, payload, temp.Length);
            Array.Copy(data, 0, payload, 4, data.Length);

            _parentServer.BytesSent += payload.Length;

            try
            {
                _handle.Send(payload);
            }
            catch
            {
                Disconnect();
            }
        }

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
            }
        }

        /// <summary>
        ///     Adds a Type to the serializer so a message can be properly serialized.
        /// </summary>
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="type">Type to be added</param>
        public void AddTypeToSerializer(Type parent, Type type)
        {
            if (type == null || parent == null)
                throw new ArgumentNullException();

            var isAdded = false;
            foreach (var subType in RuntimeTypeModel.Default[parent].GetSubtypes())
                if (subType.DerivedType.Type == type)
                    isAdded = true;

            if (!isAdded)
                RuntimeTypeModel.Default[parent].AddSubType(_typeIndex += 1, type);
        }

        public void AddTypesToSerializer(Type parent, params Type[] types)
        {
            foreach (var type in types)
                AddTypeToSerializer(parent, type);
        }
    }
}