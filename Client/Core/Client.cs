using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;
using xClient.Config;
using xClient.Core.Compression;
using xClient.Core.Encryption;
using xClient.Core.Extensions;
using xClient.Core.Packets;
using xClient.Core.ReverseProxy.Packets;
using System.Collections.Generic;
using System.Linq;
using xClient.Core.ReverseProxy;
using System.Net;

namespace xClient.Core
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

        private static readonly Random _rnd = new Random();

        /// <summary>
        /// Fires an event that informs subscribers that the client has failed.
        /// </summary>
        /// <param name="ex">The exception containing information about the cause of the client's failure.</param>
        private void OnClientFail(Exception ex)
        {
            if (ClientFail != null)
            {
                ClientFail(this, ex);
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
            if (ClientState != null)
            {
                ClientState(this, connected);
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
        /// The type of the packet received.
        /// </summary>
        public enum ReceiveType
        {
            Header,
            Payload
        }

        /// <summary>
        /// A list of all the connected proxy clients that this client holds.
        /// </summary>
        private List<ReverseProxyClient> _proxyClients;

        /// <summary>
        /// Lock object for the list of proxy clients.
        /// </summary>
        private readonly object _proxyClientsLock = new object();

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

        public const uint KEEP_ALIVE_TIME = 25000;
        public const uint KEEP_ALIVE_INTERVAL = 25000;

        public const int HEADER_SIZE = 4;
        public const int MAX_PACKET_SIZE = (1024*1024)*2; //2MB
        private Socket _handle;
        private int _typeIndex;

        /// <summary>
        /// The buffer for the client's incoming and outgoing packets.
        /// </summary>
        private byte[] _buffer = new byte[MAX_PACKET_SIZE];

        //receive info
        private int _readOffset;
        private int _writeOffset;
        private int _readableDataLen;
        private int _payloadLen;
        private ReceiveType _receiveState = ReceiveType.Header;

        /// <summary>
        /// Gets if the client is currently connected to a server.
        /// </summary>
        public bool Connected { get; private set; }

        private const bool encryptionEnabled = true;
        private const bool compressionEnabled = true;

        public Client()
        {
        }

        /// <summary>
        /// Attempts to connect to the specified host on the specified port.
        /// </summary>
        /// <param name="host">The host (or server) to connect to.</param>
        /// <param name="port">The port of the host.</param>
        public void Connect(string csvListOfHosts, string csvListOfPorts)
        {
            try
            {
                Disconnect();
                Initialize();

                char[] comma = {','};

                string[] hostList = csvListOfHosts.Split(comma);
                string[] portList = csvListOfPorts.Split(comma);

                int index = getIndexFor(hostList, portList);
                String hostName = hostList[index];
                String portName = portList[index];

                _handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _handle.SetKeepAliveEx(KEEP_ALIVE_INTERVAL, KEEP_ALIVE_TIME);
                _handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                _handle.NoDelay = true;

                int portNumber = getPortNumber(portName);

                _handle.Connect(hostName, portNumber);

                if (_handle.Connected)
                {
                    _handle.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, AsyncReceive, null);
                    OnClientState(true);
                }
            }
            catch (Exception ex)
            {
                OnClientFail(ex);
            }
        }

        private int getIndexFor(String[] hostList, String[] portList)
        {
            //90% chance of returning the primary, 10% chance of returning secondary, 1% chance of returning ternary
            int index = 0;
            int attempts = Math.Min(hostList.Length, portList.Length) - 1;
            while (attempts > 0 && _rnd.NextDouble() > 0.9)
            {
                index++;
                attempts--;
            }
            return index;
        }

        private int getPortNumber(string port)
        {

            // four byte IP address AA.BB.CC.DD results in 2 byte port number CCDD
            // the class A and B addresses bytes are ignored and can be set to resemble any valid IP address

            int intPort = 0;
            try
            {
                intPort = int.Parse(port);
            }
            catch (Exception)
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(port);
                foreach (IPAddress theaddress in addresslist)
                {
                    if (theaddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        byte[] bytes = theaddress.GetAddressBytes();
                        Array.Reverse(bytes);
                        bytes[2] = 0;
                        bytes[3] = 0;
                        intPort = BitConverter.ToInt32(bytes, 0);
                    }
                }
            }
            return intPort;
        }

        private void Initialize()
        {
            AddTypeToSerializer(typeof (IPacket), typeof (UnknownPacket));
            lock (_proxyClientsLock)
            {
                _proxyClients = new List<ReverseProxyClient>();
            }
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

            _readableDataLen += bytesTransferred;
            bool process = true;

            while (process)
            {
                switch (_receiveState)
                {
                    case ReceiveType.Header:
                    {
                        process = _readableDataLen >= HEADER_SIZE;
                        if (process)
                        {
                            _payloadLen = BitConverter.ToInt32(_buffer, _readOffset);

                            _readableDataLen -= HEADER_SIZE;
                            _readOffset += HEADER_SIZE;
                            _receiveState = ReceiveType.Payload;
                        }
                        break;
                    }
                    case ReceiveType.Payload:
                    {
                        process = _readableDataLen >= _payloadLen;
                        if (process)
                        {
                            byte[] payload = new byte[_payloadLen];
                            try
                            {
                                Array.Copy(this._buffer, _readOffset, payload, 0, payload.Length);
                            }
                            catch
                            {
                            }

                            if (encryptionEnabled)
                                payload = AES.Decrypt(payload, Encoding.UTF8.GetBytes(Settings.PASSWORD));

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
                        break;
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
            catch (Exception ex)
            {
                OnClientFail(ex);
            }
        }

        public void Send<T>(T packet) where T : IPacket
        {
            lock (_handle)
            {
                if (!Connected)
                    return;

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Serializer.SerializeWithLengthPrefix<T>(ms, packet, PrefixStyle.Fixed32);

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
                data = AES.Encrypt(data, Encoding.UTF8.GetBytes(Settings.PASSWORD));

            byte[] temp = BitConverter.GetBytes(data.Length);

            byte[] payload = new byte[data.Length + 4];
            Array.Copy(temp, payload, temp.Length);
            Array.Copy(data, 0, payload, 4, data.Length);

            try
            {
                _handle.Send(payload);
            }
            catch (Exception ex)
            {
                OnClientFail(ex);
            }
        }

        /// <summary>
        /// Disconnect the client from the server, disconnect all proxies that
        /// are held by this client, and dispose of other resources associated
        /// with this client.
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

                if(_proxyClients != null)
                {
                    lock (_proxyClientsLock)
                    {
                        foreach (ReverseProxyClient proxy in _proxyClients)
                            proxy.Disconnect();
                    }
                }
                Commands.CommandHandler.StreamCodec = null;
            }
        }

        /// <summary>
        /// Adds a Type to the serializer so a message can be properly serialized.
        /// </summary>
        /// <param name="parent">The parent type.</param>
        /// <param name="type">Type to be added.</param>
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