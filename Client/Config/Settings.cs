using System;

#if !DEBUG
using xClient.Core.Encryption;
#endif

namespace xClient.Config
{
    public static class Settings
    {
#if DEBUG
        public static string VERSION = "1.0.0.0d";
        public static string HOSTS = "localhost:4782;";
        public static int RECONNECTDELAY = 500;
        public static string PASSWORD = "1234";
        public static string DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string SUBFOLDER = "Test";
        public static string INSTALLNAME = "test.exe";
        public static bool INSTALL = false;
        public static bool STARTUP = false;
        public static string MUTEX = "123AKs82kA,ylAo2kAlUS2kYkala!";
        public static string STARTUPKEY = "Test key";
        public static bool HIDEFILE = false;
        public static bool ENABLELOGGER = true;
        public static string TAG = "DEBUG";

        public static void Initialize()
        {
        }
#else
        public static string VERSION = "1.0.0.0r";
        public static string HOSTS = "localhost:4782;";
        public static int RECONNECTDELAY = 5000;
        public static string PASSWORD = "1234";
        public static string DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string SUBFOLDER = "SUB";
        public static string INSTALLNAME = "INSTALL";
        public static bool INSTALL = false;
        public static bool STARTUP = true;
        public static string MUTEX = "MUTEX";
        public static string STARTUPKEY = "STARTUP";
        public static bool HIDEFILE = true;
        public static bool ENABLELOGGER = true;
        public static string ENCRYPTIONKEY = "ENCKEY";
        public static string TAG = "RELEASE";

        public static void Initialize()
        {
            AES.PreHashKey(ENCRYPTIONKEY);
            TAG = AES.Decrypt(TAG);
            VERSION = AES.Decrypt(VERSION);
            HOSTS = AES.Decrypt(HOSTS);
            PASSWORD = AES.Decrypt(PASSWORD);
            SUBFOLDER = AES.Decrypt(SUBFOLDER);
            INSTALLNAME = AES.Decrypt(INSTALLNAME);
            MUTEX = AES.Decrypt(MUTEX);
            STARTUPKEY = AES.Decrypt(STARTUPKEY);
        }
#endif
    }
}