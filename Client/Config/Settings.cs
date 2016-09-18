using System;
using xClient.Core.Helper;

#if !DEBUG
using xClient.Core.Cryptography;
#endif

namespace xClient.Config
{
    public static class Settings
    {
#if DEBUG
        public static string VERSION = System.Windows.Forms.Application.ProductVersion;
        public static string HOSTS = "localhost:4782;";
        public static int RECONNECTDELAY = 500;
        public static string KEY = "1WvgEMPjdwfqIMeM9MclyQ==";
        public static string AUTHKEY = "NcFtjbDOcsw7Evd3coMC0y4koy/SRZGydhNmno81ZOWOvdfg7sv0Cj5ad2ROUfX4QMscAIjYJdjrrs41+qcQwg==";
        public static Environment.SpecialFolder SPECIALFOLDER = Environment.SpecialFolder.ApplicationData;
        public static string DIRECTORY = Environment.GetFolderPath(SPECIALFOLDER);
        public static string SUBDIRECTORY = "Test";
        public static string INSTALLNAME = "test.exe";
        public static bool INSTALL = false;
        public static bool STARTUP = false;
        public static string MUTEX = "123AKs82kA,ylAo2kAlUS2kYkala!";
        public static string STARTUPKEY = "Test key";
        public static bool HIDEFILE = false;
        public static bool ENABLELOGGER = false;
        public static string TAG = "DEBUG";
        public static string LOGDIRECTORYNAME = "Logs";
        public static bool HIDELOGDIRECTORY = false;
        public static bool HIDEINSTALLSUBDIRECTORY = false;

        public static bool Initialize()
        {
            FixDirectory();
            return true;
        }
#else
        public static string VERSION = "";
        public static string HOSTS = "";
        public static int RECONNECTDELAY = 5000;
        public static string KEY = "";
        public static string AUTHKEY = "";
        public static Environment.SpecialFolder SPECIALFOLDER = Environment.SpecialFolder.ApplicationData;
        public static string DIRECTORY = Environment.GetFolderPath(SPECIALFOLDER);
        public static string SUBDIRECTORY = "";
        public static string INSTALLNAME = "";
        public static bool INSTALL = false;
        public static bool STARTUP = false;
        public static string MUTEX = "";
        public static string STARTUPKEY = "";
        public static bool HIDEFILE = false;
        public static bool ENABLELOGGER = false;
        public static string ENCRYPTIONKEY = "";
        public static string TAG = "";
        public static string LOGDIRECTORYNAME = "";
        public static bool HIDELOGDIRECTORY = false;
        public static bool HIDEINSTALLSUBDIRECTORY = false;

        public static bool Initialize()
        {
            if (string.IsNullOrEmpty(VERSION)) return false;
            AES.SetDefaultKey(ENCRYPTIONKEY);
            TAG = AES.Decrypt(TAG);
            VERSION = AES.Decrypt(VERSION);
            HOSTS = AES.Decrypt(HOSTS);
            SUBDIRECTORY = AES.Decrypt(SUBDIRECTORY);
            INSTALLNAME = AES.Decrypt(INSTALLNAME);
            MUTEX = AES.Decrypt(MUTEX);
            STARTUPKEY = AES.Decrypt(STARTUPKEY);
            LOGDIRECTORYNAME = AES.Decrypt(LOGDIRECTORYNAME);
            FixDirectory();
            return true;
        }
#endif

        static void FixDirectory()
        {
            if (PlatformHelper.Is64Bit) return;

            // https://msdn.microsoft.com/en-us/library/system.environment.specialfolder(v=vs.110).aspx
            switch (SPECIALFOLDER)
            {
                case Environment.SpecialFolder.ProgramFilesX86:
                    SPECIALFOLDER = Environment.SpecialFolder.ProgramFiles;
                    break;
                case Environment.SpecialFolder.SystemX86:
                    SPECIALFOLDER = Environment.SpecialFolder.System;
                    break;
            }

            DIRECTORY = Environment.GetFolderPath(SPECIALFOLDER);
        }
    }
}