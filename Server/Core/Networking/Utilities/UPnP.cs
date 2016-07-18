using System;
using System.Collections.Generic;
using Mono.Nat;

namespace xServer.Core.Networking.Utilities
{
    internal static class UPnP
    {
        private static Dictionary<int, Mapping> _mappings;
        private static bool _discoveryComplete;
        private static INatDevice _device;
        private static int _port = -1;

        /// <summary>
        /// Initializes the discovery of new UPnP devices.
        /// </summary>
        public static void Initialize()
        {
            _mappings = new Dictionary<int, Mapping>();

            try
            {
                NatUtility.DeviceFound += DeviceFound;
                NatUtility.DeviceLost += DeviceLost;

                _discoveryComplete = false;

                NatUtility.StartDiscovery();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Initializes the discovery of new UPnP devices
        /// and creates a port map with the given port.
        /// </summary>
        /// <param name="port">The port to map.</param>
        public static void Initialize(int port)
        {
            _port = port;
            Initialize();
        }

        /// <summary>
        /// Tells if the class found an UPnP device.
        /// </summary>
        public static bool IsDeviceFound
        {
            get { return _device != null; }
        }

        /// <summary>
        /// Creates a new port map.
        /// </summary>
        /// <param name="port">The port to map.</param>
        /// <param name="externalPort">The port which has been mapped, -1 if it failed.</param>
        /// <returns>True if successfull, else False.</returns>
        public static bool CreatePortMap(int port, out int externalPort)
        {
            if (!_discoveryComplete)
            {
                externalPort = -1;
                return false;
            }

            try
            {
                Mapping mapping = new Mapping(Protocol.Tcp, port, port);

                for (int i = 0; i < 3; i++)
                    _device.CreatePortMapAsync(mapping);

                if (_mappings.ContainsKey(mapping.PrivatePort))
                    _mappings[mapping.PrivatePort] = mapping;
                else
                    _mappings.Add(mapping.PrivatePort, mapping);

                externalPort = mapping.PublicPort;
                return true;
            }
            catch (MappingException)
            {
                externalPort = -1;
                return false;
            }
        }

        /// <summary>
        /// Deletes an existing port map.
        /// </summary>
        /// <param name="port">The port to delete.</param>
        public static void DeletePortMap(int port)
        {
            if (!_discoveryComplete)
                return;

            Mapping mapping;
            if (_mappings.TryGetValue(port, out mapping))
            {
                try
                {
                    for (int i = 0; i < 3; i++)
                        _device.DeletePortMapAsync(mapping);
                }
                catch (MappingException)
                {
                }
            }
        }

        private static void DeviceFound(object sender, DeviceEventArgs args)
        {
            _device = args.Device;

            NatUtility.StopDiscovery();

            _discoveryComplete = true;

            if (_port > 0)
            {
                int outPort;
                CreatePortMap(_port, out outPort);
            }
        }

        private static void DeviceLost(object sender, DeviceEventArgs args)
        {
            _device = null;
            _discoveryComplete = false;
        }
    }
}