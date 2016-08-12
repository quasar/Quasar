using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls
{
    public class RegistryTreeView : TreeView
    {
        public RegistryTreeView()
        {
            //Enable double buffering and ignore WM_ERASEBKGND to reduce flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}
