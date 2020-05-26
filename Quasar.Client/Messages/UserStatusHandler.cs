using Quasar.Client.Helper;
using Quasar.Client.Networking;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using System.Diagnostics;
using System.Threading;

namespace Quasar.Client.Messages
{
    public class UserStatusHandler : MessageProcessorBase<object>
    {
        public UserStatus LastUserStatus { get; private set; }

        private readonly QuasarClient _client;

        private readonly CancellationTokenSource _tokenSource;

        private readonly CancellationToken _token;

        public UserStatusHandler(QuasarClient client) : base(false)
        {
            // TODO: handle disconnect
            _client = client;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            new Thread(UserIdleThread) { IsBackground = true }.Start();
        }

        public override bool CanExecute(IMessage message) => false;

        public override bool CanExecuteFrom(ISender sender) => false;

        public override void Execute(ISender sender, IMessage message)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tokenSource.Cancel();
            }
        }

        private void UserIdleThread()
        {
            while (!_token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                if (IsUserIdle())
                {
                    if (LastUserStatus != UserStatus.Idle)
                    {
                        LastUserStatus = UserStatus.Idle;
                        _client.Send(new SetUserStatus {Message = LastUserStatus});
                    }
                }
                else
                {
                    if (LastUserStatus != UserStatus.Active)
                    {
                        LastUserStatus = UserStatus.Active;
                        _client.Send(new SetUserStatus {Message = LastUserStatus});
                    }
                }
            }

        }

        private bool IsUserIdle()
        {
            long ticks = Stopwatch.GetTimestamp();

            long idleTime = ticks - NativeMethodsHelper.GetLastInputInfoTickCount();

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }
    }
}
