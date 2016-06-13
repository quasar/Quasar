// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This interface is used by applications or other filters to determine
    /// what filters exist in the filter graph.
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A86893-0AD4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IEnumFilters
    {
        /// <summary>
        /// Retrieves the specified number of filters in the enumeration sequence.
        /// </summary>
        /// 
        /// <param name="cFilters">Number of filters to retrieve.</param>
        /// <param name="filters">Array in which to place <see cref="IBaseFilter"/> interfaces.</param>
        /// <param name="filtersFetched">Actual number of filters placed in the array.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Next( [In] int cFilters,
            [Out, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0 )] IBaseFilter[] filters,
            [Out] out int filtersFetched );

        /// <summary>
        /// Skips a specified number of filters in the enumeration sequence.
        /// </summary>
        /// 
        /// <param name="cFilters">Number of filters to skip.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Skip( [In] int cFilters );

        /// <summary>
        /// Resets the enumeration sequence to the beginning.
        /// </summary>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Reset( );

        /// <summary>
        /// Makes a copy of the enumerator with the same enumeration state.
        /// </summary>
        /// 
        /// <param name="enumFilters">Duplicate of the enumerator.</param>
        /// 
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        /// 
        [PreserveSig]
        int Clone( [Out] out IEnumFilters enumFilters );
    }
}
