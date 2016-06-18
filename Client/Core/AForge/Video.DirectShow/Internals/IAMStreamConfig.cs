// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2008
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This interface sets the output format on certain capture and compression filters,
    /// for both audio and video.
    /// </summary>
    /// 
    [ComImport,
    Guid( "C6E13340-30AC-11d0-A18C-00A0C9118956" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IAMStreamConfig
    {
        /// <summary>
        /// Set the output format on the pin.
        /// </summary>
        /// 
        /// <param name="mediaType">Media type to set.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetFormat( [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Retrieves the audio or video stream's format.
        /// </summary>
        /// 
        /// <param name="mediaType">Retrieved media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetFormat( [Out, MarshalAs( UnmanagedType.LPStruct )] out AMMediaType mediaType );

        /// <summary>
        /// Retrieve the number of format capabilities that this pin supports.
        /// </summary>
        /// 
        /// <param name="count">Variable that receives the number of format capabilities.</param>
        /// <param name="size">Variable that receives the size of the configuration structure in bytes.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetNumberOfCapabilities( out int count, out int size );

        /// <summary>
        /// Retrieve a set of format capabilities.
        /// </summary>
        /// 
        /// <param name="index">Specifies the format capability to retrieve, indexed from zero.</param>
        /// <param name="mediaType">Retrieved media type.</param>
        /// <param name="streamConfigCaps">Byte array, which receives information about capabilities.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetStreamCaps(
            [In] int index,
            [Out, MarshalAs( UnmanagedType.LPStruct )] out AMMediaType mediaType,
            [In, MarshalAs( UnmanagedType.LPStruct )] VideoStreamConfigCaps streamConfigCaps
            );
    }
}
