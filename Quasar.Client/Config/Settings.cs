using System;
using System.Security.Cryptography.X509Certificates;
using Quasar.Common.Helpers;

#if !DEBUG
using Quasar.Common.Cryptography;
#endif

namespace Quasar.Client.Config
{
    public static class Settings
    {
#if DEBUG
        public static string VERSION = System.Windows.Forms.Application.ProductVersion;
        public static string HOSTS = "localhost:4782;";
        public static int RECONNECTDELAY = 500;
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
        public static string ENCRYPTIONKEY = "-.)4>[=u%5G3hY3&";
        public static string TAG = "DEBUG";
        public static string LOGDIRECTORYNAME = "Logs";
        public static string CLIENTCERTIFICATESTR = "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";
        public static X509Certificate2 CLIENTCERTIFICATE;
        public static string SERVERCERTIFICATESTR = CLIENTCERTIFICATESTR; // dummy certificate, it's not used in production
        public static X509Certificate2 SERVERCERTIFICATE;
        public static bool HIDELOGDIRECTORY = false;
        public static bool HIDEINSTALLSUBDIRECTORY = false;

        public static bool Initialize()
        {
            CLIENTCERTIFICATE = new X509Certificate2(Convert.FromBase64String(CLIENTCERTIFICATESTR));
            SERVERCERTIFICATE = new X509Certificate2(Convert.FromBase64String(SERVERCERTIFICATESTR));
            FixDirectory();
            return true;
        }
#else
        public static string VERSION = "";
        public static string HOSTS = "";
        public static int RECONNECTDELAY = 5000;
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
        public static string CLIENTCERTIFICATESTR = "";
        public static X509Certificate2 CLIENTCERTIFICATE;
        public static string SERVERCERTIFICATESTR = "";
        public static X509Certificate2 SERVERCERTIFICATE;
        public static bool HIDELOGDIRECTORY = false;
        public static bool HIDEINSTALLSUBDIRECTORY = false;

        public static bool Initialize()
        {
            if (string.IsNullOrEmpty(VERSION)) return false;
            var aes = new Aes256(ENCRYPTIONKEY);
            TAG = aes.Decrypt(TAG);
            VERSION = aes.Decrypt(VERSION);
            HOSTS = aes.Decrypt(HOSTS);
            SUBDIRECTORY = aes.Decrypt(SUBDIRECTORY);
            INSTALLNAME = aes.Decrypt(INSTALLNAME);
            MUTEX = aes.Decrypt(MUTEX);
            STARTUPKEY = aes.Decrypt(STARTUPKEY);
            LOGDIRECTORYNAME = aes.Decrypt(LOGDIRECTORYNAME);
            CLIENTCERTIFICATE = new X509Certificate2(Convert.FromBase64String(aes.Decrypt(CLIENTCERTIFICATESTR)));
            SERVERCERTIFICATE = new X509Certificate2(Convert.FromBase64String(aes.Decrypt(SERVERCERTIFICATESTR)));
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