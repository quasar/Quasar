// AForge Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;

    /// <summary>
    /// Proxy video source for asynchronous processing of another nested video source.
    /// </summary>
    /// 
    /// <remarks><para>The class represents a simple proxy, which wraps the specified <see cref="NestedVideoSource"/>
    /// with the aim of asynchronous processing of received video frames. The class intercepts <see cref="NewFrame"/>
    /// event from the nested video source and fires it to clients from its own thread, which is different from the thread
    /// used by nested video source for video acquisition. This allows clients to perform processing of video frames
    /// without blocking video acquisition thread, which continue to run and acquire next video frame while current is still
    /// processed.</para>
    /// 
    /// <para>For example, let’s suppose that it takes 100 ms for the nested video source to acquire single frame, so the original
    /// frame rate is 10 frames per second. Also let’s assume that we have an image processing routine, which also takes
    /// 100 ms to process a single frame. If the acquisition and processing are done sequentially, then resulting
    /// frame rate will drop to 5 frames per second. However, if doing both in parallel, then there is a good chance to
    /// keep resulting frame rate equal (or close) to the original frame rate.</para>
    /// 
    /// <para>The class provides a bonus side effect - easer debugging of image processing routines, which are put into
    /// <see cref="NewFrame"/> event handler. In many cases video source classes fire their <see cref="IVideoSource.NewFrame"/>
    /// event from a try/catch block, which makes it very hard to spot error made in user's code - the catch block simply
    /// hides exception raised in user’s code. The <see cref="AsyncVideoSource"/> does not have any try/catch blocks around
    /// firing of <see cref="NewFrame"/> event, so always user gets exception in the case it comes from his code. At the same time
    /// nested video source is not affected by the user's exception, since it runs in different thread.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // usage of AsyncVideoSource is the same as usage of any
    /// // other video source class, so code change is very little
    /// 
    /// // create nested video source, for example JPEGStream
    /// JPEGStream stream = new JPEGStream( "some url" );
    /// // create async video source
    /// AsyncVideoSource asyncSource = new AsyncVideoSource( stream );
    /// // set NewFrame event handler
    /// asyncSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// asyncSource.Start( );
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
    public class AsyncVideoSource : IVideoSource
    {
        private readonly IVideoSource nestedVideoSource = null;
        private Bitmap lastVideoFrame = null;

        private Thread imageProcessingThread = null;
        private AutoResetEvent isNewFrameAvailable = null;
        private AutoResetEvent isProcessingThreadAvailable = null;

        // skip frames or not in the case if processing thread is busy
        private bool skipFramesIfBusy = false;
        // processed frames count
        private int framesProcessed;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from video source.</para>
        /// 
        /// <para><note>This event is fired from a different thread other than the video acquisition thread created
        /// by <see cref="NestedVideoSource"/>. This allows nested video frame to continue acquisition of the next
        /// video frame while clients perform processing of the current video frame.</note></para>
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
        /// <remarks><para>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</para>
        /// 
        /// <para><note>Unlike <see cref="NewFrame"/> event, this event is simply redirected to the corresponding
        /// event of the <see cref="NestedVideoSource"/>, so it is fired from the thread of the nested video source.</note></para>
        /// </remarks>
        ///
        public event VideoSourceErrorEventHandler VideoSourceError
        {
            add { nestedVideoSource.VideoSourceError += value; }
            remove { nestedVideoSource.VideoSourceError -= value; }
        }

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// 
        /// <para><note>Unlike <see cref="NewFrame"/> event, this event is simply redirected to the corresponding
        /// event of the <see cref="NestedVideoSource"/>, so it is fired from the thread of the nested video source.</note></para>
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished
        {
            add { nestedVideoSource.PlayingFinished += value; }
            remove { nestedVideoSource.PlayingFinished -= value; }
        }

        /// <summary>
        /// Nested video source which is the target for asynchronous processing.
        /// </summary>
        /// 
        /// <remarks><para>The property is set through the class constructor.</para>
        /// 
        /// <para>All calls to this object are actually redirected to the nested video source. The only
        /// exception is the <see cref="NewFrame"/> event, which is handled differently. This object gets
        /// <see cref="IVideoSource.NewFrame"/> event from the nested class and then fires another
        /// <see cref="NewFrame"/> event, but from a different thread.</para>
        /// </remarks>
        /// 
        public IVideoSource NestedVideoSource
        {
            get { return nestedVideoSource; }
        }

        /// <summary>
        /// Specifies if the object should skip frames from the nested video source when it is busy. 
        /// </summary>
        /// 
        /// <remarks><para>Specifies if the object should skip frames from the nested video source
        /// in the case if it is still busy processing the previous video frame in its own thread.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para></remarks>
        /// 
        public bool SkipFramesIfBusy
        {
            get { return skipFramesIfBusy; }
            set { skipFramesIfBusy = value; }
        }

        /// <summary>
        /// Video source string.
        /// </summary>
        /// 
        /// <remarks><para>The property is redirected to the corresponding property of <see cref="NestedVideoSource"/>,
        /// so check its documentation to find what it means.</para></remarks>
        /// 
        public string Source
        {
            get { return nestedVideoSource.Source; }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks><para>Number of frames the <see cref="NestedVideoSource">nested video source</see> received from
        /// the moment of the last access to the property.</para>
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get { return nestedVideoSource.FramesReceived; }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks><para>Number of bytes the <see cref="NestedVideoSource">nested video source</see> received from
        /// the moment of the last access to the property.</para></remarks>
        ///
        public long BytesReceived
        {
            get { return nestedVideoSource.BytesReceived; }
        }

        /// <summary>
        /// Processed frames count.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps the number of processed video frames since the last access to this property. 
        /// </para>
        /// 
        /// <para>The value of this property equals to <see cref="FramesReceived"/> in most cases if the
        /// <see cref="SkipFramesIfBusy"/> property is set to <see langword="false"/> - every received frame gets processed
        /// sooner or later. However, if the <see cref="SkipFramesIfBusy"/> property is set to <see langword="true"/>,
        /// then value of this property may be lower than the value of the <see cref="FramesReceived"/> property, which
        /// means that nested video source performs acquisition faster than client perform processing of the received frame
        /// and some frame are skipped from processing.</para>
        /// </remarks>
        /// 
        public int FramesProcessed
        {
            get
            {
                int frames = framesProcessed;
                framesProcessed = 0;
                return frames;
            }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks><para>Current state of the video source object - running or not.</para></remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                bool isRunning = nestedVideoSource.IsRunning;

                if ( !isRunning )
                {
                    Free( );
                }

                return isRunning;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="nestedVideoSource">Nested video source which is the target for asynchronous processing.</param>
        /// 
        public AsyncVideoSource( IVideoSource nestedVideoSource )
        {
            this.nestedVideoSource = nestedVideoSource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="nestedVideoSource">Nested video source which is the target for asynchronous processing.</param>
        /// <param name="skipFramesIfBusy">Specifies if the object should skip frames from the nested video source
        /// in the case if it is still busy processing the previous video frame.</param>
        /// 
        public AsyncVideoSource( IVideoSource nestedVideoSource, bool skipFramesIfBusy )
        {
            this.nestedVideoSource = nestedVideoSource;
            this.skipFramesIfBusy = skipFramesIfBusy;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks><para>Starts the nested video source and returns execution to caller. This object creates
        /// an extra thread which is used to fire <see cref="NewFrame"/> events, so the image processing could be
        /// done on another thread without blocking video acquisition thread.</para></remarks>
        /// 
        public void Start( )
        {
            if ( !IsRunning )
            {
                framesProcessed = 0;

                // create all synchronization events
                isNewFrameAvailable = new AutoResetEvent( false );
                isProcessingThreadAvailable = new AutoResetEvent( true );

                // create image processing thread
                imageProcessingThread = new Thread( new ThreadStart( imageProcessingThread_Worker ) );
                imageProcessingThread.Start( );

                // start the nested video source
                nestedVideoSource.NewFrame += new NewFrameEventHandler( nestedVideoSource_NewFrame );
                nestedVideoSource.Start( );
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks><para>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</para></remarks>
        ///
        public void SignalToStop( )
        {
            nestedVideoSource.SignalToStop( );
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks><para>Waits for video source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</para></remarks>
        ///
        public void WaitForStop( )
        {
            nestedVideoSource.WaitForStop( );
            Free( );
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops nested video source by calling its <see cref="IVideoSource.Stop"/> method.
        /// See documentation of the particular video source for additional details.</para></remarks>
        /// 
        public void Stop( )
        {
            nestedVideoSource.Stop( );
            Free( );
        }

        private void Free( )
        {
            if ( imageProcessingThread != null )
            {
                nestedVideoSource.NewFrame -= new NewFrameEventHandler( nestedVideoSource_NewFrame );

                // make sure processing thread does nothing
                isProcessingThreadAvailable.WaitOne( );
                // signal worker thread to stop and wait for it
                lastVideoFrame = null;
                isNewFrameAvailable.Set( );
                imageProcessingThread.Join( );
                imageProcessingThread = null;

                // release events
                isNewFrameAvailable.Close( );
                isNewFrameAvailable = null;

                isProcessingThreadAvailable.Close( );
                isProcessingThreadAvailable = null;
            }
        }

        // New frame from nested video source
        private void nestedVideoSource_NewFrame( object sender, NewFrameEventArgs eventArgs )
        {
            // don't even try doing something if there are no clients
            if ( NewFrame == null )
                return;

            if ( skipFramesIfBusy )
            {
                if ( !isProcessingThreadAvailable.WaitOne( 0, false ) )
                {
                    // return in the case if image processing thread is still busy and
                    // we are allowed to skip frames
                    return;
                }
            }
            else
            {
                // make sure image processing thread is available in the case we cannot skip frames
                isProcessingThreadAvailable.WaitOne( );
            }

            // pass the image to processing frame and exit
            lastVideoFrame = CloneImage( eventArgs.Frame );
            isNewFrameAvailable.Set( );
        }

        private void imageProcessingThread_Worker( )
        {
            while ( true )
            {
                // wait for new frame to process
                isNewFrameAvailable.WaitOne( );

                // if it is null, then we need to exit
                if ( lastVideoFrame == null )
                {
                    break;
                }

                if ( NewFrame != null )
                {
                    NewFrame( this, new NewFrameEventArgs( lastVideoFrame ) );
                }

                lastVideoFrame.Dispose( );
                lastVideoFrame = null;
                framesProcessed++;

                // we are free now for new image
                isProcessingThreadAvailable.Set( );
            }
        }

        // Note: image cloning is taken from AForge.Imaging.Image.Clone() to avoid reference,
        // which may be unwanted

        private static Bitmap CloneImage( Bitmap source )
        {
            // lock source bitmap data
            BitmapData sourceData = source.LockBits(
                new Rectangle( 0, 0, source.Width, source.Height ),
                ImageLockMode.ReadOnly, source.PixelFormat );

            // create new image
            Bitmap destination = CloneImage( sourceData );

            // unlock source image
            source.UnlockBits( sourceData );

            //
            if (
                ( source.PixelFormat == PixelFormat.Format1bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format4bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format8bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Indexed ) )
            {
                ColorPalette srcPalette = source.Palette;
                ColorPalette dstPalette = destination.Palette;

                int n = srcPalette.Entries.Length;

                // copy pallete
                for ( int i = 0; i < n; i++ )
                {
                    dstPalette.Entries[i] = srcPalette.Entries[i];
                }

                destination.Palette = dstPalette;
            }

            return destination;
        }

        private static Bitmap CloneImage( BitmapData sourceData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // create new image
            Bitmap destination = new Bitmap( width, height, sourceData.PixelFormat );

            // lock destination bitmap data
            BitmapData destinationData = destination.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, destination.PixelFormat );

            AForge.SystemTools.CopyUnmanagedMemory( destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride );

            // unlock destination image
            destination.UnlockBits( destinationData );

            return destination;
        }
    }
}
