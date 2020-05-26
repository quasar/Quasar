using System;
using System.Threading;

namespace Quasar.Client.Utilities
{
    public class SingleInstanceMutex : IDisposable
    {
        private readonly Mutex _appMutex;

        public bool CreatedNew { get; }

        public bool IsDisposed { get; private set; }

        public SingleInstanceMutex(string name)
        {
            _appMutex = new Mutex(false, name, out var createdNew);
            CreatedNew = createdNew;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _appMutex?.Dispose();
                IsDisposed = true;
            }
        }
    }
}
