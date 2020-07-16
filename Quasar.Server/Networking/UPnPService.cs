using Open.Nat;
using System;
using System.Collections.Generic;
using System.Threading;

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
        private NatDevice _device;

        /// <summary>
        /// The NAT discoverer used to discover NAT-UPnP devices.
        /// </summary>
        private NatDiscoverer _discoverer;

        /// <summary>
        /// Initializes the discovery of new UPnP devices.
        /// </summary>
        public UPnPService()
        {
            _discoverer = new NatDiscoverer();
        }

        /// <summary>
        /// Creates a new port mapping on the UPnP device.
        /// </summary>
        /// <param name="port">The port to map.</param>
        public async void CreatePortMapAsync(int port)
        {
            try
            {
                var cts = new CancellationTokenSource(10000);
                _device = await _discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                Mapping mapping = new Mapping(Protocol.Tcp, port, port);

                await _device.CreatePortMapAsync(mapping);

                if (_mappings.ContainsKey(mapping.PrivatePort))
                    _mappings[mapping.PrivatePort] = mapping;
                else
                    _mappings.Add(mapping.PrivatePort, mapping);
            }
            catch (Exception ex) when (ex is MappingException || ex is NatDeviceNotFoundException)
            {
            }
        }

        /// <summary>
        /// Deletes an existing port mapping.
        /// </summary>
        /// <param name="port">The port mapping to delete.</param>
        public async void DeletePortMapAsync(int port)
        {
            if (_mappings.TryGetValue(port, out var mapping))
            {
                try
                {
                    await _device.DeletePortMapAsync(mapping);
                    _mappings.Remove(mapping.PrivatePort);
                }
                catch (Exception ex) when (ex is MappingException || ex is NatDeviceNotFoundException)
                {
                }
            }
        }
    }
}
