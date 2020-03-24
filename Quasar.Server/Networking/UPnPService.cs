using Mono.Nat;
using System;
using System.Collections.Generic;

namespace Quasar.Server.Networking
{
    public class UPnPService
    {
        /// <summary>
        /// Used to keep track of all created mappings.
        /// </summary>
        private readonly Dictionary<int, Mapping> _mappings = new Dictionary<int, Mapping>();

        /// <summary>
        /// The discovered UPnP device.
        /// </summary>
        private INatDevice _device;

        /// <summary>
        /// The port used to create an mapping once a UPnP device was found.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Initializes the discovery of new UPnP devices and creates a port mapping using the given port
        /// once a suitable device was found.
        /// </summary>
        /// <param name="port">The port to map.</param>
        public UPnPService(int port) : this()
        {
            _port = port;
        }

        /// <summary>
        /// Initializes the discovery of new UPnP devices.
        /// </summary>
        public UPnPService()
        {
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.DeviceLost += DeviceLost;
            NatUtility.StartDiscovery();
        }

        /// <summary>
        /// States if an UPnP device was found.
        /// </summary>
        public bool DiscoveryCompleted => _device != null;

        /// <summary>
        /// Creates a new port mapping on the UPnP device.
        /// </summary>
        /// <param name="port">The port to map.</param>
        public void CreatePortMap(int port)
        {
            if (!DiscoveryCompleted)
                return;

            try
            {
                Mapping mapping = new Mapping(Protocol.Tcp, port, port);

                _device.BeginCreatePortMap(mapping, EndCreateAsync, mapping);
            }
            catch (MappingException)
            {
            }
        }

        private void EndCreateAsync(IAsyncResult ar)
        {
            var mapping = (Mapping) ar.AsyncState;

            _device?.EndCreatePortMap(ar);

            if (_mappings.ContainsKey(mapping.PrivatePort))
                _mappings[mapping.PrivatePort] = mapping;
            else
                _mappings.Add(mapping.PrivatePort, mapping);
        }

        private void EndDeleteAsync(IAsyncResult ar)
        {
            var mapping = (Mapping)ar.AsyncState;

            _device?.EndDeletePortMap(ar);

            _mappings.Remove(mapping.PrivatePort);
        }

        /// <summary>
        /// Deletes an existing port map.
        /// </summary>
        /// <param name="port">The port to delete.</param>
        public void DeletePortMap(int port)
        {
            if (!DiscoveryCompleted)
                return;

            if (_mappings.TryGetValue(port, out var mapping))
            {
                try
                {
                    _device.BeginDeletePortMap(mapping, EndDeleteAsync, mapping);
                }
                catch (MappingException)
                {
                }
            }
        }

        private void DeviceFound(object sender, DeviceEventArgs args)
        {
            _device = args.Device;
            NatUtility.StopDiscovery();

            if (_port > 0)
                CreatePortMap(_port);
        }

        private void DeviceLost(object sender, DeviceEventArgs args)
        {
            _device = null;
        }
    }
}
