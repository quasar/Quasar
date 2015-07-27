using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Core.Extensions
{
    public static class ListViewExtensions
    {
        private const uint SET_COLUMN_WIDTH = 4126;
        private const int AUTOSIZE_USEHEADER = -2;

        public static void AutosizeColumns(this ListView targetListView)
        {
            if (PlatformHelper.RunningOnMono) return;
            for (int lngColumn = 0; lngColumn <= (targetListView.Columns.Count - 1); lngColumn++)
            {
                NativeMethods.SendMessage(targetListView.Handle, SET_COLUMN_WIDTH, lngColumn, AUTOSIZE_USEHEADER);
            }
        }
    }
}