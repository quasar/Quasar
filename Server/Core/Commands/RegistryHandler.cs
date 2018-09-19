using Quasar.Common.Messages;
using Quasar.Common.Networking;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    public class RegistryHandler : MessageProcessorBase<string>
    {
        private readonly Client _client;

        public RegistryHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetRegistryKeysResponse ||
                                                             message is GetCreateRegistryKeyResponse ||
                                                             message is GetDeleteRegistryKeyResponse ||
                                                             message is GetRenameRegistryKeyResponse ||
                                                             message is GetCreateRegistryValueResponse ||
                                                             message is GetDeleteRegistryValueResponse ||
                                                             message is GetRenameRegistryValueResponse ||
                                                             message is GetChangeRegistryValueResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetRegistryKeysResponse keysResp:
                    Execute(sender, keysResp);
                    break;
                case GetCreateRegistryKeyResponse createKeysResp:
                    Execute(sender, createKeysResp);
                    break;
                case GetDeleteRegistryKeyResponse deleteKeysResp:
                    Execute(sender, deleteKeysResp);
                    break;
                case GetRenameRegistryKeyResponse renameKeysResp:
                    Execute(sender, renameKeysResp);
                    break;
                case GetCreateRegistryValueResponse createValueResp:
                    Execute(sender, createValueResp);
                    break;
                case GetDeleteRegistryValueResponse deleteValueResp:
                    Execute(sender, deleteValueResp);
                    break;
                case GetRenameRegistryValueResponse renameValueResp:
                    Execute(sender, renameValueResp);
                    break;
                case GetChangeRegistryValueResponse changeValueResp:
                    Execute(sender, changeValueResp);
                    break;
            }
        }

        private void Execute(ISender client, GetRegistryKeysResponse message)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }
}
