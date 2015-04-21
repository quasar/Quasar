using System.Net;
using System.Net.Sockets;
using System.Threading;
using NATUPNPLib;

namespace xServer.Core.Helper
{
    internal static class UPnP
    {
        public static void ForwardPort(ushort port)
        {
            new Thread(() =>
            {
                EndPoint endPoint;
                string ipAddr = string.Empty;
                int retry = 0;

                do
                {
                    try
                    {
                        TcpClient c = null;
                        try
                        {
                            c = new TcpClient();
                            c.Connect("www.google.com", 80);
                            endPoint = c.Client.LocalEndPoint;
                        }
                        finally
                        {
                            // Placed in here to make sure that a failed TcpClient will never linger!
                            if (c != null)
                            {
                                c.Close();
                            }
                        }

                        if (endPoint != null)
                        {
                            ipAddr = endPoint.ToString();
                            int index = ipAddr.IndexOf(":");
                            ipAddr = ipAddr.Remove(index);

                            // We got through successfully and with an endpoint and a parsed IP address. We may exit the loop.
                            break;
                        }
                        else
                        {
                            retry++;
                        }
                    }
                    catch
                    {
                        retry++;
                    }
                } while (retry < 5);

                if (string.IsNullOrEmpty(ipAddr)) // If we can't successfully connect
                    return;

                try
                {
                    IStaticPortMappingCollection portMap = new UPnPNAT().StaticPortMappingCollection;
                    if (portMap != null)
                        portMap.Add(port, "TCP", port, ipAddr, true, "xRAT 2.0 UPnP");
                }
                catch
                {
                }
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
            {
            }
        }
    }
}