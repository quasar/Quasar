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
    /// The <b>IPropertyBag</b> interface provides an object with a property bag in
    /// which the object can persistently save its properties. 
    /// </summary>
    /// 
    [ComImport,
    Guid( "55272A00-42CB-11CE-8135-00AA004BB851" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IPropertyBag
    {
        /// <summary>
        /// Read a property from property bag.
        /// </summary>
        /// 
        /// <param name="propertyName">Property name to read.</param>
        /// <param name="pVar">Property value.</param>
        /// <param name="pErrorLog">Caller's error log.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Read(
            [In, MarshalAs( UnmanagedType.LPWStr )] string propertyName,
            [In, Out, MarshalAs( UnmanagedType.Struct )] ref object pVar,
            [In] IntPtr pErrorLog );

        /// <summary>
        /// Write property to property bag.
        /// </summary>
        /// 
        /// <param name="propertyName">Property name to read.</param>
        /// <param name="pVar">Property value.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Write(
            [In, MarshalAs( UnmanagedType.LPWStr )] string propertyName,
            [In, MarshalAs( UnmanagedType.Struct )] ref object pVar );
    }
}
