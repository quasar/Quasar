// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@gmail.com
//
// Written by Jeremy Noring 
// kidjan@gmail.com
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
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid( "56a86899-0ad4-11ce-b03a-0020af0ba770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IMediaFilter : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID(
            [Out] out Guid pClassID );

        #endregion

        /// <summary>
        /// This method informs the filter to transition to the new state. 
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Stop( );

        /// <summary>
        /// This method informs the filter to transition to the new state. 
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Pause( );

        /// <summary>
        /// This method informs the filter to transition to the new (running) state. Passes a time value to synchronize independent streams. 
        /// </summary>
        /// 
        /// <param name="tStart">Time value of the reference clock.  The amount to be added to the IMediaSample  time stamp to determine the time at which that sample should be rendered according to the reference clock. That is, it is the reference time at which a sample with a stream time of zero should be rendered.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Run( [In] long tStart );

        /// <summary>
        /// This method determines the filter's state. 
        /// </summary>
        /// 
        /// <param name="dwMilliSecsTimeout">Duration of the time-out, in milliseconds. To block indefinitely, pass INFINITE. </param>
        /// <param name="filtState">Returned state of the filter. States include stopped, paused, running, or intermediate (in the process of changing). </param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetState(
            [In] int dwMilliSecsTimeout,
            [Out] out FilterState filtState );

        /// <summary>
        /// This method identifies the reference clock to which the filter should synchronize activity.
        /// </summary>
        /// 
        /// <param name="pClock">Pointer to the IReferenceClock  interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetSyncSource( [In] IReferenceClock pClock );

        /// <summary>
        /// This method retrieves the current reference clock in use by this filter. 
        /// </summary>
        /// 
        /// <param name="pClock">Pointer to a reference clock; it will be set to the IReferenceClock  interface. </param>
        /// 
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetSyncSource( [Out] out IReferenceClock pClock );
    }
}

