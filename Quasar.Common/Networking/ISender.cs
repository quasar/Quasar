using Quasar.Common.Messages;

namespace Quasar.Common.Networking
{
    public interface ISender
    {
        void Send<T>(T message) where T : IMessage;
        void Disconnect();
    }
}
