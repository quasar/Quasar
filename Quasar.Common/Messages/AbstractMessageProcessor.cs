using System.Threading;
using Quasar.Common.Networking;

namespace Quasar.Common.Messages
{
    /// <summary>
    /// Provides a MessageProcessor implementation that invokes callbacks for each reported progress value.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
    /// <remarks>
    /// Any handler provided to the constructor or event handlers registered with
    /// the <see cref="ProgressChanged"/> event are invoked through a 
    /// <see cref="System.Threading.SynchronizationContext"/> instance chosen
    /// when the instance is constructed.
    /// </remarks>
    /// TODO: This can be simplified with .NET Framework 4.5+
    public abstract class AbstractMessageProcessor<T> : IMessageProcessor, IProgress<T>
    {
        /// <summary>
        /// The synchronization context chosen upon construction.
        /// </summary>
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// A cached delegate used to post invocation to the synchronization context.
        /// </summary>
        private readonly SendOrPostCallback _invokeHandlers;

        /// <summary>
        /// Represents the method that will handle progress updates.
        /// </summary>
        /// <param name="sender">The client which changed its state.</param>
        /// <param name="value">The new connection state of the client.</param>
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
        /// <param name="value">
        /// The value of the updated progress.
        /// </param>
        protected virtual void OnReport(T value)
        {
            // If there's no handler, don't bother going through the sync context.
            // Inside the callback, we'll need to check again, in case 
            // an event handler is removed between now and then.
            var handler = ProgressChanged;
            if (handler != null)
            {
                _synchronizationContext.Post(_invokeHandlers, value);
            }
        }

        /// <summary>
        /// Initializes the <see cref="AbstractMessageProcessor{T}"/>
        /// </summary>
        /// <param name="useCurrentContext">
        /// If this value is <code>false</code>, the callbacks progress callbacks will be invoked on the ThreadPool.
        /// Otherwise the current SynchronizationContext will be used.
        /// </param>
        protected AbstractMessageProcessor(bool useCurrentContext)
        {
            _invokeHandlers = InvokeHandlers;
            _synchronizationContext = useCurrentContext ? SynchronizationContext.Current : ProgressStatics.DefaultContext;
        }

        /// <summary>
        /// Invokes the action and event callbacks.
        /// </summary>
        /// <param name="state">
        /// The progress value.
        /// </param>
        private void InvokeHandlers(object state)
        {
            T value = (T)state;

            var handler = ProgressChanged;
            handler?.Invoke(this, value);
        }

        void IProgress<T>.Report(T value) { OnReport(value); }
        //public abstract void NotifyDisconnect(ISender sender);
        public abstract bool CanExecute(IMessage message);
        public abstract bool CanExecuteFrom(ISender sender);
        public abstract void Execute(ISender sender, IMessage message);
    }

    /// <summary>
    /// Holds static values for <see cref="AbstractMessageProcessor{T}"/>.
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
