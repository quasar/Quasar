using System;
using System.Threading;

namespace Quasar.Client.Utilities
{
    /// <summary>
    /// A user-wide mutex that ensures that only one instance runs at a time.
    /// </summary>
    public class SingleInstanceMutex : IDisposable
    {
        /// <summary>
        /// The mutex used for process synchronization.
        /// </summary>
        private readonly Mutex _appMutex;

        /// <summary>
        /// Represents if the mutex was created on the system or it already existed.
        /// </summary>
        public bool CreatedNew { get; }

        /// <summary>
        /// Determines if the instance is disposed and should not be used anymore.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SingleInstanceMutex"/> using the given mutex name.
        /// </summary>
        /// <param name="name">The name of the mutex.</param>
        public SingleInstanceMutex(string name)
        {
            _appMutex = new Mutex(false, $"Local\\{name}", out var createdNew);
            CreatedNew = createdNew;
        }

        /// <summary>
        /// Releases all resources used by this <see cref="SingleInstanceMutex"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the mutex object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called from <see cref="Dispose"/>, <c>false</c> if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                _appMutex?.Dispose();
            }

            IsDisposed = true;
        }
    }
}
