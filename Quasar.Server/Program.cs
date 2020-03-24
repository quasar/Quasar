using System;
using System.Windows.Forms;
using Quasar.Server.Forms;

namespace Quasar.Server
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
