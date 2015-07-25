using System.Collections.Generic;
using System.Threading;
using Mono.Nat;

namespace xServer.Core.Helper
{
    internal static class UPnP
    {
        public static bool IsPortForwarded { get; private set; }
        public static ushort Port { get; private set; }
        private static readonly HashSet<INatDevice> Devices = new HashSet<INatDevice>();
        private static bool _eventSub;
        private static bool _isDiscovering;
        private static readonly object _isDiscoveringLock = new object();

        public static void ForwardPort(ushort port)
        {
            lock (_isDiscoveringLock)
            {
                if (_isDiscovering) return;
                _isDiscovering = true;
            }

            Port = port;

            if (!_eventSub)
            {
                NatUtility.DeviceFound += DeviceFound;
                NatUtility.DeviceLost += DeviceLost;
                _eventSub = true;
            }

            new Thread(() =>
            {
                NatUtility.StartDiscovery();

                int trys = 0;
                while (Devices.Count == 0 && trys < 8) // wait until first device found
                {
                    trys++;
                    Thread.Sleep(1000);
                }

                if (Devices.Count > 0)
                {
                    try
                    {
                        foreach (var device in Devices)
                        {
                            if (device.GetSpecificMapping(Protocol.Tcp, Port).PublicPort < 0) // if port is not mapped
                            {
                                device.CreatePortMap(new Mapping(Protocol.Tcp, Port, Port));
                                IsPortForwarded = true;
                            }
                        }
                    }
                    catch (MappingException)
                    {
                        IsPortForwarded = false;
                    }
                }

                NatUtility.StopDiscovery();

                lock (_isDiscoveringLock)
                {
                    _isDiscovering = false;
                }
            }).Start();
        }

        private static void DeviceFound(object sender, DeviceEventArgs args)
        {
            Devices.Add(args.Device);
        }

        private static void DeviceLost(object sender, DeviceEventArgs args)
        {
            Devices.Remove(args.Device);
        }

        public static void RemovePort()
        {
            foreach (var device in Devices)
            {
                if (device.GetSpecificMapping(Protocol.Tcp, Port).PublicPort > 0) // if port map exists
                {
                    device.DeletePortMap(new Mapping(Protocol.Tcp, Port, Port));
                    IsPortForwarded = false;
                }
            }
        }
    }
}