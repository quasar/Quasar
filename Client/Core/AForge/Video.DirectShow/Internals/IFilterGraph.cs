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
    /// The interface provides methods for building a filter graph. An application can use it to add filters to
    /// the graph, connect or disconnect filters, remove filters, and perform other basic operations. 
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A8689F-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IFilterGraph
    {
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
        int EnumFilters( [Out] out IntPtr enumerator );

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
    }
}
