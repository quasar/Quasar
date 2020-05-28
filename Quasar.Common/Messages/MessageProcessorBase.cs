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
    public abstract class MessageProcessorBase<T> : IMessageProcessor, IProgress<T>
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
        /// If this value is <c>false</c>, the progress callbacks will be invoked on the ThreadPool.
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

        /// <inheritdoc />
        public abstract bool CanExecute(IMessage message);

        /// <inheritdoc />
        public abstract bool CanExecuteFrom(ISender sender);

        /// <inheritdoc />
        public abstract void Execute(ISender sender, IMessage message);

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
        /// A default synchronization context that targets the <see cref="ThreadPool"/>.
        /// </summary>
        internal static readonly SynchronizationContext DefaultContext = new SynchronizationContext();
    }
}
