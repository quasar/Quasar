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
    /// The interface sets properties on the video window.
    /// </summary>
    /// 
	[ComImport,
	Guid("56A868B4-0AD4-11CE-B03A-0020AF0BA770"),
	InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IVideoWindow
	{
        /// <summary>
        /// Sets the video window caption.
        /// </summary>
        /// 
        /// <param name="caption">Caption.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Caption( string caption );

        /// <summary>
        /// Retrieves the video window caption.
        /// </summary>
        /// 
        /// <param name="caption">Caption.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Caption( [Out] out string caption );

        /// <summary>
        /// Sets the window style on the video window.
        /// </summary>
        /// 
        /// <param name="windowStyle">Window style flags.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_WindowStyle( int windowStyle );

        /// <summary>
        /// Retrieves the window style on the video window.
        /// </summary>
        /// 
        /// <param name="windowStyle">Window style flags.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_WindowStyle( out int windowStyle );

        /// <summary>
        /// Sets the extended window style on the video window.
        /// </summary>
        /// 
        /// <param name="windowStyleEx">Window extended style flags.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_WindowStyleEx( int windowStyleEx );

        /// <summary>
        /// Retrieves the extended window style on the video window.
        /// </summary>
        /// 
        /// <param name="windowStyleEx">Window extended style flags.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_WindowStyleEx( out int windowStyleEx );

        /// <summary>
        /// Specifies whether the video renderer automatically shows the video window when it receives video data.
        /// </summary>
        /// 
        /// <param name="autoShow">Specifies whether the video renderer automatically shows the video window.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_AutoShow( [In, MarshalAs( UnmanagedType.Bool )] bool autoShow );

        /// <summary>
        /// Queries whether the video renderer automatically shows the video window when it receives video data.
        /// </summary>
        /// 
        /// <param name="autoShow">REceives window auto show flag.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_AutoShow( [Out, MarshalAs( UnmanagedType.Bool )] out bool autoShow );

        /// <summary>
        /// Shows, hides, minimizes, or maximizes the video window.
        /// </summary>
        /// 
        /// <param name="windowState">Window state.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_WindowState( int windowState );

        /// <summary>
        /// Queries whether the video window is visible, hidden, minimized, or maximized.
        /// </summary>
        /// 
        /// <param name="windowState">Window state.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_WindowState( out int windowState );

        /// <summary>
        /// Specifies whether the video window realizes its palette in the background.
        /// </summary>
        /// 
        /// <param name="backgroundPalette">Value that specifies whether the video renderer realizes it palette in the background.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_BackgroundPalette( [In, MarshalAs( UnmanagedType.Bool )] bool backgroundPalette );

        /// <summary>
        /// Queries whether the video window realizes its palette in the background.
        /// </summary>
        /// 
        /// <param name="backgroundPalette">Receives state of background palette flag.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_BackgroundPalette( [Out, MarshalAs( UnmanagedType.Bool )] out bool backgroundPalette );

        /// <summary>
        /// Shows or hides the video window.
        /// </summary>
        /// 
        /// <param name="visible">Value that specifies whether to show or hide the window.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Visible( [In, MarshalAs( UnmanagedType.Bool )] bool visible );

        /// <summary>
        /// Queries whether the video window is visible.
        /// </summary>
        /// 
        /// <param name="visible">Visibility flag.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Visible( [Out, MarshalAs( UnmanagedType.Bool )] out bool visible );

        /// <summary>
        /// Sets the video window's x-coordinate.
        /// </summary>
        /// 
        /// <param name="left">Specifies the x-coordinate, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Left( int left );

        /// <summary>
        /// Retrieves the video window's x-coordinate.
        /// </summary>
        /// 
        /// <param name="left">x-coordinate, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Left( out int left );

        /// <summary>
        /// Sets the width of the video window.
        /// </summary>
        /// 
        /// <param name="width">Specifies the width, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Width( int width );

        /// <summary>
        /// Retrieves the width of the video window.
        /// </summary>
        /// 
        /// <param name="width">Width, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Width( out int width );

        /// <summary>
        /// Sets the video window's y-coordinate.
        /// </summary>
        /// 
        /// <param name="top">Specifies the y-coordinate, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Top( int top );

        /// <summary>
        /// Retrieves the video window's y-coordinate.
        /// </summary>
        /// 
        /// <param name="top">y-coordinate, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Top( out int top );

        /// <summary>
        /// Sets the height of the video window.
        /// </summary>
        /// 
        /// <param name="height">Specifies the height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Height( int height );

        /// <summary>
        /// Retrieves the height of the video window.
        /// </summary>
        /// 
        /// <param name="height">Height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Height( out int height );

        /// <summary>
        /// Specifies a parent window for the video windowþ
        /// </summary>
        /// 
        /// <param name="owner">Specifies a handle to the parent window.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_Owner( IntPtr owner );

        /// <summary>
        /// Retrieves the video window's parent window, if anyþ
        /// </summary>
        /// 
        /// <param name="owner">Parent window's handle.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_Owner( out IntPtr owner );

        /// <summary>
        /// Specifies a window to receive mouse and keyboard messages from the video window.
        /// </summary>
        /// 
        /// <param name="drain">Specifies a handle to the window.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_MessageDrain( IntPtr drain );

        /// <summary>
        /// Retrieves the window that receives mouse and keyboard messages from the video window, if any.
        /// </summary>
        /// 
        /// <param name="drain">Window's handle.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_MessageDrain( out IntPtr drain );

        /// <summary>
        /// Retrieves the color that appears around the edges of the destination rectangle.
        /// </summary>
        /// 
        /// <param name="color">Border's color.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_BorderColor( out int color );

        /// <summary>
        /// Sets the color that appears around the edges of the destination rectangle.
        /// </summary>
        /// 
        /// <param name="color">Specifies the border color.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_BorderColor( int color );

        /// <summary>
        /// Queries whether the video renderer is in full-screen mode.
        /// </summary>
        /// 
        /// <param name="fullScreenMode">Full-screen mode.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_FullScreenMode(
            [Out, MarshalAs( UnmanagedType.Bool )] out bool fullScreenMode );

        /// <summary>
        /// Enables or disables full-screen mode.
        /// </summary>
        /// 
        /// <param name="fullScreenMode">Boolean value that specifies whether to enable or disable full-screen mode.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int put_FullScreenMode( [In, MarshalAs( UnmanagedType.Bool )] bool fullScreenMode );

        /// <summary>
        /// Places the video window at the top of the Z order.
        /// </summary>
        /// 
        /// <param name="focus">Value that specifies whether to give the window focus.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetWindowForeground( int focus );

        /// <summary>
        /// Forwards a message to the video window.
        /// </summary>
        /// 
        /// <param name="hwnd">Handle to the window.</param>
        /// <param name="msg">Specifies the message.</param>
        /// <param name="wParam">Message parameter.</param>
        /// <param name="lParam">Message parameter.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int NotifyOwnerMessage( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam );

        /// <summary>
        /// Sets the position of the video windowþ
        /// </summary>
        /// 
        /// <param name="left">Specifies the x-coordinate, in pixels.</param>
        /// <param name="top">Specifies the y-coordinate, in pixels.</param>
        /// <param name="width">Specifies the width, in pixels.</param>
        /// <param name="height">Specifies the height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetWindowPosition( int left, int top, int width, int height );

        /// <summary>
        /// Retrieves the position of the video window.
        /// </summary>
        /// 
        /// <param name="left">x-coordinate, in pixels.</param>
        /// <param name="top">y-coordinate, in pixels.</param>
        /// <param name="width">Width, in pixels.</param>
        /// <param name="height">Height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetWindowPosition( out int left, out int top, out int width, out int height );

        /// <summary>
        /// Retrieves the minimum ideal size for the video image.
        /// </summary>
        /// 
        /// <param name="width">Receives the minimum ideal width, in pixels.</param>
        /// <param name="height">Receives the minimum ideal height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetMinIdealImageSize( out int width, out int height );

        /// <summary>
        /// Retrieves the maximum ideal size for the video image.
        /// </summary>
        /// 
        /// <param name="width">Receives the maximum ideal width, in pixels.</param>
        /// <param name="height">Receives the maximum ideal height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetMaxIdealImageSize( out int width, out int height );

        /// <summary>
        /// Retrieves the restored window position.
        /// </summary>
        /// 
        /// <param name="left">x-coordinate, in pixels.</param>
        /// <param name="top">y-coordinate, in pixels.</param>
        /// <param name="width">Width, in pixels.</param>
        /// <param name="height">Height, in pixels.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetRestorePosition( out int left, out int top, out int width, out int height );

        /// <summary>
        /// Hides the cursor.
        /// </summary>
        /// 
        /// <param name="hideCursor">Specifies whether to hide or display the cursor.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int HideCursor( [In, MarshalAs( UnmanagedType.Bool )] bool hideCursor );

        /// <summary>
        /// Queries whether the cursor is hidden.
        /// </summary>
        /// 
        /// <param name="hideCursor">Specifies if cursor is hidden or not.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int IsCursorHidden( [Out, MarshalAs( UnmanagedType.Bool )] out bool hideCursor );
    }
}
