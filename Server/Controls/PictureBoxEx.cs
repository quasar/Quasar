using System.Windows.Forms;

namespace xServer.Controls
{
    internal class PictureBoxEx : PictureBox
    {
        public PictureBoxEx()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
