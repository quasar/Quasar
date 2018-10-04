using System.Windows.Forms;

namespace Quasar.Client.Data
{
    public static class ClientData
    {
        public static string CurrentPath { get; set; }
        public static string InstallPath { get; set; }
        public static bool AddToStartupFailed { get; set; }

        static ClientData()
        {
            CurrentPath = Application.ExecutablePath;
        }
    }
}
