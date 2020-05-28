using System.Net;
using System.Windows.Forms;

namespace Quasar.Client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // enable TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var app = new QuasarApplication())
            {
                app.Run();
            }
        }
    }
}
