using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Enums
{
    /*
     * Derived and Adapted By Justin Yanke
     * github: https://github.com/yankejustin
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * This code is created by Justin Yanke.
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * No modifications made
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Original Source:
     * https://github.com/quasar/QuasarRAT/blob/regedit/Client/Enums/RegistrySearchAction.cs
     */

    /// <summary>
    /// Specifies the items to retrieve and send when searching the registry.
    /// </summary>
    [Flags]
    public enum RegistrySearchAction
    {
        Keys,
        Values,
        Data
    }
}
