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
        private static int _port;
        private static int _addPortTries;
        private static int _removePortTries;

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
        public static bool CreatePortMap(int port)
        {
            if (!_discoveryComplete)
            {
                return false;
            }

            _addPortTries++;
            try
            {
                Mapping mapping = new Mapping(Protocol.Tcp, port, port);
                _device.CreatePortMap(mapping); //The first call to CreatePortMap can fail to map the port and doesn't throw an exception.

                Mapping[] maps = _device.GetAllMappings(); //Check to see if map was succesfully ported. If it wasn't maps.Length will be 0.

                if (maps.Length < 1 && _addPortTries < 8)
                    CreatePortMap(port); //Port wasn't mapped succesfully, retry until port is mapped succesfully.

                if (_addPortTries >= 7) //Port could not be mapped, return false.
                    return false;

                _addPortTries = 0;

                if (_mappings.ContainsKey(mapping.PrivatePort))
                    _mappings[mapping.PrivatePort] = mapping;
                else
                    _mappings.Add(mapping.PrivatePort, mapping);

                return true;
            }
            catch (MappingException)
            {
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

            _removePortTries++;

            Mapping mapping;
            if (_mappings.TryGetValue(port, out mapping))
            {
                try
                {
                    _device.DeletePortMap(mapping); //Removing the port can fail the first attempt at removal
                    Mapping[] maps = _device.GetAllMappings(); //If port wasn't removed the first attempt, it will still be mapped

                    if (maps.Length > 0 && _removePortTries < 8) //Attempt to remove port a few times until it is succesfully removed
                    {
                        _removePortTries++;
                        DeletePortMap(port);
                    }
                    else
                        _mappings.Remove(mapping.PrivatePort); //Remove map from dictionary

                    _removePortTries = 0;
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
                CreatePortMap(_port);
            }
        }

        private static void DeviceLost(object sender, DeviceEventArgs args)
        {
            _device = null;
            _discoveryComplete = false;
        }
    }
}