namespace ProtoBuf.Meta
{
    /// <summary>
    /// Delegate type used to perform type-formatting functions; the sender originates as the type-model.
    /// </summary>
    public delegate void TypeFormatEventHandler(object sender, TypeFormatEventArgs args);
}