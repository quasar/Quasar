using System;

namespace xClient.Enums
{
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