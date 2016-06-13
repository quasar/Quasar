// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2012
// contacts@aforgenet.com
//
// Copyright © César Souza, 2012
// cesarsouza@gmail.com
//

namespace AForge.Video
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;

    /// <summary>
    /// Screen capture video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source constantly captures the desktop screen.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // get entire desktop area size
    /// Rectangle screenArea = Rectangle.Empty;
    /// foreach ( System.Windows.Forms.Screen screen in 
    ///           System.Windows.Forms.Screen.AllScreens )
    /// {
    ///     screenArea = Rectangle.Union( screenArea, screen.Bounds );
    /// }
    ///     
    /// // create screen capture video source
    /// ScreenCaptureStream stream = new ScreenCaptureStream( screenArea );
    /// 
    /// // set NewFrame event handler
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// 
    /// // start the video source
    /// stream.Start( );
    /// 
    /// // ...
    /// // signal to stop
    /// stream.SignalToStop( );
    /// // ...
    /// 
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class ScreenCaptureStream : IVideoSource
    {
        private Rectangle region;

        // frame interval in milliseconds
        private int frameInterval = 100;
        // received frames count
        private int framesReceived;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from video source.</para>
        /// 
        /// <para><note>Since video source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed video frame, because the video source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        public virtual string Source
        {
            get { return "Screen Capture"; }
        }

        /// <summary>
        /// Gets or sets the screen capture region.
        /// </summary>
        /// 
        /// <remarks><para>This property specifies which region (rectangle) of the screen to capture. It may cover multiple displays
        /// if those are available in the system.</para>
        /// 
        /// <para><note>The property must be set before starting video source to have any effect.</note></para>
        /// </remarks>
        /// 
        public Rectangle Region
        {
            get { return region; }
            set { region = value; }
        }

        /// <summary>
        /// Time interval between making screen shots, ms.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies time interval in milliseconds between consequent screen captures.
        /// Expected frame rate of the stream should be approximately 1000/FrameInteval.</para>
        /// 
        /// <para>If the property is set to 0, then the stream will capture screen as fast as the system allows.</para>
        /// 
        /// <para>Default value is set to <b>100</b>.</para>
        /// </remarks>
        /// 
        public int FrameInterval
        {
            get { return frameInterval; }
            set { frameInterval = Math.Max( 0, value ); }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks><para><note>The property is not implemented for this video source and always returns 0.</note></para>
        /// </remarks>
        /// 
        public long BytesReceived
        {
            get { return 0; }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                if ( thread != null )
                {
                    // check thread status
                    if ( thread.Join( 0 ) == false )
                        return true;

                    // the thread is not running, free resources
                    Free( );
                }
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenCaptureStream"/> class.
        /// </summary>
        /// 
        /// <param name="region">Screen's rectangle to capture (the rectangle may cover multiple displays).</param>
        /// 
        public ScreenCaptureStream( Rectangle region )
        {
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenCaptureStream"/> class.
        /// </summary>
        /// 
        /// <param name="region">Screen's rectangle to capture (the rectangle may cover multiple displays).</param>
        /// <param name="frameInterval">Time interval between making screen shots, ms.</param>
        /// 
        public ScreenCaptureStream( Rectangle region, int frameInterval )
        {
            this.region = region;
            this.FrameInterval = frameInterval;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        /// <exception cref="ArgumentException">Video source is not specified.</exception>
        /// 
        public void Start( )
        {
            if ( !IsRunning )
            {
                framesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent( false );

                // create and start new thread
                thread = new Thread( new ThreadStart( WorkerThread ) );
                thread.Name = Source; // mainly for debugging
                thread.Start( );
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop( )
        {
            // stop thread
            if ( thread != null )
            {
                // signal to stop
                stopEvent.Set( );
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop( )
        {
            if ( thread != null )
            {
                // wait for thread stop
                thread.Join( );

                Free( );
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops video source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop( )
        {
            if ( this.IsRunning )
            {
                stopEvent.Set( );
                thread.Abort( );
                WaitForStop( );
            }
        }

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void Free( )
        {
            thread = null;

            // release events
            stopEvent.Close( );
            stopEvent = null;
        }

        // Worker thread
        private void WorkerThread( )
        {
            int width = region.Width;
            int height = region.Height;
            int x = region.Location.X;
            int y = region.Location.Y;
            Size size = region.Size;

            Bitmap bitmap = new Bitmap( width, height, PixelFormat.Format32bppArgb );
            Graphics graphics = Graphics.FromImage( bitmap );

            // download start time and duration
            DateTime start;
            TimeSpan span;

            while ( !stopEvent.WaitOne( 0, false ) )
            {
                // set dowbload start time
                start = DateTime.Now;

                try
                {
                    // capture the screen
                    graphics.CopyFromScreen( x, y, 0, 0, size, CopyPixelOperation.SourceCopy );

                    // increment frames counter
                    framesReceived++;

                    // provide new image to clients
                    if ( NewFrame != null )
                    {
                        // notify client
                        NewFrame( this, new NewFrameEventArgs( bitmap ) );
                    }

                    // wait for a while ?
                    if ( frameInterval > 0 )
                    {
                        // get download duration
                        span = DateTime.Now.Subtract( start );

                        // miliseconds to sleep
                        int msec = frameInterval - (int) span.TotalMilliseconds;

                        if ( ( msec > 0 ) && ( stopEvent.WaitOne( msec, false ) ) )
                            break;
                    }
                }
                catch ( ThreadAbortException )
                {
                    break;
                }
                catch ( Exception exception )
                {
                    // provide information to clients
                    if ( VideoSourceError != null )
                    {
                        VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                    }
                    // wait for a while before the next try
                    Thread.Sleep( 250 );
                }

                // need to stop ?
                if ( stopEvent.WaitOne( 0, false ) )
                    break;
            }

            // release resources
            graphics.Dispose( );
            bitmap.Dispose( );

            if ( PlayingFinished != null )
            {
                PlayingFinished( this, ReasonToFinishPlaying.StoppedByUser );
            }
        }
    }
}
