using Quasar.Common.Networking;

namespace Quasar.Common.Messages
{
    public interface IMessageProcessor
    {
        bool CanExecute(IMessage message);
        bool CanExecuteFrom(ISender sender);
        void Execute(ISender sender, IMessage message);
    }
}
