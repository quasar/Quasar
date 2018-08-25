using Quasar.Common.Networking;

namespace Quasar.Common.Messages
{
    public abstract class AbstractMessageProcessor : IMessageProcessor
    {
        public delegate void ReportProgressEventHandler(object sender, object arg);
        public event ReportProgressEventHandler ReportProgress;

        protected virtual void OnReportProgress(object e)
        {
            ReportProgress?.Invoke(this, e);
        }

        //public abstract void NotifyDisconnect(Client sender);
        public abstract bool CanExecute(IMessage message);
        public abstract bool CanExecuteFrom(ISender sender);
        public abstract void Execute(ISender sender, IMessage message);
    }
}
