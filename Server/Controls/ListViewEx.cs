using System.Windows.Forms;

namespace xServer.Controls
{
    public partial class ListViewEx : ListView
    {
        public ListViewEx()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
        }
    }
}