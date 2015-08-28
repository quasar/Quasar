using System.Threading;

namespace xClient.Core.Helper
{
    public static class MutexHelper
    {
        private static Mutex _appMutex;

        public static bool CreateMutex(string name)
        {
            bool createdNew;
            _appMutex = new Mutex(false, name, out createdNew);
            return createdNew;
        }

        public static void CloseMutex()
        {
            if (_appMutex != null)
            {
                _appMutex.Close();
                _appMutex = null;
            }
        }
    }
}
