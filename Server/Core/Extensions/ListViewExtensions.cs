using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace xServer.Core.Extensions
{
    public static class ListViewExtensions
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, int pszSubIdList);

        private const uint WM_CHANGEUISTATE = 0x127;
        private const uint SET_COLUMN_WIDTH = 4126;
        private const int AUTOSIZE_USEHEADER = -2;

        public static void RemoveDots(ListView targetListView)
        {
            SendMessage(targetListView.Handle, WM_CHANGEUISTATE, 65537, 0);
        }

        public static void ChangeTheme(ListView targetListView)
        {
            SetWindowTheme(targetListView.Handle, "Explorer", 0);
        }

        public static void AutosizeColumns(ListView targetListView)
        {
            for (int lngColumn = 0; lngColumn <= (targetListView.Columns.Count - 1); lngColumn++)
            {
                SendMessage(targetListView.Handle, SET_COLUMN_WIDTH, lngColumn, AUTOSIZE_USEHEADER);
            }
        }
    }
}