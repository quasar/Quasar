using System.Windows.Forms;

namespace xServer.Controls
{
    public class ListViewEx : ListView
    {
        public ListViewEx()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }
    }
}