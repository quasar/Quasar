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
using xClient.Config;
using xClient.Core.Compression;
using xClient.Core.Encryption;
using xClient.Core.Packets;
using xClient.Core.Packets.ClientPackets;
using xClient.Core.Packets.ServerPackets;

namespace xClient.Core
{
    public class Client
    {
        //TODO: Lock objects where needed.
        //TODO: Create and handle ReadQueue.

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
                try
                {
                    if (compressionEnabled)
                        e = new LZ4Decompressor32().Decompress(e);

                    if (encryptionEnabled)
                        e = AES.Decrypt(e, Encoding.UTF8.GetBytes(Settings.PASSWORD));

                    using (MemoryStream deserialized = new MemoryStream(e))
                    {
                        IPacket packet = Serializer.DeserializeWithLengthPrefix<IPacket>(deserialized, PrefixStyle.Fixed32);

                        if (packet.GetType() == typeof(KeepAlive))
                            new KeepAliveResponse() { TimeSent = ((KeepAlive)packet).TimeSent }.Execute(this);
                        else if (packet.GetType() == typeof(KeepAliveResponse))
                            HandleKeepAlivePacket((KeepAliveResponse)packet, this);
                        else
                            ClientRead(this, packet);
                    }
                }
                catch
                {
                    new UnknownPacket().Execute(this);
                }
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

        private readonly AsyncOperation _asyncOperation;
        private Socket _handle;

        private int _sendIndex;
        private byte[] _sendBuffer;

        private int _readIndex;
        private byte[] _readBuffer;

        private Queue<byte[]> _sendQueue;

        private SocketAsyncEventArgs[] _item = new SocketAsyncEventArgs[2];

        private List<KeepAlive> _keepAlives;

        private bool[] _processing = new bool[2];

        public int BufferSize { get; set; }

        private IPEndPoint _endPoint;
        public IPEndPoint EndPoint
        {
            get
            {
                return _endPoint ?? new IPEndPoint(IPAddress.None, 0);
            }
        }

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        public bool Connected { get; private set; }

        private int _typeIndex = 0;

        public Client(int bufferSize)
        {
            _asyncOperation = AsyncOperationManager.CreateOperation(null);
            BufferSize = bufferSize;
        }

