using Quasar.Server.Forms;
using System;
using System.Net;
using System.Windows.Forms;

namespace Quasar.Server
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // enable TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
