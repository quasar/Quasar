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
    /// The IBaseFilter interface provides methods for controlling a filter.
    /// All DirectShow filters expose this interface
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A86895-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IBaseFilter
    {
        // --- IPersist Methods

        /// <summary>
        /// Returns the class identifier (CLSID) for the component object.
        /// </summary>
        /// 
        /// <param name="ClassID">Points to the location of the CLSID on return.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetClassID( [Out] out Guid ClassID );

        // --- IMediaFilter Methods

        /// <summary>
        /// Stops the filter.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Stop( );

        /// <summary>
        /// Pauses the filter.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Pause( );

        /// <summary>
        /// Runs the filter.
        /// </summary>
        /// 
        /// <param name="start">Reference time corresponding to stream time 0.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Run( long start );

        /// <summary>
        /// Retrieves the state of the filter (running, stopped, or paused).
        /// </summary>
        /// 
        /// <param name="milliSecsTimeout">Time-out interval, in milliseconds.</param>
        /// <param name="filterState">Pointer to a variable that receives filter's state.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetState( int milliSecsTimeout, [Out] out int filterState );

        /// <summary>
        /// Sets the reference clock for the filter or the filter graph.
        /// </summary>
        /// 
        /// <param name="clock">Pointer to the clock's <b>IReferenceClock</b> interface, or NULL. </param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetSyncSource( [In] IntPtr clock );

        /// <summary>
        /// Retrieves the current reference clock.
        /// </summary>
        /// 
        /// <param name="clock">Address of a variable that receives a pointer to the clock's IReferenceClock interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetSyncSource( [Out] out IntPtr clock );

        // --- IBaseFilter Methods

        /// <summary>
        /// Enumerates the pins on this filter.
        /// </summary>
        /// 
        /// <param name="enumPins">Address of a variable that receives a pointer to the IEnumPins interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int EnumPins( [Out] out IEnumPins enumPins );

        /// <summary>
        /// Retrieves the pin with the specified identifier.
        /// </summary>
        /// 
        /// <param name="id">Pointer to a constant wide-character string that identifies the pin.</param>
        /// <param name="pin">Address of a variable that receives a pointer to the pin's IPin interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int FindPin( [In, MarshalAs( UnmanagedType.LPWStr )] string id, [Out] out IPin pin );

        /// <summary>
        /// Retrieves information about the filter.
        /// </summary>
        /// 
        /// <param name="filterInfo">Pointer to <b>FilterInfo</b> structure.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryFilterInfo( [Out] out FilterInfo filterInfo );

        /// <summary>
        /// Notifies the filter that it has joined or left the filter graph.
        /// </summary>
        /// 
        /// <param name="graph">Pointer to the Filter Graph Manager's <b>IFilterGraph</b> interface, or NULL
        /// if the filter is leaving the graph.</param>
        /// <param name="name">String that specifies a name for the filter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int JoinFilterGraph( [In] IFilterGraph graph, [In, MarshalAs( UnmanagedType.LPWStr )] string name );

        /// <summary>
        /// Retrieves a string containing vendor information.
        /// </summary>
        /// 
        /// <param name="vendorInfo">Receives a string containing the vendor information.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int QueryVendorInfo( [Out, MarshalAs( UnmanagedType.LPWStr )] out string vendorInfo );
    }
}
