using System.Drawing;
using System.Windows.Forms;

namespace xServer.Controls
{
    public class Line : Control
    {
        public enum Alignment
        {
            Horizontal,
            Vertical
        }

        public Alignment LineAlignment { get; set; }

        public Line()
        {
            this.TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.LightGray)), new Point(5, 5),
                LineAlignment == Alignment.Horizontal ? new Point(500, 5) : new Point(5, 500));
        }
    }
}
