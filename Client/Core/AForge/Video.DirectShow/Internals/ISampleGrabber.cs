// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The interface is exposed by the Sample Grabber Filter. It enables an application to retrieve
    /// individual media samples as they move through the filter graph.
    /// </summary>
    /// 
	[ComImport,
	Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabber
	{
        /// <summary>
        /// Specifies whether the filter should stop the graph after receiving one sample.
        /// </summary>
        /// 
        /// <param name="oneShot">Boolean value specifying whether the filter should stop the graph after receiving one sample.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetOneShot( [In, MarshalAs( UnmanagedType.Bool )] bool oneShot );

        /// <summary>
        /// Specifies the media type for the connection on the Sample Grabber's input pin.
        /// </summary>
        /// 
        /// <param name="mediaType">Specifies the required media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetMediaType( [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Retrieves the media type for the connection on the Sample Grabber's input pin.
        /// </summary>
        /// 
        /// <param name="mediaType"><see cref="AMMediaType"/> structure, which receives media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetConnectedMediaType( [Out, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Specifies whether to copy sample data into a buffer as it goes through the filter.
        /// </summary>
        /// 
        /// <param name="bufferThem">Boolean value specifying whether to buffer sample data.
        /// If <b>true</b>, the filter copies sample data into an internal buffer.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetBufferSamples( [In, MarshalAs( UnmanagedType.Bool )] bool bufferThem );

        /// <summary>
        /// Retrieves a copy of the sample that the filter received most recently.
        /// </summary>
        /// 
        /// <param name="bufferSize">Pointer to the size of the buffer. If pBuffer is NULL, this parameter receives the required size.</param>
        /// <param name="buffer">Pointer to a buffer to receive a copy of the sample, or NULL.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCurrentBuffer( ref int bufferSize, IntPtr buffer );

        /// <summary>
        /// Not currently implemented.
        /// </summary>
        /// 
        /// <param name="sample"></param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCurrentSample( IntPtr sample );

        /// <summary>
        /// Specifies a callback method to call on incoming samples.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="ISampleGrabberCB"/> interface containing the callback method, or NULL to cancel the callback.</param>
        /// <param name="whichMethodToCallback">Index specifying the callback method.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetCallback( ISampleGrabberCB callback, int whichMethodToCallback );
    }
}
