using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Common.Video.Codecs;
using System.Drawing;
using System.IO;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    // TODO: Capture mouse in frames: https://stackoverflow.com/questions/6750056/how-to-capture-the-screen-and-mouse-pointer-using-windows-apis
    public class RemoteDesktopHandler : MessageProcessorBase<Bitmap>
    {
        /// <summary>
        /// States if the client is currently streaming desktop frames.
        /// </summary>
        public bool IsStarted { get; set; }

        private readonly object _syncLock = new object();

        private readonly object _sizeLock = new object();

        private Size _localResolution;

        public Size LocalResolution
        {
            get
            {
                lock (_sizeLock)
                {
                    return _localResolution;
                }
            }
            set
            {
                lock (_sizeLock)
                {
                    _localResolution = value;
                }
            }
        }

        /// <summary>
        /// Represents the method that will handle display changes.
        /// </summary>
        /// <param name="sender">The message processor which updated the progress.</param>
        /// <param name="value">All currently available displays.</param>
        public delegate void DisplaysChangedEventHandler(object sender, int value);

        /// <summary>
        /// Raised when a display changed.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> chosen when the instance was constructed.
        /// </remarks>
        public event DisplaysChangedEventHandler DisplaysChanged;

        /// <summary>
        /// Reports changed displays.
        /// </summary>
        /// <param name="value">
        /// All currently available displays.
        /// </param>
        protected virtual void OnDisplaysChanged(int value)
        {
            SynchronizationContext.Post(val =>
            {
                var handler = DisplaysChanged;
                handler?.Invoke(this, (int)val);
            }, value);
        }

        private readonly Client _client;
        private UnsafeStreamCodec _codec;

        public RemoteDesktopHandler(Client client) : base(true)
        {
            _client = client;
        }

        public override bool CanExecute(IMessage message) => message is GetDesktopResponse || message is GetMonitorsResponse;

        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        public override void Execute(ISender sender, IMessage message)
        {
            switch (message)
            {
                case GetDesktopResponse d:
                    Execute(sender, d);
                    break;
                case GetMonitorsResponse m:
                    Execute(sender, m);
                    break;
            }
        }

        public void BeginReceiveFrames(int quality, int display)
        {
            lock (_syncLock)
            {
                IsStarted = true;
                _codec?.Dispose();
                _codec = null;
                _client.Send(new GetDesktop { CreateNew = true, Quality = quality, DisplayIndex = display });
            }
        }

        public void EndReceiveFrames()
        {
            lock (_syncLock)
            {
                IsStarted = false;
            }
        }

        public void RefreshDisplays()
        {
            _client.Send(new GetMonitors());
        }

        public void SendMouseEvent(MouseAction mouseAction, bool isMouseDown, int x, int y, int displayIndex)
        {
            lock (_syncLock)
            {
                _client.Send(new DoMouseEvent
                {
                    Action = mouseAction,
                    IsMouseDown = isMouseDown,
                    // calculate remote width & height
                    X = x * _codec.Resolution.Width / LocalResolution.Width,
                    Y = y * _codec.Resolution.Height / LocalResolution.Height,
                    MonitorIndex = displayIndex
                });
            }
        }

        public void SendKeyboardEvent(byte keyCode, bool keyDown)
        {
            _client.Send(new DoKeyboardEvent {Key = keyCode, KeyDown = keyDown});
        }

        private void Execute(ISender client, GetDesktopResponse message)
        {
            lock (_syncLock)
            {
                if (!IsStarted)
                    return;

                if (_codec == null || _codec.ImageQuality != message.Quality || _codec.Monitor != message.Monitor || _codec.Resolution != message.Resolution)
                {
                    _codec?.Dispose();
                    _codec = new UnsafeStreamCodec(message.Quality, message.Monitor, message.Resolution);
                }

                using (MemoryStream ms = new MemoryStream(message.Image))
                {
                    // create deep copy & resize bitmap to local resolution
                    OnReport(new Bitmap(_codec.DecodeData(ms), LocalResolution));
                }
                
                message.Image = null;

                client.Send(new GetDesktop {Quality = message.Quality, DisplayIndex = message.Monitor});
            }
        }

        private void Execute(ISender client, GetMonitorsResponse message)
        {
            OnDisplaysChanged(message.Number);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                lock (_syncLock)
                {
                    _codec?.Dispose();
                    IsStarted = false;
                }
            }
            // get rid of unmanaged resources
        }
    }
}
