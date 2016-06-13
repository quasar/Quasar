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
    /// This interface builds capture graphs and other custom filter graphs. 
    /// </summary>
    /// 
    [ComImport,
    Guid( "93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface ICaptureGraphBuilder2
    {
        /// <summary>
        /// Specify filter graph for the capture graph builder to use.
        /// </summary>
        /// 
        /// <param name="graphBuilder">Filter graph's interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetFiltergraph( [In] IGraphBuilder graphBuilder );

        /// <summary>
        /// Retrieve the filter graph that the builder is using.
        /// </summary>
        /// 
        /// <param name="graphBuilder">Filter graph's interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetFiltergraph( [Out] out IGraphBuilder graphBuilder );

        /// <summary>
        /// Create file writing section of the filter graph.
        /// </summary>
        /// 
        /// <param name="type">GUID that represents either the media subtype of the output or the
        /// class identifier (CLSID) of a multiplexer filter or file writer filter.</param>
        /// <param name="fileName">Output file name.</param>
        /// <param name="baseFilter">Receives the multiplexer's <see cref="IBaseFilter"/> interface.</param>
        /// <param name="fileSinkFilter">Receives the file writer's IFileSinkFilter interface. Can be NULL.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetOutputFileName(
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid type,
            [In, MarshalAs( UnmanagedType.LPWStr )] string fileName,
            [Out] out IBaseFilter baseFilter,
            [Out] out IntPtr fileSinkFilter
            );

        /// <summary>
        /// Searche the graph for a specified interface, starting from a specified filter.
        /// </summary>
        /// 
        /// <param name="category">GUID that specifies the search criteria.</param>
        /// <param name="type">GUID that specifies the major media type of an output pin, or NULL.</param>
        /// <param name="baseFilter"><see cref="IBaseFilter"/> interface of the filter. The method begins searching from this filter.</param>
        /// <param name="interfaceID">Interface identifier (IID) of the interface to locate.</param>
        /// <param name="retInterface">Receives found interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int FindInterface(
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid category,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid type,
            [In] IBaseFilter baseFilter,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid interfaceID ,
            [Out, MarshalAs( UnmanagedType.IUnknown )] out object retInterface
            );

        /// <summary>
        /// Connect an output pin on a source filter to a rendering filter, optionally through a compression filter.
        /// </summary>
        /// 
        /// <param name="category">Pin category.</param>
        /// <param name="mediaType">Major-type GUID that specifies the media type of the output pin.</param>
        /// <param name="source">Starting filter for the connection.</param>
        /// <param name="compressor">Interface of an intermediate filter, such as a compression filter. Can be NULL.</param>
        /// <param name="renderer">Sink filter, such as a renderer or mux filter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int RenderStream(
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid category,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid mediaType,
            [In, MarshalAs( UnmanagedType.IUnknown )] object source,
            [In] IBaseFilter compressor,
            [In] IBaseFilter renderer
            );

        /// <summary>
        /// Set the start and stop times for one or more streams of captured data.
        /// </summary>
        /// 
        /// <param name="category">Pin category.</param>
        /// <param name="mediaType">Major-type GUID that specifies the media type.</param>
        /// <param name="filter"><see cref="IBaseFilter"/> interface that specifies which filter to control.</param>
        /// <param name="start">Start time.</param>
        /// <param name="stop">Stop time.</param>
        /// <param name="startCookie">Value that is sent as the second parameter of the
        /// EC_STREAM_CONTROL_STARTED event notification.</param>
        /// <param name="stopCookie">Value that is sent as the second parameter of the
        /// EC_STREAM_CONTROL_STOPPED event notification.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ControlStream(
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid category,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid mediaType,
            [In, MarshalAs( UnmanagedType.Interface )] IBaseFilter filter,
            [In] long start,
            [In] long stop,
            [In] short startCookie,
            [In] short stopCookie
            );

        /// <summary>
        /// Preallocate a capture file to a specified size.
        /// </summary>
        /// 
        /// <param name="fileName">File name to create or resize.</param>
        /// <param name="size">Size of the file to allocate, in bytes.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AllocCapFile(
            [In, MarshalAs( UnmanagedType.LPWStr )] string fileName,
            [In] long size
            );

        /// <summary>
        /// Copy the valid media data from a capture file.
        /// </summary>
        /// 
        /// <param name="oldFileName">Old file name.</param>
        /// <param name="newFileName">New file name.</param>
        /// <param name="allowEscAbort">Boolean value that specifies whether pressing the ESC key cancels the copy operation.</param>
        /// <param name="callback">IAMCopyCaptureFileProgress interface to display progress information, or NULL.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int CopyCaptureFile(
            [In, MarshalAs( UnmanagedType.LPWStr )] string oldFileName,
            [In, MarshalAs( UnmanagedType.LPWStr )] string newFileName,
            [In, MarshalAs( UnmanagedType.Bool )] bool allowEscAbort,
            [In] IntPtr callback
            );

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="source">Interface on a filter, or to an interface on a pin.</param>
        /// <param name="pinDirection">Pin direction (input or output).</param>
        /// <param name="category">Pin category.</param>
        /// <param name="mediaType">Media type.</param>
        /// <param name="unconnected">Boolean value that specifies whether the pin must be unconnected.</param>
        /// <param name="index">Zero-based index of the pin to retrieve, from the set of matching pins.</param>
        /// <param name="pin">Interface of the matching pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int FindPin(
            [In, MarshalAs( UnmanagedType.IUnknown )] object source,
            [In] PinDirection pinDirection,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid category,
            [In, MarshalAs( UnmanagedType.LPStruct )] Guid mediaType,
            [In, MarshalAs( UnmanagedType.Bool )] bool unconnected,
            [In] int index,
            [Out, MarshalAs( UnmanagedType.Interface )] out IPin pin
            );
    }
}
