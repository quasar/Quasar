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
    /// The interface inherits contains methods for retrieving event notifications and for overriding the
    /// filter graph's default handling of events.
    /// </summary>
    [ComVisible( true ), ComImport,
    Guid( "56a868c0-0ad4-11ce-b03a-0020af0ba770" ),
    InterfaceType( ComInterfaceType.InterfaceIsDual )]
    internal interface IMediaEventEx
    {
        /// <summary>
        /// Retrieves a handle to a manual-reset event that remains signaled while the queue contains event notifications.
        /// </summary>
        /// <param name="hEvent">Pointer to a variable that receives the event handle.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetEventHandle( out IntPtr hEvent );

        /// <summary>
        /// Retrieves the next event notification from the event queue.
        /// </summary>
        /// 
        /// <param name="lEventCode">Variable that receives the event code.</param>
        /// <param name="lParam1">Pointer to a variable that receives the first event parameter.</param>
        /// <param name="lParam2">Pointer to a variable that receives the second event parameter.</param>
        /// <param name="msTimeout">Time-out interval, in milliseconds.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetEvent( [Out, MarshalAs( UnmanagedType.I4 )] out DsEvCode lEventCode, [Out] out IntPtr lParam1, [Out] out IntPtr lParam2, int msTimeout );

        /// <summary>
        /// Waits for the filter graph to render all available data.
        /// </summary>
        /// 
        /// <param name="msTimeout">Time-out interval, in milliseconds. Pass zero to return immediately.</param>
        /// <param name="pEvCode">Pointer to a variable that receives an event code.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int WaitForCompletion( int msTimeout, [Out] out int pEvCode );

        /// <summary>
        /// Cancels the Filter Graph Manager's default handling for a specified event.
        /// </summary>
        /// 
        /// <param name="lEvCode">Event code for which to cancel default handling.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int CancelDefaultHandling( int lEvCode );

        /// <summary>
        /// Restores the Filter Graph Manager's default handling for a specified event.
        /// </summary>
        /// <param name="lEvCode">Event code for which to restore default handling.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int RestoreDefaultHandling( int lEvCode );

        /// <summary>
        /// Frees resources associated with the parameters of an event.
        /// </summary>
        /// <param name="lEvCode">Event code.</param>
        /// <param name="lParam1">First event parameter.</param>
        /// <param name="lParam2">Second event parameter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int FreeEventParams( [In, MarshalAs( UnmanagedType.I4 )] DsEvCode lEvCode, IntPtr lParam1, IntPtr lParam2 );

        /// <summary>
        /// Registers a window to process event notifications.
        /// </summary>
        /// 
        /// <param name="hwnd">Handle to the window, or <see cref="IntPtr.Zero"/> to stop receiving event messages.</param>
        /// <param name="lMsg">Window message to be passed as the notification.</param>
        /// <param name="lInstanceData">Value to be passed as the <i>lParam</i> parameter for the <i>lMsg</i> message.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetNotifyWindow( IntPtr hwnd, int lMsg, IntPtr lInstanceData );

        /// <summary>
        /// Enables or disables event notifications.
        /// </summary>
        /// 
        /// <param name="lNoNotifyFlags">Value indicating whether to enable or disable event notifications.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetNotifyFlags( int lNoNotifyFlags );

        /// <summary>
        /// Determines whether event notifications are enabled.
        /// </summary>
        /// 
        /// <param name="lplNoNotifyFlags">Variable that receives current notification status.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetNotifyFlags( out int lplNoNotifyFlags );
    }
}
