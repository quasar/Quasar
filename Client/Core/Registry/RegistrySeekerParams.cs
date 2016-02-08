using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Enums;

namespace xClient.Core.Registry
{
    /*
     * Derived and Adapted By Justin Yanke
     * github: https://github.com/yankejustin
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * This code is created by Justin Yanke and has only been
     * modified partially.
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Modified by StingRaptor on January 21, 2016
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Original Source:
     * https://github.com/quasar/QuasarRAT/blob/regedit/Client/Core/Registry/RegistrySeekerParams.cs
     */

    public class RegistrySeekerParams
    {
        public RegistryKey RootKey { get; set; }

        public RegistrySeekerParams(RegistryKey registryKey)
        {
            this.RootKey = registryKey;
        }
    }
}
