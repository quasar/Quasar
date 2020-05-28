using Quasar.Common.Networking;

namespace Quasar.Common.Messages
{
    /// <summary>
    /// Provides basic functionality to process messages.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Decides whether this message processor can execute the specified message.
        /// </summary>
        /// <param name="message">The message to execute.</param>
        /// <returns><c>True</c> if the message can be executed by this message processor, otherwise <c>false</c>.</returns>
        bool CanExecute(IMessage message);

        /// <summary>
        /// Decides whether this message processor can execute messages received from the sender.
        /// </summary>
        /// <param name="sender">The sender of a message.</param>
        /// <returns><c>True</c> if this message processor can execute messages from the sender, otherwise <c>false</c>.</returns>
        bool CanExecuteFrom(ISender sender);

        /// <summary>
        /// Executes the received message.
        /// </summary>
        /// <param name="sender">The sender of this message.</param>
        /// <param name="message">The received message.</param>
        void Execute(ISender sender, IMessage message);
    }
}
