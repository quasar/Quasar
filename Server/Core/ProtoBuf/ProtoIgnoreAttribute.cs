using System;

namespace ProtoBuf
{
    /// <summary>
    /// Indicates that a member should be excluded from serialization; this
    /// is only normally used when using implict fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true)]
    public class ProtoIgnoreAttribute : Attribute
    {
    }
}