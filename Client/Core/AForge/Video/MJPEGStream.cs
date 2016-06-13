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
    /// MJPEG video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source downloads JPEG images from the specified URL, which represents
    /// MJPEG stream.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create MJPEG video source
    /// MJPEGStream stream = new MJPEGStream( "some url" );
    /// // set event handlers
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// stream.Start( );
    /// // ...
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
    public class MJPEGStream : IVideoSource
	{
        // URL for MJPEG stream
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
        private bool useSeparateConnectionGroup = true;
        // timeout value for web request
        private int requestTimeout = 10000;
        // if we should use basic authentication when connecting to the video source
        private bool forceBasicAuthentication = false;

        // buffer size used to download MJPEG stream
        private const int bufSize = 1024 * 1024;
        // size of portion to read at once
        private const int readSize = 1024;

		private Thread	thread = null;
		private ManualResetEvent stopEvent = null;
		private ManualResetEvent reloadEvent = null;

        private string userAgent = "Mozilla/5.0";

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
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>URL, which provides MJPEG stream.</remarks>
        /// 
        public string Source
		{
			get { return source; }
			set
			{
				source = value;
				// signal to reload
				if ( thread != null )
					reloadEvent.Set( );
			}
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
        /// User agent to specify in HTTP request header.
        /// </summary>
        /// 
        /// <remarks><para>Some IP cameras check what is the requesting user agent and depending
        /// on it they provide video in different formats or do not provide it at all. The property
        /// sets the value of user agent string, which is sent to camera in request header.
        /// </para>
        /// 
        /// <para>Default value is set to "Mozilla/5.0". If the value is set to <see langword="null"/>,
        /// the user agent string is not sent in request header.</para>
        /// </remarks>
        /// 
        public string HttpUserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
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
        /// <remarks>The property sets timeout value in milliseconds for web requests.
        /// Default value is 10000 milliseconds.</remarks>
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

					// the thread is not running, so free resources
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
        /// Initializes a new instance of the <see cref="MJPEGStream"/> class.
        /// </summary>
        /// 
        public MJPEGStream( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MJPEGStream"/> class.
        /// </summary>
        /// 
        /// <param name="source">URL, which provides MJPEG stream.</param>
        /// 
        public MJPEGStream( string source )
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
				stopEvent	= new ManualResetEvent( false );
				reloadEvent	= new ManualResetEvent( false );

				// create and start new thread
				thread = new Thread( new ThreadStart( WorkerThread ) );
				thread.Name = source;
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
			reloadEvent.Close( );
			reloadEvent = null;
		}

        // Worker thread
        private void WorkerThread( )
		{
            // buffer to read stream
            byte[] buffer = new byte[bufSize];
            // JPEG magic number
            byte[] jpegMagic = new byte[] { 0xFF, 0xD8, 0xFF };
            int jpegMagicLength = 3;

            ASCIIEncoding encoding = new ASCIIEncoding( );

            while ( !stopEvent.WaitOne( 0, false ) )
			{
				// reset reload event
				reloadEvent.Reset( );

                // HTTP web request
				HttpWebRequest request = null;
                // web responce
				WebResponse response = null;
                // stream for MJPEG downloading
                Stream stream = null;
                // boundary betweeen images (string and binary versions)
				byte[] boundary = null;
                string boudaryStr = null;
                // length of boundary
				int boundaryLen;
                // flag signaling if boundary was checked or not
                bool boundaryIsChecked = false;
                // read amounts and positions
				int read, todo = 0, total = 0, pos = 0, align = 1;
				int start = 0, stop = 0;

				// align
				//  1 = searching for image start
				//  2 = searching for image end

				try
				{
					// create request
                    request = (HttpWebRequest) WebRequest.Create( source );
                    // set user agent
                    if ( userAgent != null )
                    {
                        request.UserAgent = userAgent;
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

					// check content type
                    string contentType = response.ContentType;
                    string[] contentTypeArray = contentType.Split( '/' );

                    // "application/octet-stream"
                    if ( ( contentTypeArray[0] == "application" ) && ( contentTypeArray[1] == "octet-stream" ) )
                    {
                        boundaryLen = 0;
                        boundary = new byte[0];
                    }
                    else if ( ( contentTypeArray[0] == "multipart" ) && ( contentType.Contains( "mixed" ) ) )
                    {
                        // get boundary
                        int boundaryIndex = contentType.IndexOf( "boundary", 0 );
                        if ( boundaryIndex != -1 )
                        {
                            boundaryIndex = contentType.IndexOf( "=", boundaryIndex + 8 );
                        }

                        if ( boundaryIndex == -1 )
                        {
                            // try same scenario as with octet-stream, i.e. without boundaries
                            boundaryLen = 0;
                            boundary = new byte[0];
                        }
                        else
                        {
                            boudaryStr = contentType.Substring( boundaryIndex + 1 );
                            // remove spaces and double quotes, which may be added by some IP cameras
                            boudaryStr = boudaryStr.Trim( ' ', '"' );

                            boundary = encoding.GetBytes( boudaryStr );
                            boundaryLen = boundary.Length;
                            boundaryIsChecked = false;
                        }
                    }
                    else
                    {
                        throw new Exception( "Invalid content type." );
                    }

					// get response stream
                    stream = response.GetResponseStream( );
                    stream.ReadTimeout = requestTimeout;

					// loop
					while ( ( !stopEvent.WaitOne( 0, false ) ) && ( !reloadEvent.WaitOne( 0, false ) ) )
					{
						// check total read
						if ( total > bufSize - readSize )
						{
							total = pos = todo = 0;
						}

						// read next portion from stream
						if ( ( read = stream.Read( buffer, total, readSize ) ) == 0 )
							throw new ApplicationException( );

						total += read;
						todo += read;

						// increment received bytes counter
						bytesReceived += read;

                        // do we need to check boundary ?
                        if ( ( boundaryLen != 0 ) && ( !boundaryIsChecked ) )
                        {
                            // some IP cameras, like AirLink, claim that boundary is "myboundary",
                            // when it is really "--myboundary". this needs to be corrected.

                            pos = ByteArrayUtils.Find( buffer, boundary, 0, todo );
                            // continue reading if boudary was not found
                            if ( pos == -1 )
                                continue;

                            for ( int i = pos - 1; i >= 0; i-- )
                            {
                                byte ch = buffer[i];

                                if ( ( ch == (byte) '\n' ) || ( ch == (byte) '\r' ) )
                                {
                                    break;
                                }

                                boudaryStr = (char) ch + boudaryStr;
                            }

                            boundary = encoding.GetBytes( boudaryStr );
                            boundaryLen = boundary.Length;
                            boundaryIsChecked = true;
                        }
				
						// search for image start
						if ( ( align == 1 ) && ( todo >= jpegMagicLength ) )
						{
							start = ByteArrayUtils.Find( buffer, jpegMagic, pos, todo );
							if ( start != -1 )
							{
								// found JPEG start
								pos		= start + jpegMagicLength;
								todo	= total - pos;
								align	= 2;
							}
							else
							{
								// delimiter not found
                                todo    = jpegMagicLength - 1;
								pos		= total - todo;
							}
						}

                        // search for image end ( boundaryLen can be 0, so need extra check )
						while ( ( align == 2 ) && ( todo != 0 ) && ( todo >= boundaryLen ) )
						{
							stop = ByteArrayUtils.Find( buffer,
                                ( boundaryLen != 0 ) ? boundary : jpegMagic,
                                pos, todo );

							if ( stop != -1 )
							{
								pos		= stop;
								todo	= total - pos;

								// increment frames counter
								framesReceived ++;

								// image at stop
								if ( ( NewFrame != null ) && ( !stopEvent.WaitOne( 0, false ) ) )
								{
									Bitmap bitmap = (Bitmap) Bitmap.FromStream ( new MemoryStream( buffer, start, stop - start ) );
									// notify client
                                    NewFrame( this, new NewFrameEventArgs( bitmap ) );
									// release the image
                                    bitmap.Dispose( );
                                    bitmap = null;
								}

								// shift array
								pos		= stop + boundaryLen;
								todo	= total - pos;
								Array.Copy( buffer, pos, buffer, 0, todo );

								total	= todo;
								pos		= 0;
								align	= 1;
							}
							else
							{
								// boundary not found
                                if ( boundaryLen != 0 )
                                {
                                    todo = boundaryLen - 1;
                                    pos = total - todo;
                                }
                                else
                                {
                                    todo = 0;
                                    pos = total;
                                }
							}
						}
					}
				}
				catch ( ApplicationException )
				{
                    // do nothing for Application Exception, which we raised on our own
                    // wait for a while before the next try
                    Thread.Sleep( 250 );
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
