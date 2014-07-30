using NATUPNPLib;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Core
{
    class UPnP
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
                    UPnPNAT upnpNat = new UPnPNAT();
                    IStaticPortMappingCollection portMap = upnpNat.StaticPortMappingCollection;
                    portMap.Add(port, "TCP", port, ipAddr, true, "xRAT 2.0.0.0 UPnP");
                }
                catch { return; }
            }).Start();
        }
    }
}
