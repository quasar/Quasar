using System.Windows.Forms;

namespace xServer.Controls
{
    internal class ListViewEx : ListView
    {
        public ListViewEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}