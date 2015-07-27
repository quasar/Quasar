using xServer.Core.Networking;

namespace xServer.Core.Helper
{
    public static class WindowHelper
    {
        public static string GetWindowTitle(string title, Client c)
        {
            return string.Format("{0} - {1}@{2} [{3}:{4}]", title, c.Value.Username, c.Value.PCName, c.EndPoint.Address.ToString(), c.EndPoint.Port.ToString());
        }

        public static string GetWindowTitle(string title, int count)
        {
            return string.Format("{0} [Selected: {1}]", title, count);
        }
    }
}
