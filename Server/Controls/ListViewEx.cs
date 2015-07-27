using System;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Utilities;

namespace xServer.Controls
{
    internal class AeroListView : ListView
    {
        private const uint WM_CHANGEUISTATE = 0x127;

        private const int UIS_SET = 1;
        private const int UISF_HIDEFOCUS = 0x1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AeroListView"/> class.
        /// </summary>
        public AeroListView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.View = View.Details;
            this.FullRowSelect = true;
        }

        /// <summary>
        /// Raises the <see cref="E:HandleCreated" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (PlatformHelper.RunningOnMono) return;

            if (PlatformHelper.VistaOrHigher)
            {
                // set window theme to explorer
                NativeMethods.SetWindowTheme(this.Handle, "explorer", null);
            }

            if (PlatformHelper.XpOrHigher)
            {
                // removes the ugly dotted line around focused item
                NativeMethods.SendMessage(this.Handle, WM_CHANGEUISTATE,
                    NativeMethodsHelper.MakeLong(UIS_SET, UISF_HIDEFOCUS), 0);
            }
        }
    }
}