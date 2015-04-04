using System;
using System.Windows.Forms;

namespace xClient.Core.Elevation
{
    public class CommandButton : Button
    {
        public CommandButton()
        {
            FlatStyle = FlatStyle.System;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cParams = base.CreateParams;
                if (Environment.OSVersion.Version.Major >= 6)
                    cParams.Style |= 14;
                return cParams;
            }
        }
    }
}