// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The interface controls certain video capture operations such as enumerating available
    /// frame rates and image orientation.
    /// </summary>
    /// 
    [ComImport,
    Guid( "6A2E0670-28E4-11D0-A18c-00A0C9118956" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IAMVideoControl
    {
        /// <summary>
        /// Retrieves the capabilities of the underlying hardware.
        /// </summary>
        /// 
        /// <param name="pin">Pin to query capabilities from.</param>
        /// <param name="flags">Get capabilities of the specified pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCaps( [In] IPin pin, [Out, MarshalAs( UnmanagedType.I4 )] out VideoControlFlags flags );

        /// <summary>
        /// Sets the video control mode of operation.
        /// </summary>
        /// 
        /// <param name="pin">The pin to set the video control mode on.</param>
        /// <param name="mode">Value specifying a combination of the flags to set the video control mode.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetMode( [In] IPin pin, [In, MarshalAs( UnmanagedType.I4 )] VideoControlFlags mode );

        /// <summary>
        /// Retrieves the video control mode of operation.
        /// </summary>
        /// 
        /// <param name="pin">The pin to retrieve the video control mode from.</param>
        /// <param name="mode">Gets combination of flags, which specify the video control mode.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetMode( [In] IPin pin, [Out, MarshalAs( UnmanagedType.I4 )] out VideoControlFlags mode );

        /// <summary>
        /// The method retrieves the actual frame rate, expressed as a frame duration in 100-nanosecond units.
        /// USB (Universal Serial Bus) and IEEE 1394 cameras may provide lower frame rates than requested
        /// because of bandwidth availability. This is only available during video streaming.
        /// </summary>
        /// 
        /// <param name="pin">The pin to retrieve the frame rate from.</param>
        /// <param name="actualFrameRate">Gets frame rate in frame duration in 100-nanosecond units.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCurrentActualFrameRate( [In] IPin pin, [Out, MarshalAs( UnmanagedType.I8 )] out long actualFrameRate );
        
        /// <summary>
        /// Retrieves the maximum frame rate currently available based on bus bandwidth usage for connections
        /// such as USB and IEEE 1394 camera devices where the maximum frame rate can be limited by bandwidth
        /// availability.
        /// </summary>
        /// 
        /// <param name="pin">The pin to retrieve the maximum frame rate from.</param>
        /// <param name="index">Index of the format to query for maximum frame rate. This index corresponds
        /// to the order in which formats are enumerated by <see cref="IAMStreamConfig.GetStreamCaps"/>.</param>
        /// <param name="dimensions">Frame image size (width and height) in pixels.</param>
        /// <param name="maxAvailableFrameRate">Gets maximum available frame rate. The frame rate is expressed as frame duration in 100-nanosecond units.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetMaxAvailableFrameRate( [In] IPin pin, [In] int index, 
            [In] System.Drawing.Size dimensions,
            [Out] out long maxAvailableFrameRate );

        /// <summary>
        /// Retrieves a list of available frame rates.
        /// </summary>
        /// 
        /// <param name="pin">The pin to retrieve the maximum frame rate from.</param>
        /// <param name="index">Index of the format to query for maximum frame rate. This index corresponds
        /// to the order in which formats are enumerated by <see cref="IAMStreamConfig.GetStreamCaps"/>.</param>
        /// <param name="dimensions">Frame image size (width and height) in pixels.</param>
        /// <param name="listSize">Number of elements in the list of frame rates.</param>
        /// <param name="frameRate">Array of frame rates in 100-nanosecond units.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetFrameRateList( [In] IPin pin, [In] int index,
            [In] System.Drawing.Size dimensions,
            [Out] out int listSize,
            [Out] out IntPtr frameRate );
    }
}