        internal Client(Socket sock, int size, Type[] packets)
        {
            try
            {
                AddTypesToSerializer(typeof(IPacket), packets);

                _asyncOperation = AsyncOperationManager.CreateOperation(null);

                Initialize();
                _item[0].SetBuffer(new byte[size], 0, size);

                _handle = sock;

                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                BufferSize = size;
                _endPoint = (IPEndPoint)_handle.RemoteEndPoint;
                Connected = true;

                if (!_handle.ReceiveAsync(_item[0]))
                    Process(null, _item[0]);
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

                _item[0].RemoteEndPoint = new IPEndPoint(GetAddress(host), port);
                if (!_handle.ConnectAsync(_item[0]))
                    Process(null, _item[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnClientFail(ex);
                Disconnect();
            }
        }

        private IPAddress GetAddress(string host)
        {
            IPAddress[] hosts = Dns.GetHostAddresses(host);

            foreach (IPAddress h in hosts)
                if (h.AddressFamily == AddressFamily.InterNetwork)
                    return h;

            return null;
        }

        private void Initialize()
        {

            AddTypesToSerializer(typeof(IPacket), new Type[]
            {
                typeof(UnknownPacket),
                typeof(KeepAlive),
                typeof(KeepAliveResponse)
            });

            _processing = new bool[2];

            _sendIndex = 0;
            _readIndex = 0;

            _sendBuffer = new byte[0];
            _readBuffer = new byte[0];

            _sendQueue = new Queue<byte[]>();

            _keepAlives = new List<KeepAlive>();

            _item[0] = new SocketAsyncEventArgs();
            _item[0].Completed += Process;
            _item[1] = new SocketAsyncEventArgs();
            _item[1].Completed += Process;
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

        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Connect:

                            _endPoint = (IPEndPoint)_handle.RemoteEndPoint;

                            Connected = true;

                            _item[0].SetBuffer(new byte[BufferSize], 0, BufferSize);

                            _asyncOperation.Post(x => OnClientState((bool)x), true);

                            SendKeepAlives();

                            if (!_handle.ReceiveAsync(e))
                                Process(null, e);

                            break;
                        case SocketAsyncOperation.Receive:
                            if (!Connected)
                                return;

                            if (e.BytesTransferred != 0)
                            {
                                HandleRead(e.Buffer, 0, e.BytesTransferred);

                                e.SetBuffer(new byte[BufferSize], 0, BufferSize);

                                if (!_handle.ReceiveAsync(e))
                                    Process(null, e);
                            }
                            else
                            {
                                Disconnect();
                            }
                            break;
                        case SocketAsyncOperation.Send:
                            if (!Connected)
                                return;

                            _sendIndex += e.BytesTransferred;

                            bool eos = (_sendIndex >= _sendBuffer.Length);

                            if (_sendQueue.Count == 0 && eos)
                                _processing[1] = false;
                            else
                                HandleSendQueue();
                            break;
                    }
                }
                else
                {
                    if (e.LastOperation == SocketAsyncOperation.Connect)
                        _asyncOperation.Post(x => OnClientFail(new Exception("failed to connect")), null);
                    Disconnect();
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            //if (_processing[0])
            //    return;

            _processing[0] = true;

            bool raise = Connected;
            Connected = false;

            if (_handle != null)
                _handle.Close();
            if (_sendQueue != null)
                _sendQueue.Clear();
            if (_keepAlives != null)
                _keepAlives.Clear();

            _sendBuffer = new byte[0];
            _readBuffer = new byte[0];

            if (raise)
                _asyncOperation.Post(x => OnClientState(false), null);

            _endPoint = null;
        }

        public void Send<T>(IPacket packet) where T : IPacket
        {
            lock (_sendQueue)
            {
                if (!Connected) return;

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
                {
                    return;
                }
            }
        }


        private void Send(byte[] data)
        {
            if (!Connected)
                return;

            if (encryptionEnabled)
                data = AES.Encrypt(data, Encoding.UTF8.GetBytes(Settings.PASSWORD));

            if (compressionEnabled)
                data = new LZ4Compressor32().Compress(data);

            _sendQueue.Enqueue(data);

            if (!_processing[1])
            {
                _processing[1] = true;
                HandleSendQueue();
            }
        }


        private void HandleSendQueue()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (_sendIndex >= _sendBuffer.Length)
                    {
                        _sendIndex = 0;
                        _sendBuffer = Header(_sendQueue.Dequeue());
                    }

                    int write = Math.Min(_sendBuffer.Length - _sendIndex, BufferSize);

                    _item[1].SetBuffer(_sendBuffer, _sendIndex, write);

                    if (!_handle.SendAsync(_item[1]))
                        Process(null, _item[1]);

                    return;
                }
                catch
                {
                    continue;
                }
            }
            Disconnect();
        }

        private byte[] Header(byte[] data)
        {
            byte[] T = new byte[data.Length + 4];
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, T, 0, 4);
            Buffer.BlockCopy(data, 0, T, 4, data.Length);
            return T;
        }

        private void HandleRead(byte[] data, int index, int length)
        {
            try
            {
                if (_readIndex >= _readBuffer.Length)
                {
                    _readIndex = 0;
                    int len = BitConverter.ToInt32(data, index);
                    Array.Resize(ref _readBuffer, (len < 0) ? _readBuffer.Length : len);
                    index += 4;
                }

                int read = Math.Min(_readBuffer.Length - _readIndex, length - index);
                Buffer.BlockCopy(data, index, _readBuffer, _readIndex, read);
                _readIndex += read;

                if (_readIndex >= _readBuffer.Length)
                {
                    _asyncOperation.Post(x => OnClientRead((byte[])x), _readBuffer);
                }

                if (read < (length - index))
                {
                    HandleRead(data, index + read, length);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        private void SendKeepAlives()
        {
            new Thread(() =>
            {
                while (Connected)
                {
                    try
                    {
                        KeepAlive keepAlive = new KeepAlive();
                        lock (_keepAlives)
                        {
                            _keepAlives.Add(keepAlive);
                        }
                        keepAlive.Execute(this);
                        Timer timer = new Timer(KeepAliveCallback, keepAlive, 25000, Timeout.Infinite);
                    }
                    catch
                    {

                    }
                    Thread.Sleep(15000);
                }

            }) { IsBackground = true }.Start();
        }

        private void KeepAliveCallback(object state)
        {
            KeepAlive keepAlive = (KeepAlive)state;

            if (_keepAlives.Contains(keepAlive))
            {
                Disconnect();
            }
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
