using Quasar.Common.Networking;
using System;
using System.Threading;

namespace Quasar.Common.Messages
{
    /// <summary>
    /// Provides a MessageProcessor implementation that provides progress report callbacks.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
    /// <remarks>
    /// Any event handlers registered with the <see cref="ProgressChanged"/> event are invoked through a 
    /// <see cref="System.Threading.SynchronizationContext"/> instance chosen when the instance is constructed.
    /// </remarks>
    /// TODO: .NET 4.5 Change: this can be simplified with .NET 4.5+ --> IProgress{T} in System namespace
    public abstract class MessageProcessorBase<T> : IMessageProcessor, IProgress<T>, IDisposable
    {
        /// <summary>
        /// The synchronization context chosen upon construction.
        /// </summary>
        protected readonly SynchronizationContext SynchronizationContext;

        /// <summary>
        /// A cached delegate used to post invocation to the synchronization context.
        /// </summary>
        private readonly SendOrPostCallback _invokeReportProgressHandlers;

        /// <summary>
        /// Represents the method that will handle progress updates.
        /// </summary>
        /// <param name="sender">The message processor which updated the progress.</param>
        /// <param name="value">The new progress.</param>
        public delegate void ReportProgressEventHandler(object sender, T value);

        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event ReportProgressEventHandler ProgressChanged;

        /// <summary>
        /// Reports a progress change.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        protected virtual void OnReport(T value)
        {
            // If there's no handler, don't bother going through the sync context.
            // Inside the callback, we'll need to check again, in case 
            // an event handler is removed between now and then.
            var handler = ProgressChanged;
            if (handler != null)
            {
                SynchronizationContext.Post(_invokeReportProgressHandlers, value);
            }
        }

        /// <summary>
        /// Initializes the <see cref="MessageProcessorBase{T}"/>
        /// </summary>
        /// <param name="useCurrentContext">
        /// If this value is <code>false</code>, the progress callbacks will be invoked on the ThreadPool.
        /// Otherwise the current SynchronizationContext will be used.
        /// </param>
        protected MessageProcessorBase(bool useCurrentContext)
        {
            _invokeReportProgressHandlers = InvokeReportProgressHandlers;
            SynchronizationContext = useCurrentContext ? SynchronizationContext.Current : ProgressStatics.DefaultContext;
        }

        /// <summary>
        /// Invokes the progress event callbacks.
        /// </summary>
        /// <param name="state">The progress value.</param>
        private void InvokeReportProgressHandlers(object state)
        {
            var handler = ProgressChanged;
            handler?.Invoke(this, (T)state);
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this message processor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Decides whether this message processor can execute the specified message.
        /// </summary>
        /// <param name="message">The message to execute.</param>
        /// <returns><code>True</code> if the message can be executed by this message processor, otherwise <code>false</code>.</returns>
        public abstract bool CanExecute(IMessage message);

        /// <summary>
        /// Decides whether this message processor can execute messages received from the sender.
        /// </summary>
        /// <param name="sender">The sender of a message.</param>
        /// <returns><code>True</code> if this message processor can execute messages from the sender, otherwise <code>false</code>.</returns>
        public abstract bool CanExecuteFrom(ISender sender);

        /// <summary>
        /// Executes the received message.
        /// </summary>
        /// <param name="sender">The sender of this message.</param>
        /// <param name="message">The received message.</param>
        public abstract void Execute(ISender sender, IMessage message);

        protected abstract void Dispose(bool disposing);
        void IProgress<T>.Report(T value) => OnReport(value);
    }

    /// <summary>
    /// Holds static values for <see cref="MessageProcessorBase{T}"/>.
    /// </summary>
    /// <remarks>
    /// This avoids one static instance per type T.
    /// </remarks>
    internal static class ProgressStatics
    {
        /// <summary>
        /// A default synchronization context that targets the ThreadPool.
        /// </summary>
        internal static readonly SynchronizationContext DefaultContext = new SynchronizationContext();
    }
}
