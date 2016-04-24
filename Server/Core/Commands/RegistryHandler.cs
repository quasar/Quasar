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
        

        #region Registry Key
        
        public static void HandleLoadRegistryKey(xServer.Core.Packets.ClientPackets.GetRegistryKeysResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.AddKeys(packet.RootKey, packet.Matches);
                    }
                    else
                    {
                        client.Value.FrmRe.ShowErrorMessage(packet.ErrorMsg);
                        //If root keys failed to load then close the form
                        if (packet.RootKey == null)
                        {
                            //Invoke a closing of the form
                            client.Value.FrmRe.PerformClose();
                        }
                    }
                }
            }
            catch { }
        }

        public static void HandleCreateRegistryKey(xServer.Core.Packets.ClientPackets.GetCreateRegistryKeyResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.CreateNewKey(packet.ParentPath, packet.Match);
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
                        client.Value.FrmRe.RemoveKey(packet.ParentPath, packet.KeyName);
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
                        client.Value.FrmRe.RenameKey(packet.ParentPath, packet.OldKeyName, packet.NewKeyName);
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

        #region Registry Value

        public static void HandleCreateRegistryValue(xServer.Core.Packets.ClientPackets.GetCreateRegistryValueResponse packet, Client client)
        {
            try
            {
                // Make sure that the client is in the correct state to handle the packet appropriately.
                if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                {
                    if (!packet.IsError)
                    {
                        client.Value.FrmRe.CreateValue(packet.KeyPath, packet.Value);
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
                        client.Value.FrmRe.DeleteValue(packet.KeyPath, packet.ValueName);
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
                        client.Value.FrmRe.RenameValue(packet.KeyPath, packet.OldValueName, packet.NewValueName);
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
                        client.Value.FrmRe.ChangeValue(packet.KeyPath, packet.Value);
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
