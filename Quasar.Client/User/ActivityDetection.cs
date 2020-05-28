using Quasar.Client.Helper;
using Quasar.Client.Networking;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using System;
using System.Threading;

namespace Quasar.Client.User
{
    public class ActivityDetection : IDisposable
    {
        public UserStatus LastUserStatus { get; private set; }

        private readonly QuasarClient _client;

        private readonly CancellationTokenSource _tokenSource;

        private readonly CancellationToken _token;

        public ActivityDetection(QuasarClient client)
        {
            _client = client;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            client.ClientState += OnClientStateChange;
        }

        private void OnClientStateChange(Networking.Client s, bool connected)
        {
            // reset user status
            if (connected)
                LastUserStatus = UserStatus.Active;
        }

        public void Start()
        {
            new Thread(UserIdleThread).Start();
        }

        public void Dispose()
        {
            _client.ClientState -= OnClientStateChange;
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        private void UserIdleThread()
        {
            while (!_token.WaitHandle.WaitOne(1000))
            {
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
            var ticks = Environment.TickCount;

            var idleTime = ticks - NativeMethodsHelper.GetLastInputInfoTickCount();

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }
    }
}
