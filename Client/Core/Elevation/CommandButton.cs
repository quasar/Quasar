using System;
using System.Windows.Forms;

namespace xClient.Core.Elevation
{
    public class CommandButton : Button
    {
        public CommandButton()
        {
            this.FlatStyle = FlatStyle.System;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParams = base.CreateParams;
                if (Environment.OSVersion.Version.Major >= 6)
                    cParams.Style |= 14;
                return cParams;
            }
        }
    }
}