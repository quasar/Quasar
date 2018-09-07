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

        private readonly SendOrPostCallback _invokeClientDisconnectedHandlers;

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

        public delegate void ClientDisconnectedEventHandler(object sender, ISender client);
        public event ClientDisconnectedEventHandler ClientDisconnected;

        protected virtual void OnClientDisconnected(ISender client)
        {
            // If there's no handler, don't bother going through the sync context.
            // Inside the callback, we'll need to check again, in case 
            // an event handler is removed between now and then.
            var handler = ProgressChanged;
            if (handler != null)
            {
                SynchronizationContext.Post(_invokeClientDisconnectedHandlers, client);
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
            _invokeClientDisconnectedHandlers = InvokeClientDisconnectedHandlers;
            SynchronizationContext = useCurrentContext ? SynchronizationContext.Current : ProgressStatics.DefaultContext;
        }

        /// <summary>
        /// Invokes the action and event callbacks.
        /// </summary>
        /// <param name="state">The progress value.</param>
        private void InvokeReportProgressHandlers(object state)
        {
            var handler = ProgressChanged;
            handler?.Invoke(this, (T)state);
        }

        private void InvokeClientDisconnectedHandlers(object state)
        {
            var handler = ClientDisconnected;
            handler?.Invoke(this, (ISender)state);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void NotifyDisconnect(ISender sender) => OnClientDisconnected(sender);
        void IProgress<T>.Report(T value) => OnReport(value);
        public abstract bool CanExecute(IMessage message);
        public abstract bool CanExecuteFrom(ISender sender);
        public abstract void Execute(ISender sender, IMessage message);
        protected abstract void Dispose(bool disposing);
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
