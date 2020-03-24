using System.Drawing;
using System.IO;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Networking;
using Quasar.Common.Video.Codecs;
using Quasar.Server.Networking;

namespace Quasar.Server.Messages
{
    public class RemoteDesktopHandler : MessageProcessorBase<Bitmap>
    {
        /// <summary>
        /// States if the client is currently streaming desktop frames.
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// Used in lock statements to synchronize access to <see cref="_codec"/> between UI thread and thread pool.
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// Used in lock statements to synchronize access to <see cref="LocalResolution"/> between UI thread and thread pool.
        /// </summary>
        private readonly object _sizeLock = new object();

        /// <summary>
        /// The local resolution, see <seealso cref="LocalResolution"/>.
        /// </summary>
        private Size _localResolution;

        /// <summary>
        /// The local resolution in width x height. It indicates to which resolution the received frame should be resized.
        /// </summary>
        /// <remarks>
        /// This property is thread-safe.
        /// </remarks>
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
        /// <param name="sender">The message processor which raised the event.</param>
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
        /// <param name="value">All currently available displays.</param>
        private void OnDisplaysChanged(int value)
        {
            SynchronizationContext.Post(val =>
            {
                var handler = DisplaysChanged;
                handler?.Invoke(this, (int)val);
            }, value);
        }

        /// <summary>
        /// The client which is associated with this remote desktop handler.
        /// </summary>
        private readonly Client _client;

        /// <summary>
        /// The video stream codec used to decode received frames.
        /// </summary>
        private UnsafeStreamCodec _codec;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDesktopHandler"/> class using the given client.
        /// </summary>
        /// <param name="client">The associated client.</param>
        public RemoteDesktopHandler(Client client) : base(true)
        {
            _client = client;
        }

        /// <inheritdoc />
        public override bool CanExecute(IMessage message) => message is GetDesktopResponse || message is GetMonitorsResponse;

        /// <inheritdoc />
        public override bool CanExecuteFrom(ISender sender) => _client.Equals(sender);

        /// <inheritdoc />
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

        /// <summary>
        /// Begins receiving frames from the client using the specified quality and display.
        /// </summary>
        /// <param name="quality">The quality of the remote desktop frames.</param>
        /// <param name="display">The display to receive frames from.</param>
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

        /// <summary>
        /// Ends receiving frames from the client.
        /// </summary>
        public void EndReceiveFrames()
        {
            lock (_syncLock)
            {
                IsStarted = false;
            }
        }

        /// <summary>
        /// Refreshes the available displays of the client.
        /// </summary>
        public void RefreshDisplays()
        {
            _client.Send(new GetMonitors());
        }

        /// <summary>
        /// Sends a mouse event to the specified display of the client.
        /// </summary>
        /// <param name="mouseAction">The mouse action to send.</param>
        /// <param name="isMouseDown">Indicates whether it's a mousedown or mouseup event.</param>
        /// <param name="x">The X-coordinate inside the <see cref="LocalResolution"/>.</param>
        /// <param name="y">The Y-coordinate inside the <see cref="LocalResolution"/>.</param>
        /// <param name="displayIndex">The display to execute the mouse event on.</param>
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

        /// <summary>
        /// Sends a keyboard event to the client.
        /// </summary>
        /// <param name="keyCode">The pressed key.</param>
        /// <param name="keyDown">Indicates whether it's a keydown or keyup event.</param>
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
                lock (_syncLock)
                {
                    _codec?.Dispose();
                    IsStarted = false;
                }
            }
        }
    }
}
