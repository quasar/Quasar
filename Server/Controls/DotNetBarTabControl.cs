using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// thanks to Mavamaarten~ for coding this

namespace xServer.Controls
{
    internal class DotNetBarTabControl : TabControl
    {
        public DotNetBarTabControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer, true);
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(44, 136);
            Alignment = TabAlignment.Left;
            SelectedIndex = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap b = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(b);
            if (!DesignMode)
                SelectedTab.BackColor = SystemColors.Control;
            g.Clear(SystemColors.Control);
            g.FillRectangle(new SolidBrush(Color.FromArgb(246, 248, 252)),
                new Rectangle(0, 0, ItemSize.Height + 4, Height));
            g.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(ItemSize.Height + 3, 0),
                new Point(ItemSize.Height + 3, 999));
            g.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(0, Size.Height - 1),
                new Point(Width + 3, Size.Height - 1));
            for (int i = 0; i <= TabCount - 1; i++)
            {
                if (i == SelectedIndex)
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2),
                        new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                    ColorBlend myBlend = new ColorBlend();
                    myBlend.Colors = new Color[] { Color.FromArgb(232, 232, 240), Color.FromArgb(232, 232, 240), Color.FromArgb(232, 232, 240) };
                    myBlend.Positions = new float[] { 0f, 0.5f, 1f };
                    LinearGradientBrush lgBrush = new LinearGradientBrush(x2, Color.Black, Color.Black, 90f);
                    lgBrush.InterpolationColors = myBlend;
                    g.FillRectangle(lgBrush, x2);
                    g.DrawRectangle(new Pen(Color.FromArgb(170, 187, 204)), x2);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    Point[] p =
                    {
                        new Point(ItemSize.Height - 3, GetTabRect(i).Location.Y + 20),
                        new Point(ItemSize.Height + 4, GetTabRect(i).Location.Y + 14),
                        new Point(ItemSize.Height + 4, GetTabRect(i).Location.Y + 27)
                    };
                    g.FillPolygon(SystemBrushes.Control, p);
                    g.DrawPolygon(new Pen(Color.FromArgb(170, 187, 204)), p);

                    if (ImageList != null)
                    {
                        try
                        {
                            g.DrawImage(ImageList.Images[TabPages[i].ImageIndex],
                                new Point(x2.Location.X + 8, x2.Location.Y + 6));
                            g.DrawString("  " + TabPages[i].Text, Font, Brushes.Black, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                        }
                        catch (Exception)
                        {
                            g.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                                Brushes.Black, x2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                        }
                    }
                    else
                    {
                        g.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold),
                            Brushes.Black, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                    }

                    g.DrawLine(new Pen(Color.FromArgb(200, 200, 250)), new Point(x2.Location.X - 1, x2.Location.Y - 1),
                        new Point(x2.Location.X, x2.Location.Y));
                    g.DrawLine(new Pen(Color.FromArgb(200, 200, 250)), new Point(x2.Location.X - 1, x2.Bottom - 1),
                        new Point(x2.Location.X, x2.Bottom));
                }
                else
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2),
                        new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                    g.FillRectangle(new SolidBrush(Color.FromArgb(246, 248, 252)), x2);
                    g.DrawLine(new Pen(Color.FromArgb(170, 187, 204)), new Point(x2.Right, x2.Top),
                        new Point(x2.Right, x2.Bottom));
                    if (ImageList != null)
                    {
                        try
                        {
                            g.DrawImage(ImageList.Images[TabPages[i].ImageIndex],
                                new Point(x2.Location.X + 8, x2.Location.Y + 6));
                            g.DrawString("  " + TabPages[i].Text, Font, Brushes.DimGray, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                        }
                        catch (Exception)
                        {
                            g.DrawString(TabPages[i].Text, Font, Brushes.DimGray, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                        }
                    }
                    else
                    {
                        g.DrawString(TabPages[i].Text, Font, Brushes.DimGray, x2, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        });
                    }
                }
            }

            e.Graphics.DrawImage(b, new Point(0, 0));
            g.Dispose();
            b.Dispose();
        }
    }
}