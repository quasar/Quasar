using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE REGISTRY. */
    public static partial class CommandHandler
    {
        public static void HandleLoadRegistryKey(xServer.Core.Packets.ClientPackets.GetRegistryKeysResponse packet, Client client)
        {
            try
            {
                // Make sure that we can use the packet.
                if (packet.Matches != null && packet.Matches.Length > 0)
                {
                    // Make sure that the client is in the correct state to handle the packet appropriately.
                    if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                    {
                        client.Value.FrmRe.AddKeysToTree(packet.RootKey, packet.Matches);
                    }
                }
            }
            catch
            { }
        }

        #region Registry Key Edit

        public static void HandleCreateRegistryKey(xServer.Core.Packets.ClientPackets.GetCreateRegistryKeyResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.AddKeyToTree(packet.ParentPath, packet.Match);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        public static void HandleDeleteRegistryKey(xServer.Core.Packets.ClientPackets.GetDeleteRegistryKeyResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.RemoveKeyFromTree(packet.ParentPath, packet.KeyName);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        public static void HandleRenameRegistryKey(xServer.Core.Packets.ClientPackets.GetRenameRegistryKeyResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.RenameKeyFromTree(packet.ParentPath, packet.OldKeyName, packet.NewKeyName);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        #endregion

        #region Registry Value Edit

        public static void HandleCreateRegistryValue(xServer.Core.Packets.ClientPackets.GetCreateRegistryValueResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.AddValueToList(packet.KeyPath, packet.Value);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        public static void HandleDeleteRegistryValue(xServer.Core.Packets.ClientPackets.GetDeleteRegistryValueResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.DeleteValueFromList(packet.KeyPath, packet.ValueName);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        public static void HandleRenameRegistryValue(xServer.Core.Packets.ClientPackets.GetRenameRegistryValueResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.RenameValueFromList(packet.KeyPath, packet.OldValueName, packet.NewValueName);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        public static void HandleChangeRegistryValue(xServer.Core.Packets.ClientPackets.GetChangeRegistryValueResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.ChangeValueFromList(packet.KeyPath, packet.Value);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
