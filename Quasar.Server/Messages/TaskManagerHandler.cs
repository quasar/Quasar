using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Common.Networking;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class TaskManagerHandler : MessageProcessorBase<Process[]>
    {
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
        public override bool CanExecute(IMessage message) => message is GetProcessesResponse;

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
            _client.Send(new DoProcessStart {ApplicationName = applicationName});
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

        protected override void Dispose(bool disposing)
        {
        }
    }
}
