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
    /// The IReferenceClock interface provides the reference time for the filter graph.
    ///
    /// Filters that can act as a reference clock can expose this interface. It is also exposed by the System Reference Clock. 
    /// The filter graph manager uses this interface to synchronize the filter graph. Applications can use this interface to 
    /// retrieve the current reference time, or to request notification of an elapsed time.
    /// </summary>
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid( "56a86897-0ad4-11ce-b03a-0020af0ba770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IReferenceClock
    {
        /// <summary>
        /// The GetTime method retrieves the current reference time.
        /// </summary>
        /// 
        /// <param name="pTime">Pointer to a variable that receives the current time, in 100-nanosecond units.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetTime( [Out] out long pTime );

        /// <summary>
        /// The AdviseTime method creates a one-shot advise request.
        /// </summary>
        /// 
        /// <param name="baseTime">Base reference time, in 100-nanosecond units. See Remarks.</param>
        /// <param name="streamTime">Stream offset time, in 100-nanosecond units. See Remarks.</param>
        /// <param name="hEvent">Handle to an event, created by the caller.</param>
        /// <param name="pdwAdviseCookie">Pointer to a variable that receives an identifier for the advise request.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AdviseTime(
            [In] long baseTime,
            [In] long streamTime,
            [In] IntPtr hEvent,
            [Out] out int pdwAdviseCookie );

        /// <summary>
        /// The AdvisePeriodic method creates a periodic advise request.
        /// </summary>
        /// 
        /// <param name="startTime">Time of the first notification, in 100-nanosecond units. Must be greater than zero and less than MAX_TIME.</param>
        /// <param name="periodTime">Time between notifications, in 100-nanosecond units. Must be greater than zero.</param>
        /// <param name="hSemaphore">Handle to a semaphore, created by the caller.</param>
        /// <param name="pdwAdviseCookie">Pointer to a variable that receives an identifier for the advise request.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int AdvisePeriodic(
            [In] long startTime,
            [In] long periodTime,
            [In] IntPtr hSemaphore,
            [Out] out int pdwAdviseCookie );

        /// <summary>
        /// The Unadvise method removes a pending advise request.
        /// </summary>
        /// 
        /// <param name="dwAdviseCookie">Identifier of the request to remove. Use the value returned by IReferenceClock::AdviseTime  or IReferenceClock::AdvisePeriodic  in the pdwAdviseToken parameter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Unadvise( [In] int dwAdviseCookie );
    }
}
