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
    /// This interface is exposed by all input and output pins of DirectShow filters.
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A86891-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IPin
    {
        /// <summary>
        /// Connects the pin to another pin.
        /// </summary>
        /// 
        /// <param name="receivePin">Other pin to connect to.</param>
        /// <param name="mediaType">Type to use for the connections (optional).</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Connect( [In] IPin receivePin, [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Makes a connection to this pin and is called by a connecting pin.
        /// </summary>
        /// 
        /// <param name="receivePin">Connecting pin.</param>
        /// <param name="mediaType">Media type of the samples to be streamed.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ReceiveConnection( [In] IPin receivePin, [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Breaks the current pin connection.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Disconnect( );

        /// <summary>
        /// Returns a pointer to the connecting pin.
        /// </summary>
        /// 
        /// <param name="pin">Receives <b>IPin</b> interface of connected pin (if any).</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ConnectedTo( [Out] out IPin pin );

        /// <summary>
        /// Returns the media type of this pin's connection.
        /// </summary>
        /// 
        /// <param name="mediaType">Pointer to an <see cref="AMMediaType"/> structure. If the pin is connected,
        /// the media type is returned. Otherwise, the structure is initialized to a default state in which
        /// all elements are 0, with the exception of <b>lSampleSize</b>, which is set to 1, and
        /// <b>FixedSizeSamples</b>, which is set to <b>true</b>.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ConnectionMediaType( [Out, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Retrieves information about this pin (for example, the name, owning filter, and direction).
        /// </summary>
        /// 
        /// <param name="pinInfo"><see cref="PinInfo"/> structure that receives the pin information.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryPinInfo( [Out] out PinInfo pinInfo );

        /// <summary>
        /// Retrieves the direction for this pin.
        /// </summary>
        /// 
        /// <param name="pinDirection">Receives direction of the pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryDirection( out PinDirection pinDirection );

        /// <summary>
        /// Retrieves an identifier for the pin.
        /// </summary>
        /// 
        /// <param name="id">Pin identifier.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryId( [Out, MarshalAs( UnmanagedType.LPWStr )] out string id );

        /// <summary>
        /// Queries whether a given media type is acceptable by the pin.
        /// </summary>
        /// 
        /// <param name="mediaType"><see cref="AMMediaType"/> structure that specifies the media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryAccept( [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Provides an enumerator for this pin's preferred media types.
        /// </summary>
        /// 
        /// <param name="enumerator">Address of a variable that receives a pointer to the <b>IEnumMediaTypes</b> interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int EnumMediaTypes( IntPtr enumerator );

        /// <summary>
        /// Provides an array of the pins to which this pin internally connects.
        /// </summary>
        /// 
        /// <param name="apPin">Address of an array of <b>IPin</b> pointers.</param>
        /// <param name="nPin">On input, specifies the size of the array. When the method returns,
        /// the value is set to the number of pointers returned in the array.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryInternalConnections( IntPtr apPin, [In, Out] ref int nPin );

        /// <summary>
        /// Notifies the pin that no additional data is expected.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int EndOfStream( );

        /// <summary>
        /// Begins a flush operation.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int BeginFlush( );

        /// <summary>
        /// Ends a flush operation.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int EndFlush( );

        /// <summary>
        /// Specifies that samples following this call are grouped as a segment with a given start time, stop time, and rate.
        /// </summary>
        /// 
        /// <param name="start">Start time of the segment, relative to the original source, in 100-nanosecond units.</param>
        /// <param name="stop">End time of the segment, relative to the original source, in 100-nanosecond units.</param>
        /// <param name="rate">Rate at which this segment should be processed, as a percentage of the original rate.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int NewSegment(
            long start,
            long stop,
            double rate );
    }
}
