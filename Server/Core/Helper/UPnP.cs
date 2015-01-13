using System.Net;
using System.Net.Sockets;
using System.Threading;
using NATUPNPLib;

namespace xServer.Core.Helper
{
    static class UPnP
    {
        public static void ForwardPort(ushort port)
        {
            new Thread(() =>
            {
                EndPoint endPoint;
                string ipAddr = "";
                int i = 0;

            Retry:
                try
                {
                    TcpClient c = new TcpClient();
                    c.Connect("www.google.com", 80);
                    endPoint = c.Client.LocalEndPoint;
                    c.Close();

                    if (endPoint != null)
                    {
                        ipAddr = endPoint.ToString();
                        int index = ipAddr.IndexOf(":");
                        ipAddr = ipAddr.Remove(index);
                    }
                }
                catch { i++; if (i < 5) goto Retry; }

                try
                {
                    IStaticPortMappingCollection portMap = new UPnPNAT().StaticPortMappingCollection;
                    if (portMap != null)
                        portMap.Add(port, "TCP", port, ipAddr, true, "xRAT 2.0 UPnP");
                }
                catch
                { }
            }).Start();
        }

        public static void RemovePort(ushort port)
        {
            try
            {
                IStaticPortMappingCollection portMap = new UPnPNAT().StaticPortMappingCollection;
                if (portMap != null)
                    portMap.Remove(port, "TCP");
            }
            catch
            { }
        }
    }
}
