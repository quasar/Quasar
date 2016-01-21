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
        public bool GatherKeyValues { get; private set; }

        public RegistryKey RootKey { get; set; }

        public RegistrySeekerParams(RegistryKey registryKey, RegistrySearchAction keyAnalyzeDepth)
        {
            this.RootKey = registryKey;
            this.searchValueTypes = keyAnalyzeDepth;
        }

        private RegistrySearchAction searchValueTypes
        {
            get
            {
                RegistrySearchAction action = RegistrySearchAction.Keys;

                if (GatherKeyValues)
                    action |= RegistrySearchAction.Values;

                return action;
            }
            set
            {
                GatherKeyValues = (value & RegistrySearchAction.Values) == RegistrySearchAction.Values;
            }
        }
    }
}
