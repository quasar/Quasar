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
	using System.IO;
    using System.Text;
    using System.Threading;
	using System.Net;
    using System.Security;

	/// <summary>
	/// JPEG video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source constantly downloads JPEG files from the specified URL.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create JPEG video source
    /// JPEGStream stream = new JPEGStream( "some url" );
    /// // set NewFrame event handler
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// stream.Start( );
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
    /// 
    /// <para><note>Some cameras produce HTTP header, which does not conform strictly to
    /// standard, what leads to .NET exception. To avoid this exception the <b>useUnsafeHeaderParsing</b>
    /// configuration option of <b>httpWebRequest</b> should be set, what may be done using application
    /// configuration file.</note></para>
    /// <code>
    /// &lt;configuration&gt;
    /// 	&lt;system.net&gt;
    /// 		&lt;settings&gt;
    /// 			&lt;httpWebRequest useUnsafeHeaderParsing="true" /&gt;
    /// 		&lt;/settings&gt;
    /// 	&lt;/system.net&gt;
    /// &lt;/configuration&gt;
    /// </code>
    /// </remarks>
    /// 
	public class JPEGStream : IVideoSource
	{
        // URL for JPEG files
		private string source;
        // login and password for HTTP authentication
		private string login = null;
		private string password = null;
        // proxy information
        private IWebProxy proxy = null;
        // received frames count
		private int framesReceived;
        // recieved byte count
		private long bytesReceived;
        // use separate HTTP connection group or use default
		private bool useSeparateConnectionGroup = false;
        // prevent cashing or not
		private bool preventCaching = true;
        // frame interval in milliseconds
		private int frameInterval = 0;
        // timeout value for web request
        private int requestTimeout = 10000;
        // if we should use basic authentication when connecting to the video source
        private bool forceBasicAuthentication = false;

        // buffer size used to download JPEG image
		private const int bufferSize = 1024 * 1024;
        // size of portion to read at once
		private const int readSize = 1024;		

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
        /// Use or not separate connection group.
        /// </summary>
        /// 
        /// <remarks>The property indicates to open web request in separate connection group.</remarks>
        /// 
		public bool SeparateConnectionGroup
		{
			get { return useSeparateConnectionGroup; }
			set { useSeparateConnectionGroup = value; }
		}

        /// <summary>
        /// Use or not caching.
        /// </summary>
        /// 
        /// <remarks>If the property is set to <b>true</b>, then a fake random parameter will be added
        /// to URL to prevent caching. It's required for clients, who are behind proxy server.</remarks>
        /// 
		public bool PreventCaching
		{
			get { return preventCaching; }
			set { preventCaching = value; }
		}

        /// <summary>
        /// Frame interval.
        /// </summary>
        /// 
        /// <remarks>The property sets the interval in milliseconds betwen frames. If the property is
        /// set to 100, then the desired frame rate will be 10 frames per second. Default value is 0 -
        /// get new frames as fast as possible.</remarks>
        /// 
		public int FrameInterval
		{
			get { return frameInterval; }
			set { frameInterval = value; }
		}

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>URL, which provides JPEG files.</remarks>
        /// 
        public virtual string Source
		{
			get { return source; }
			set { source = value; }
		}

        /// <summary>
        /// Login value.
        /// </summary>
        /// 
        /// <remarks>Login required to access video source.</remarks>
        /// 
		public string Login
		{
			get { return login; }
			set { login = value; }
		}

        /// <summary>
        /// Password value.
        /// </summary>
        /// 
        /// <remarks>Password required to access video source.</remarks>
        /// 
        public string Password
		{
			get { return password; }
			set { password = value; }
		}

        /// <summary>
        /// Gets or sets proxy information for the request.
        /// </summary>
        /// 
        /// <remarks><para>The local computer or application config file may specify that a default
        /// proxy to be used. If the Proxy property is specified, then the proxy settings from the Proxy
        /// property overridea the local computer or application config file and the instance will use
        /// the proxy settings specified. If no proxy is specified in a config file
        /// and the Proxy property is unspecified, the request uses the proxy settings
        /// inherited from Internet Explorer on the local computer. If there are no proxy settings
        /// in Internet Explorer, the request is sent directly to the server.
        /// </para></remarks>
        /// 
        public IWebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
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
        /// Request timeout value.
        /// </summary>
        /// 
        /// <remarks><para>The property sets timeout value in milliseconds for web requests.</para>
        /// 
        /// <para>Default value is set <b>10000</b> milliseconds.</para></remarks>
        /// 
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
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
        /// Force using of basic authentication when connecting to the video source.
        /// </summary>
        /// 
        /// <remarks><para>For some IP cameras (TrendNET IP cameras, for example) using standard .NET's authentication via credentials
        /// does not seem to be working (seems like camera does not request for authentication, but expects corresponding headers to be
        /// present on connection request). So this property allows to force basic authentication by adding required HTTP headers when
        /// request is sent.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool ForceBasicAuthentication
        {
            get { return forceBasicAuthentication; }
            set { forceBasicAuthentication = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPEGStream"/> class.
        /// </summary>
        /// 
        public JPEGStream( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPEGStream"/> class.
        /// </summary>
        /// 
        /// <param name="source">URL, which provides JPEG files.</param>
        /// 
        public JPEGStream( string source )
        {
            this.source = source;
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
                // check source
                if ( ( source == null ) || ( source == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified." );

				framesReceived = 0;
				bytesReceived = 0;

				// create events
				stopEvent = new ManualResetEvent( false );

                // create and start new thread
				thread = new Thread( new ThreadStart( WorkerThread ) );
				thread.Name = source; // mainly for debugging
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
            // buffer to read stream
			byte[] buffer = new byte[bufferSize];
            // HTTP web request
			HttpWebRequest request = null;
            // web responce
			WebResponse response = null;
            // stream for JPEG downloading
			Stream stream = null;
            // random generator to add fake parameter for cache preventing
			Random rand = new Random( (int) DateTime.Now.Ticks );
            // download start time and duration
			DateTime start;
			TimeSpan span;

            while ( !stopEvent.WaitOne( 0, false ) )
			{
				int	read, total = 0;

				try
				{
                    // set dowbload start time
					start = DateTime.Now;

					// create request
					if ( !preventCaching )
					{
                        // request without cache prevention
                        request = (HttpWebRequest) WebRequest.Create( source );
					}
					else
					{
                        // request with cache prevention
                        request = (HttpWebRequest) WebRequest.Create( source + ( ( source.IndexOf( '?' ) == -1 ) ? '?' : '&' ) + "fake=" + rand.Next( ).ToString( ) );
					}

                    // set proxy
                    if ( proxy != null )
                    {
                        request.Proxy = proxy;
                    }

                    // set timeout value for the request
                    request.Timeout = requestTimeout;
					// set login and password
					if ( ( login != null ) && ( password != null ) && ( login != string.Empty ) )
                        request.Credentials = new NetworkCredential( login, password );
					// set connection group name
					if ( useSeparateConnectionGroup )
                        request.ConnectionGroupName = GetHashCode( ).ToString( );
                    // force basic authentication through extra headers if required
                    if ( forceBasicAuthentication )
                    {
                        string authInfo = string.Format( "{0}:{1}", login, password );
                        authInfo = Convert.ToBase64String( Encoding.Default.GetBytes( authInfo ) );
                        request.Headers["Authorization"] = "Basic " + authInfo;
                    }
					// get response
                    response = request.GetResponse( );
					// get response stream
                    stream = response.GetResponseStream( );
                    stream.ReadTimeout = requestTimeout;

					// loop
					while ( !stopEvent.WaitOne( 0, false ) )
					{
						// check total read
						if ( total > bufferSize - readSize )
						{
							total = 0;
						}

						// read next portion from stream
						if ( ( read = stream.Read( buffer, total, readSize ) ) == 0 )
							break;

						total += read;

						// increment received bytes counter
						bytesReceived += read;
					}

					if ( !stopEvent.WaitOne( 0, false ) )
					{
						// increment frames counter
						framesReceived++;

						// provide new image to clients
						if ( NewFrame != null )
						{
							Bitmap bitmap = (Bitmap) Bitmap.FromStream( new MemoryStream( buffer, 0, total ) );
							// notify client
                            NewFrame( this, new NewFrameEventArgs( bitmap ) );
							// release the image
                            bitmap.Dispose( );
                            bitmap = null;
						}
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
				finally
				{
					// abort request
					if ( request != null)
					{
                        request.Abort( );
                        request = null;
					}
					// close response stream
					if ( stream != null )
					{
						stream.Close( );
						stream = null;
					}
					// close response
					if ( response != null )
					{
                        response.Close( );
                        response = null;
					}
				}

				// need to stop ?
				if ( stopEvent.WaitOne( 0, false ) )
					break;
			}

            if ( PlayingFinished != null )
            {
                PlayingFinished( this, ReasonToFinishPlaying.StoppedByUser );
            }
		}
	}
}
