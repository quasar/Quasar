using System;

namespace xServer.Enums
{
    /// <summary>
    /// Specifies the items to obtain from a client when requesting
    /// to search the Client's registry.
    /// </summary>
    [Flags]
    public enum RegistrySearchAction
    {
        Keys,
        Values,
        Data
    }
}