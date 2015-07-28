using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using xServer.Core.Utilities;

namespace xServer.Controls
{
    public delegate void PictureSizeChangedEventHandler(int width, int height);

    public class PictureSizeChangedEventArgs : EventArgs
    {
        public int NewWidth;
        public int NewHeight;

        public PictureSizeChangedEventArgs(int width, int height)
        {
            NewWidth = width;
            NewHeight = height;
        }
    }

    public interface IRapidPictureBox
    {
        bool Running { get; set; }

        void Start();
        void Stop();
        void UpdateImage(Bitmap bmp, bool cloneBitmap = false);

        Image _Image { get; set; }
    }

    /// <summary>
    /// Custom PictureBox Control designed for rapidly-changing images.
    /// </summary>
    public partial class PictureBoxEx : PictureBox, IRapidPictureBox
    {
        #region IRapidPictureBox Implementation

        public bool Running { get; set; }

        public void Start()
        {
            _frameCounter = new FrameCounter();

            _sWatch = Stopwatch.StartNew();

            Running = true;
        }

        public void Stop()
        {
            if (_sWatch != null)
                _sWatch.Stop();

            Running = false;
        }

        public void UpdateImage(Bitmap bmp, bool cloneBitmap = false)
        {
            try
            {
                CountFps();

                if ((bmpWidth != bmp.Width) && (bmpHeight != bmp.Height))
                    OnPictureSizeChanged(new PictureSizeChangedEventArgs(bmp.Width, bmp.Height));

                lock (ImgLocker)
                {
                    if (this._Image != null)
                    {
                        this._Image.Dispose();
                        this._Image = null;
                    }

                    this._Image = cloneBitmap ? new Bitmap(bmp, picDesktop.Width, picDesktop.Height) /*resize bitmap*/ : bmp;
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        // Fields used to keep track of the remote desktop's size.
        public int bmpWidth { get; private set; }
        public int bmpHeight { get; private set; }

        // Fields for the FrameCounter.
        public FrameCounter _frameCounter;
        private Stopwatch _sWatch;

        public PictureBoxEx()
        {
            InitializeComponent();

            //this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            _frameCounter = new FrameCounter();
        }

        #region Events

        public event PictureSizeChangedEventHandler PictureSizeChanged;

        protected virtual void OnPictureSizeChanged(PictureSizeChangedEventArgs e)
        {
            PictureSizeChangedEventHandler handler = PictureSizeChanged;
            if (handler != null)
                handler(e.NewWidth, e.NewHeight);
        }

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            lock (ImgLocker)
            {
                if (this._Image != null)
                {
                    pe.Graphics.DrawImage(this._Image, this.Location);
                }
            }
        }

        #endregion

        private void CountFps()
        {
            var deltaTime = (float)_sWatch.Elapsed.TotalSeconds;
            _sWatch = Stopwatch.StartNew();

            _frameCounter.Update(deltaTime);
        }

        private readonly object ImgLocker = new object();
        /// <summary>
        /// Provides thread-safe access to the PictureBox's image.
        /// </summary>
        public Image _Image
        {
            get
            {
                return picDesktop.Image;
            }
            set
            {
                lock (ImgLocker)
                {
                    picDesktop.Image = value;
                }
            }
        }
    }
}