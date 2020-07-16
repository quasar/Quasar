using Quasar.Client.IO;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // enable TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling UI thread exceptions
            Application.ThreadException += HandleThreadException;

            // Add the event handler for handling non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new QuasarApplication());
        }

        private static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.WriteLine(e);
            try
            {
                string batchFile = BatchFile.CreateRestartBatch(Application.ExecutablePath);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Handles unhandled exceptions by restarting the application and hoping that they don't happen again.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">The exception event arguments. </param>
        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Debug.WriteLine(e);
                try
                {
                    string batchFile = BatchFile.CreateRestartBatch(Application.ExecutablePath);

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = true,
                        FileName = batchFile
                    };
                    Process.Start(startInfo);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
                finally
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
