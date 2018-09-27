using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Core.Extensions
{
    public static class ListViewExtensions
    {
        private const uint SET_COLUMN_WIDTH = 4126;
        private const int AUTOSIZE_USEHEADER = -2;

        /// <summary>
        /// Automatically determines the correct column size on the the given listview.
        /// </summary>
        /// <param name="targetListView">The listview whose columns are to be autosized.</param>
        public static void AutosizeColumns(this ListView targetListView)
        {
            if (PlatformHelper.RunningOnMono) return;
            for (int lngColumn = 0; lngColumn <= (targetListView.Columns.Count - 1); lngColumn++)
            {
                NativeMethods.SendMessage(targetListView.Handle, SET_COLUMN_WIDTH, lngColumn, AUTOSIZE_USEHEADER);
            }
        }

        /// <summary>
        /// Selects all items on the given listview.
        /// </summary>
        /// <param name="targetListView">The listview whose items are to be selected.</param>
        public static void SelectAllItems(this ListView targetListView)
        {
            NativeMethodsHelper.SetItemState(targetListView.Handle, -1, 2, 2);
        }
    }
}