using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class TaskManagerHandler : MessageProcessorBase<Process[]>
    {
        /// <summary>
        /// Represents the method that will handle the result of a started process.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="result">The result of the process start.</param>
        public delegate void ProcessStartedEventHandler(object sender, string result);

        /// <summary>
        /// Raised when a result of a started process is received.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event ProcessStartedEventHandler ProcessStarted;

        /// <summary>
        /// Reports the result of a started process.
        /// </summary>
        /// <param name="result">The result of the process start.</param>
        private void OnProcessStarted(string result)
        {
            SynchronizationContext.Post(r =>
            {
                var handler = ProcessStarted;
                handler?.Invoke(this, (string)r);
            }, result);
        }

        /// <summary>
        /// The client which is associated with this task manager handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManagerHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public TaskManagerHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetProcessesResponse ||
                                                             message is DoRemoteExecutionResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(_client);

        /// <inheritdoc />
        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetProcessesResponse proc:
                    Execute(sender, proc);
                    break;
                case DoRemoteExecutionResponse execResp:
                    Execute(sender, execResp);
                    break;
            }
        }

        /// <summary>
        /// Refreshes the current started processes.
        /// </summary>
        public void RefreshProcesses()
        {
            _client.Send(new GetProcesses());
        }

        /// <summary>
        /// Starts a new process given an application name.
        /// </summary>
        /// <param name="applicationName">The name or path of the application to start.</param>
        public void StartProcess(string applicationName)
        {
            _client.Send(new DoRemoteExecution {FilePath = applicationName});
        }

        /// <summary>
        /// Ends a started process given the process id.
        /// </summary>
        /// <param name="pid">The process id to end.</param>
        public void EndProcess(int pid)
        {
            _client.Send(new DoProcessKill {Pid = pid});
        }

        private void Execute(ISender client, GetProcessesResponse message)
        {
            OnReport(message.Processes);
        }

        private void Execute(ISender client, DoRemoteExecutionResponse message)
        {
            OnProcessStarted(message.Success ? "Process started successfully" : "Process failed to start");
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
