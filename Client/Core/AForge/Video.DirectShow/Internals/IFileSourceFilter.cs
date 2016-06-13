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
    /// The interface is exposed by source filters to set the file name and media type of the media file that they are to render.
    /// </summary>
    /// 
    [ComImport,
    Guid( "56A868A6-0Ad4-11CE-B03A-0020AF0BA770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IFileSourceFilter
    {
        /// <summary>
        /// Loads the source filter with the file.
        /// </summary>
        /// 
        /// <param name="fileName">The name of the file to open.</param>
        /// <param name="mediaType">Media type of the file. This can be null.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Load( [In, MarshalAs( UnmanagedType.LPWStr )] string fileName,
            [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Retrieves the current file.
        /// </summary>
        /// 
        /// <param name="fileName">Name of media file.</param>
        /// <param name="mediaType">Receives media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCurFile([Out, MarshalAs( UnmanagedType.LPWStr )] out string fileName,
            [Out, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );
    }
}
