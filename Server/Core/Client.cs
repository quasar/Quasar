using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public event ClientFailEventHandler ClientFail;
        public delegate void ClientFailEventHandler(Client s, Exception ex);

        private void OnClientFail(Exception ex)
        {
            if (ClientFail != null)
            {
                ClientFail(this, ex);
            }
        }


        public event ClientStateEventHandler ClientState;
        public delegate void ClientStateEventHandler(Client s, bool connected);

        private void OnClientState(bool connected)
        {
            if (ClientState != null)
            {
                ClientState(this, connected);
            }
        }

        public event ClientReadEventHandler ClientRead;
        public delegate void ClientReadEventHandler(Client s, IPacket packet);

        private void OnClientRead(byte[] e)
        {
            if (ClientRead != null)
            {

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

        public const int HEADER_SIZE = 4;
        public const int MAX_PACKET_SIZE = (1024 * 1024) * 1; //1MB
        private Socket _handle;
        private int _typeIndex = 0;

        private byte[] Buffer = new byte[MAX_PACKET_SIZE];

        //receive info
        private int ReadOffset = 0;
        private int WriteOffset = 0;
        private int ReadableDataLen = 0;
        private int TotalReceived = 0;
        private int PayloadLen = 0;
        private ReceiveType ReceiveState = ReceiveType.Header;

        //Connection info
        public bool Connected { get; private set; }
        private List<KeepAlive> _keepAlives;
        public UserState Value { get; set; }
        public IPEndPoint EndPoint { get; set; }
        private Server _parentServer;

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        public Client(int bufferSize)
        {

        }

        internal Client(Socket sock, int size, Type[] packets)
        {
            this._handle = sock;
            Initialize();

            AddTypesToSerializer(typeof(IPacket), packets);
            _handle.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, AynsReceive, null);

            if (_handle.Connected)
            {
                Connected = true;
                OnClientState(true);
            }
        }

        internal Client(Server server, Socket sock, int size, Type[] packets)
        {
            try
            {
                AddTypesToSerializer(typeof(IPacket), packets);
                _parentServer = server;
                Initialize();

                _handle = sock;

                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                _handle.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, AynsReceive, null);
                EndPoint = (IPEndPoint)_handle.RemoteEndPoint;
                Connected = true;
            }
            catch
            {
                Disconnect();
            }
        }

        public void Connect(string host, ushort port)
        {
            try
            {
                Disconnect();
                Initialize();

                _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                _handle.Connect(host, port);

                if (_handle.Connected)
                {
                    Connected = true;
                    _handle.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, AynsReceive, null);
                    OnClientState(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnClientFail(ex);
                Disconnect();
            }
        }

        private void Initialize()
        {
            _keepAlives = new List<KeepAlive>();

            AddTypesToSerializer(typeof(IPacket), new Type[]
            {
                typeof(UnknownPacket),
                typeof(KeepAlive),
                typeof(KeepAliveResponse)
            });
        }

        private void AynsReceive(IAsyncResult result)
        {
            int BytesTransferred = -1;
            try
            {
                BytesTransferred = _handle.EndReceive(result);

                if (BytesTransferred <= 0)
                {
                    this.Connected = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Connected = false;
                return;
            }

            _parentServer.BytesReceived += BytesTransferred;
            ReadableDataLen += BytesTransferred;
            bool Process = true;

            while (Process)
            {
                if (ReceiveState == ReceiveType.Header)
                {
                    Process = ReadableDataLen >= HEADER_SIZE;
                    if (ReadableDataLen >= HEADER_SIZE)
                    {
                        PayloadLen = BitConverter.ToInt32(Buffer, ReadOffset);

                        TotalReceived = HEADER_SIZE;
                        ReadableDataLen -= HEADER_SIZE;
                        ReadOffset += HEADER_SIZE;
                        ReceiveState = ReceiveType.Payload;
                    }
                }
                else if (ReceiveState == ReceiveType.Payload)
                {
                    Process = ReadableDataLen >= PayloadLen;
                    if (ReadableDataLen >= PayloadLen)
                    {
                        byte[] Payload = new byte[PayloadLen];
                        Array.Copy(this.Buffer, ReadOffset, Payload, 0, Payload.Length);

                        if (encryptionEnabled)
                        {
                            Payload = AES.Decrypt(Payload, Encoding.UTF8.GetBytes(XMLSettings.Password));
                        }

                        if (compressionEnabled)
                        {

                            Payload = new SafeQuickLZ().decompress(Payload, 0, Payload.Length);
                        }

                        using (MemoryStream deserialized = new MemoryStream(Payload))
                        {
                            IPacket packet = Serializer.DeserializeWithLengthPrefix<IPacket>(deserialized, PrefixStyle.Fixed32);

                            if (packet.GetType() == typeof(KeepAlive))
                                new KeepAliveResponse() { TimeSent = ((KeepAlive)packet).TimeSent }.Execute(this);
                            else if (packet.GetType() == typeof(KeepAliveResponse))
                            {
                                HandleKeepAlivePacket((KeepAliveResponse)packet, this);
                            }
                            else
                            {
                                ClientRead(this, packet);
                            }
                        }

                        TotalReceived = 0;
                        ReadOffset += PayloadLen;
                        ReadableDataLen -= PayloadLen;
                        ReceiveState = ReceiveType.Header;
                    }
                }
            }

            int len = ReceiveState == ReceiveType.Header ? HEADER_SIZE : PayloadLen;
            if (ReadOffset + len >= this.Buffer.Length)
            {
                //copy the buffer to the beginning
                Array.Copy(this.Buffer, ReadOffset, this.Buffer, 0, ReadableDataLen);
                WriteOffset = ReadableDataLen;
                ReadOffset = 0;
            }
            else
            {
                //payload fits in the buffer from the current offset
                //use BytesTransferred to write at the end of the payload
                //so that the data is not split
                WriteOffset += BytesTransferred;
            }

            try
            {
                if (Buffer.Length - WriteOffset > 0)
                {
                    _handle.BeginReceive(this.Buffer, WriteOffset, Buffer.Length - WriteOffset, SocketFlags.None, AynsReceive, null);
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
                        Serializer.SerializeWithLengthPrefix<T>(ms, (T)packet, PrefixStyle.Fixed32);

                        byte[] data = ms.ToArray();

                        Send(data);
                        OnClientWrite(packet, data.LongLength, data);
                    }
                }
                catch
                { }
            }
        }


        private void Send(byte[] data)
        {
            if (!Connected)
                return;

            if (compressionEnabled)
                data = new SafeQuickLZ().compress(data, 0, data.Length, 3);

            if (encryptionEnabled)
                data = AES.Encrypt(data, Encoding.UTF8.GetBytes(XMLSettings.Password));

            byte[] temp = BitConverter.GetBytes(data.Length);

            byte[] Payload = new byte[data.Length + 4];
            Array.Copy(temp, Payload, temp.Length);
            Array.Copy(data, 0, Payload, 4, data.Length);

            try
            {
                _handle.Send(Payload);
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            Connected = false;

            if (_handle != null)
            {
                _handle.Close();
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

        private void HandleKeepAlivePacket(KeepAliveResponse packet, Client client)
        {
            foreach (KeepAlive keepAlive in _keepAlives)
            {
                if (keepAlive.TimeSent == packet.TimeSent && keepAlive.Client == client)
                {
                    _keepAlives.Remove(keepAlive);
                    break;
                }
            }
        }
    }
}
