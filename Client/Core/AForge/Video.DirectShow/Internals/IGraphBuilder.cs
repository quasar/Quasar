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
    /// This interface provides methods that enable an application to build a filter graph.
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A868A9-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IGraphBuilder
    {
        // --- IFilterGraph Methods
        
        /// <summary>
        /// Adds a filter to the graph and gives it a name.
        /// </summary>
        /// 
        /// <param name="filter">Filter to add to the graph.</param>
        /// <param name="name">Name of the filter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AddFilter( [In] IBaseFilter filter, [In, MarshalAs( UnmanagedType.LPWStr )] string name );

        /// <summary>
        /// Removes a filter from the graph.
        /// </summary>
        /// 
        /// <param name="filter">Filter to be removed from the graph.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int RemoveFilter( [In] IBaseFilter filter );

        /// <summary>
        /// Provides an enumerator for all filters in the graph.
        /// </summary>
        /// 
        /// <param name="enumerator">Filter enumerator.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int EnumFilters( [Out] out IEnumFilters enumerator );

        /// <summary>
        /// Finds a filter that was added with a specified name.
        /// </summary>
        /// 
        /// <param name="name">Name of filter to search for.</param>
        /// <param name="filter">Interface of found filter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int FindFilterByName( [In, MarshalAs( UnmanagedType.LPWStr )] string name, [Out] out IBaseFilter filter );

        /// <summary>
        /// Connects two pins directly (without intervening filters).
        /// </summary>
        /// 
        /// <param name="pinOut">Output pin.</param>
        /// <param name="pinIn">Input pin.</param>
        /// <param name="mediaType">Media type to use for the connection.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ConnectDirect( [In] IPin pinOut, [In] IPin pinIn, [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Breaks the existing pin connection and reconnects it to the same pin.
        /// </summary>
        /// 
        /// <param name="pin">Pin to disconnect and reconnect.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Reconnect( [In] IPin pin );

        /// <summary>
        /// Disconnects a specified pin.
        /// </summary>
        /// 
        /// <param name="pin">Pin to disconnect.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Disconnect( [In] IPin pin );

        /// <summary>
        /// Sets the reference clock to the default clock.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetDefaultSyncSource( );

        // --- IGraphBuilder methods
        
        /// <summary>
        /// Connects two pins. If they will not connect directly, this method connects them with intervening transforms.
        /// </summary>
        /// 
        /// <param name="pinOut">Output pin.</param>
        /// <param name="pinIn">Input pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Connect( [In] IPin pinOut, [In] IPin pinIn );

        /// <summary>
        /// Adds a chain of filters to a specified output pin to render it.
        /// </summary>
        /// 
        /// <param name="pinOut">Output pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Render( [In] IPin pinOut );

        /// <summary>
        /// Builds a filter graph that renders the specified file.
        /// </summary>
        /// 
        /// <param name="file">Specifies a string that contains file name or device moniker.</param>
        /// <param name="playList">Reserved.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int RenderFile(
            [In, MarshalAs( UnmanagedType.LPWStr )] string file,
            [In, MarshalAs( UnmanagedType.LPWStr )] string playList);

        /// <summary>
        /// Adds a source filter to the filter graph for a specific file.
        /// </summary>
        /// 
        /// <param name="fileName">Specifies the name of the file to load.</param>
        /// <param name="filterName">Specifies a name for the source filter.</param>
        /// <param name="filter">Variable that receives the interface of the source filter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AddSourceFilter(
            [In, MarshalAs( UnmanagedType.LPWStr )] string fileName,
            [In, MarshalAs( UnmanagedType.LPWStr )] string filterName,
            [Out] out IBaseFilter filter );

        /// <summary>
        /// Sets the file for logging actions taken when attempting to perform an operation.
        /// </summary>
        /// 
        /// <param name="hFile">Handle to the log file.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetLogFile( IntPtr hFile );

        /// <summary>
        /// Requests that the graph builder return as soon as possible from its current task.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Abort( );

        /// <summary>
        /// Queries whether the current operation should continue.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int ShouldOperationContinue( );
    }
}
