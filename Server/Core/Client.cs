using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
using xServer.Core.Compression;
using xServer.Core.Encryption;
using xServer.Core.Extensions;
using xServer.Core.Packets;
using xServer.Settings;

namespace xServer.Core
{
    public class Client
    {
        public event ClientStateEventHandler ClientState;

        public delegate void ClientStateEventHandler(Client s, bool connected);

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

        public delegate void ClientReadEventHandler(Client s, IPacket packet);

        private void OnClientRead(IPacket packet)
        {
            if (ClientRead != null)
            {
                ClientRead(this, packet);
            }
        }

        public event ClientWriteEventHandler ClientWrite;

        public delegate void ClientWriteEventHandler(Client s, IPacket packet, long length, byte[] rawData);

        private void OnClientWrite(IPacket packet, long length, byte[] rawData)
        {
            if (ClientWrite != null)
            {
                ClientWrite(this, packet, length, rawData);
            }
        }

        public enum ReceiveType
        {
            Header,
            Payload
        }

        public const uint KEEP_ALIVE_TIME = 25000;
        public const uint KEEP_ALIVE_INTERVAL = 25000;

        public const int HEADER_SIZE = 4;
        public const int MAX_PACKET_SIZE = (1024*1024)*1; //1MB
        private Socket _handle;
        private int _typeIndex;

        private byte[] _buffer = new byte[MAX_PACKET_SIZE];

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
        private Server _parentServer;

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

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

                SocketExtensions.SetKeepAliveEx(_handle, KEEP_ALIVE_INTERVAL, KEEP_ALIVE_TIME);

                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                _handle.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, AsyncReceive, null);
                EndPoint = (IPEndPoint) _handle.RemoteEndPoint;
                OnClientState(true);
            }
            catch
            {
                Disconnect();
            }
        }

        private void Initialize()
        {
            AddTypeToSerializer(typeof (IPacket), typeof (UnknownPacket));
        }

        private void AsyncReceive(IAsyncResult result)
        {
            int bytesTransferred = -1;
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

            _readableDataLen += bytesTransferred;
            bool process = true;

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
                        byte[] payload = new byte[_payloadLen];
                        Array.Copy(this._buffer, _readOffset, payload, 0, payload.Length);

                        if (encryptionEnabled)
                            payload = AES.Decrypt(payload, Encoding.UTF8.GetBytes(XMLSettings.Password));

                        if (payload.Length > 0)
                        {
                            if (compressionEnabled)
                                payload = new SafeQuickLZ().Decompress(payload, 0, payload.Length);

                            using (MemoryStream deserialized = new MemoryStream(payload))
                            {
                                IPacket packet = Serializer.DeserializeWithLengthPrefix<IPacket>(deserialized,
                                    PrefixStyle.Fixed32);

                                OnClientRead(packet);
                            }
                        }

                        _readOffset += _payloadLen;
                        _readableDataLen -= _payloadLen;
                        _receiveState = ReceiveType.Header;
                    }
                }
            }

            int len = _receiveState == ReceiveType.Header ? HEADER_SIZE : _payloadLen;
            if (_readOffset + len >= this._buffer.Length)
            {
                //copy the buffer to the beginning
                Array.Copy(this._buffer, _readOffset, this._buffer, 0, _readableDataLen);
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
                    _handle.BeginReceive(this._buffer, _writeOffset, _buffer.Length - _writeOffset, SocketFlags.None,
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
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Serializer.SerializeWithLengthPrefix<T>(ms, (T) packet, PrefixStyle.Fixed32);

                        byte[] data = ms.ToArray();

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

            byte[] temp = BitConverter.GetBytes(data.Length);

            byte[] payload = new byte[data.Length + 4];
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
        /// Adds a Type to the serializer so a message can be properly serialized.
        /// </summary>
        /// <param name="parent">The parent type, i.e.: IPacket</param>
        /// <param name="type">Type to be added</param>
        public void AddTypeToSerializer(Type parent, Type type)
        {
            if (type == null || parent == null)
                throw new ArgumentNullException();

            bool isAdded = false;
            foreach (SubType subType in RuntimeTypeModel.Default[parent].GetSubtypes())
                if (subType.DerivedType.Type == type)
                    isAdded = true;

            if (!isAdded)
                RuntimeTypeModel.Default[parent].AddSubType(_typeIndex += 1, type);
        }

        public void AddTypesToSerializer(Type parent, params Type[] types)
        {
            foreach (Type type in types)
                AddTypeToSerializer(parent, type);
        }
    }
}