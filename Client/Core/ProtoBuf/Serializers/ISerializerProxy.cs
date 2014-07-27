
#if !NO_RUNTIME

namespace ProtoBuf.Serializers
{
    internal interface ISerializerProxy
    {
        IProtoSerializer Serializer { get; }
    }
}

#endif