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
    /// The interface provides methods for controlling the flow of data through the filter graph.
    /// It includes methods for running, pausing, and stopping the graph.
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A868B1-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsDual )]
    internal interface IMediaControl
    {
        /// <summary>
        /// Runs all the filters in the filter graph.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Run( );

        /// <summary>
        /// Pauses all filters in the filter graph.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Pause( );

        /// <summary>
        /// Stops all the filters in the filter graph.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Stop( );

        /// <summary>
        /// Retrieves the state of the filter graph.
        /// </summary>
        /// 
        /// <param name="timeout">Duration of the time-out, in milliseconds, or INFINITE to specify an infinite time-out.</param>
        /// <param name="filterState">Ìariable that receives a member of the <b>FILTER_STATE</b> enumeration.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetState( int timeout, out int filterState );

        /// <summary>
        /// Builds a filter graph that renders the specified file.
        /// </summary>
        /// 
        /// <param name="fileName">Name of the file to render</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int RenderFile( string fileName );

        /// <summary>
        /// Adds a source filter to the filter graph, for a specified file.
        /// </summary>
        /// 
        /// <param name="fileName">Name of the file containing the source video.</param>
        /// <param name="filterInfo">Receives interface of filter information object.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AddSourceFilter( [In] string fileName, [Out, MarshalAs( UnmanagedType.IDispatch )] out object filterInfo );

        /// <summary>
        /// Retrieves a collection of the filters in the filter graph.
        /// </summary>
        /// 
        /// <param name="collection">Receives the <b>IAMCollection</b> interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_FilterCollection(
            [Out, MarshalAs( UnmanagedType.IDispatch )] out object collection );

        /// <summary>
        /// Retrieves a collection of all the filters listed in the registry.
        /// </summary>
        /// 
        /// <param name="collection">Receives the <b>IDispatch</b> interface of <b>IAMCollection</b> object.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_RegFilterCollection(
            [Out, MarshalAs( UnmanagedType.IDispatch )] out object collection );

        /// <summary>
        /// Pauses the filter graph, allowing filters to queue data, and then stops the filter graph.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int StopWhenReady( );
    }
}
