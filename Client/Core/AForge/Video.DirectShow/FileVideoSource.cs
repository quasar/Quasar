// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Runtime.InteropServices;

    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Video source for video files.
    /// </summary>
    /// 
    /// <remarks><para>The video source provides access to video files. DirectShow is used to access video
    /// files.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source
    /// FileVideoSource videoSource = new FileVideoSource( fileName );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// // ...
    /// // signal to stop
    /// videoSource.SignalToStop( );
    /// // ...
    /// 
    /// // New frame event handler, which is invoked on each new available video frame
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class FileVideoSource : IVideoSource
    {
        // video file name
        private string fileName;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // prevent freezing
        private bool preventFreezing = false;
        // reference clock for the graph - when disabled, graph processes frames ASAP
        private bool referenceClockEnabled = true;

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
        /// <remarks>Video source is represented by video file name.</remarks>
        /// 
        public virtual string Source
        {
            get { return fileName; }
            set { fileName = value; }
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
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public long BytesReceived
        {
            get
            {
                long bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
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
        /// Prevent video freezing after screen saver and workstation lock or not.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The value specifies if the class should prevent video freezing during and
        /// after screen saver or workstation lock. To prevent freezing the <i>DirectShow</i> graph
        /// should not contain <i>Renderer</i> filter, which is added by <i>Render()</i> method
        /// of graph. However, in some cases it may be required to call <i>Render()</i> method of graph, since
        /// it may add some more filters, which may be required for playing video. So, the property is
        /// a trade off - it is possible to prevent video freezing skipping adding renderer filter or
        /// it is possible to keep renderer filter, but video may freeze during screen saver.</para>
        /// 
        /// <para><note>The property may become obsolete in the future when approach to disable freezing
        /// and adding all required filters is found.</note></para>
        /// 
        /// <para><note>The property should be set before calling <see cref="Start"/> method
        /// of the class to have effect.</note></para>
        /// 
        /// <para>Default value of this property is set to <b>false</b>.</para>
        /// 
        /// </remarks>
        /// 
        public bool PreventFreezing
        {
            get { return preventFreezing; }
            set { preventFreezing = value; }
        }

        /// <summary>
        /// Enables/disables reference clock on the graph.
        /// </summary>
        /// 
        /// <remarks><para>Disabling reference clocks causes DirectShow graph to run as fast as
        /// it can process data. When enabled, it will process frames according to presentation
        /// time of a video file.</para>
        /// 
        /// <para><note>The property should be set before calling <see cref="Start"/> method
        /// of the class to have effect.</note></para>
        /// 
        /// <para>Default value of this property is set to <b>true</b>.</para>
        /// </remarks>
        /// 
        public bool ReferenceClockEnabled
        {
            get { return referenceClockEnabled; }
            set { referenceClockEnabled = value;}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        public FileVideoSource( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="fileName">Video file name.</param>
        /// 
        public FileVideoSource( string fileName )
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start( )
        {
            if ( !IsRunning )
            {
                // check source
                if ( ( fileName == null ) || ( fileName == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified" );

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent( false );

                // create and start new thread
                thread = new Thread( new ThreadStart( WorkerThread ) );
                thread.Name = fileName; // mainly for debugging
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

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
        private void WorkerThread( )
        {
            ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying.StoppedByUser;

            // grabber
            Grabber grabber = new Grabber( this );

            // objects
            object graphObject = null;
            object grabberObject = null;

            // interfaces
            IGraphBuilder       graph = null;
            IBaseFilter         sourceBase = null;
            IBaseFilter         grabberBase = null;
            ISampleGrabber      sampleGrabber = null;
            IMediaControl       mediaControl = null;

            IMediaEventEx       mediaEvent = null;

            try
            {
                // get type for filter graph
                Type type = Type.GetTypeFromCLSID( Clsid.FilterGraph );
                if ( type == null )
                    throw new ApplicationException( "Failed creating filter graph" );

                // create filter graph
                graphObject = Activator.CreateInstance( type );
                graph = (IGraphBuilder) graphObject;

                // create source device's object
                graph.AddSourceFilter( fileName, "source", out sourceBase );
                if ( sourceBase == null )
                    throw new ApplicationException( "Failed creating source filter" );

                // get type for sample grabber
                type = Type.GetTypeFromCLSID( Clsid.SampleGrabber );
                if ( type == null )
                    throw new ApplicationException( "Failed creating sample grabber" );

                // create sample grabber
                grabberObject = Activator.CreateInstance( type );
                sampleGrabber = (ISampleGrabber) grabberObject;
                grabberBase = (IBaseFilter) grabberObject;

                // add grabber filters to graph
                graph.AddFilter( grabberBase, "grabber" );

                // set media type
                AMMediaType mediaType = new AMMediaType( );
                mediaType.MajorType = MediaType.Video;
                mediaType.SubType = MediaSubType.RGB24;
                sampleGrabber.SetMediaType( mediaType );

                // connect pins
                int pinToTry = 0;

                IPin inPin = Tools.GetInPin( grabberBase, 0 );
                IPin outPin = null;

                // find output pin acceptable by sample grabber
                while ( true )
                {
                    outPin = Tools.GetOutPin( sourceBase, pinToTry );

                    if ( outPin == null )
                    {
                        Marshal.ReleaseComObject( inPin );
                        throw new ApplicationException( "Did not find acceptable output video pin in the given source" );
                    }

                    if ( graph.Connect( outPin, inPin ) < 0 )
                    {
                        Marshal.ReleaseComObject( outPin );
                        outPin = null;
                        pinToTry++;
                    }
                    else
                    {
                        break;
                    }
                }

                Marshal.ReleaseComObject( outPin );
                Marshal.ReleaseComObject( inPin );

                // get media type
                if ( sampleGrabber.GetConnectedMediaType( mediaType ) == 0 )
                {
                    VideoInfoHeader vih = (VideoInfoHeader) Marshal.PtrToStructure( mediaType.FormatPtr, typeof( VideoInfoHeader ) );

                    grabber.Width = vih.BmiHeader.Width;
                    grabber.Height = vih.BmiHeader.Height;
                    mediaType.Dispose( );
                }

                // let's do rendering, if we don't need to prevent freezing
                if ( !preventFreezing )
                {
                    // render pin
                    graph.Render( Tools.GetOutPin( grabberBase, 0 ) );

                    // configure video window
                    IVideoWindow window = (IVideoWindow) graphObject;
                    window.put_AutoShow( false );
                    window = null;
                }

                // configure sample grabber
                sampleGrabber.SetBufferSamples( false );
                sampleGrabber.SetOneShot( false );
                sampleGrabber.SetCallback( grabber, 1 );

                // disable clock, if someone requested it
                if ( !referenceClockEnabled )
                {
                    IMediaFilter mediaFilter = (IMediaFilter) graphObject;
                    mediaFilter.SetSyncSource( null );
                }

                // get media control
                mediaControl = (IMediaControl) graphObject;

                // get media events' interface
                mediaEvent = (IMediaEventEx) graphObject;
                IntPtr p1, p2;
                DsEvCode code;

                // run
                mediaControl.Run( );

                do
                {
                    if ( mediaEvent != null )
                    {
                        if ( mediaEvent.GetEvent( out code, out p1, out p2, 0 ) >= 0 )
                        {
                            mediaEvent.FreeEventParams( code, p1, p2 );

                            if ( code == DsEvCode.Complete )
                            {
                                reasonToStop = ReasonToFinishPlaying.EndOfStreamReached;
                                break;
                            }
                        }
                    }
                }
                while ( !stopEvent.WaitOne( 100, false ) );

                mediaControl.Stop( );
            }
            catch ( Exception exception )
            {
                // provide information to clients
                if ( VideoSourceError != null )
                {
                    VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                }
            }
            finally
            {
                // release all objects
                graph           = null;
                grabberBase     = null;
                sampleGrabber   = null;
                mediaControl    = null;
                mediaEvent      = null;

                if ( graphObject != null )
                {
                    Marshal.ReleaseComObject( graphObject );
                    graphObject = null;
                }
                if ( sourceBase != null )
                {
                    Marshal.ReleaseComObject( sourceBase );
                    sourceBase = null;
                }
                if ( grabberObject != null )
                {
                    Marshal.ReleaseComObject( grabberObject );
                    grabberObject = null;
                }
            }

            if ( PlayingFinished != null )
            {
                PlayingFinished( this, reasonToStop );
            }
        }

        /// <summary>
        /// Notifies client about new frame.
        /// </summary>
        /// 
        /// <param name="image">New frame's image.</param>
        /// 
        protected void OnNewFrame( Bitmap image )
        {
            framesReceived++;
            bytesReceived += image.Width * image.Height * ( Bitmap.GetPixelFormatSize( image.PixelFormat ) >> 3 );

            if ( ( !stopEvent.WaitOne( 0, false ) ) && ( NewFrame != null ) )
                NewFrame( this, new NewFrameEventArgs( image ) );
        }

        //
        // Video grabber
        //
        private class Grabber : ISampleGrabberCB
        {
            private FileVideoSource parent;
            private int width, height;

            // Width property
            public int Width
            {
                get { return width; }
                set { width = value; }
            }
            // Height property
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            // Constructor
            public Grabber( FileVideoSource parent )
            {
                this.parent = parent;
            }

            // Callback to receive samples
            public int SampleCB( double sampleTime, IntPtr sample )
            {
                return 0;
            }

            // Callback method that receives a pointer to the sample buffer
            public int BufferCB( double sampleTime, IntPtr buffer, int bufferLen )
            {
                if ( parent.NewFrame != null )
                {
                    // create new image
                    System.Drawing.Bitmap image = new Bitmap( width, height, PixelFormat.Format24bppRgb );

                    // lock bitmap data
                    BitmapData imageData = image.LockBits(
                        new Rectangle( 0, 0, width, height ),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb );

                    // copy image data
                    int srcStride = imageData.Stride;
                    int dstStride = imageData.Stride;

                    unsafe
                    {
                        byte* dst = (byte*) imageData.Scan0.ToPointer( ) + dstStride * ( height - 1 );
                        byte* src = (byte*) buffer.ToPointer( );

                        for ( int y = 0; y < height; y++ )
                        {
                            Win32.memcpy( dst, src, srcStride );
                            dst -= dstStride;
                            src += srcStride;
                        }
                    }

                    // unlock bitmap data
                    image.UnlockBits( imageData );

                    // notify parent
                    parent.OnNewFrame( image );

                    // release the image
                    image.Dispose( );
                }

                return 0;
            }
        }
    }
}
