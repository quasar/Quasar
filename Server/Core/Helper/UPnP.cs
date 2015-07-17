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

        public static void ForwardPort(ushort port)
        {
            Port = port;
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.DeviceLost += DeviceLost;
            NatUtility.StartDiscovery();

            new Thread(() =>
            {
                while (Devices.Count == 0) // wait until first device found
                {
                    Thread.Sleep(1000);
                }

                try
                {
                    foreach (var device in Devices)
                    {
                        device.CreatePortMap(new Mapping(Protocol.Tcp, Port, Port));
                    }
                    IsPortForwarded = true;
                }
                catch (MappingException)
                {
                    IsPortForwarded = false;
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
                }
            }
            IsPortForwarded = false;
            NatUtility.StopDiscovery();
        }
    }
}