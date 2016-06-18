// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2008
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// DirectShow filter categories.
    /// </summary>
    [ComVisible( false )]
    public static class FilterCategory
    {
        /// <summary>
        /// Audio input device category.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_AudioInputDeviceCategory.</remarks>
        /// 
        public static readonly Guid AudioInputDevice =
            new Guid( 0x33D9A762, 0x90C8, 0x11D0, 0xBD, 0x43, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86 );

        /// <summary>
        /// Video input device category.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_VideoInputDeviceCategory.</remarks>
        /// 
        public static readonly Guid VideoInputDevice =
            new Guid( 0x860BB310, 0x5D01, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86 );

        /// <summary>
        /// Video compressor category.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_VideoCompressorCategory.</remarks>
        /// 
        public static readonly Guid VideoCompressorCategory =
            new Guid( 0x33D9A760, 0x90C8, 0x11D0, 0xBD, 0x43, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86 );

        /// <summary>
        /// Audio compressor category
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_AudioCompressorCategory.</remarks>
        /// 
        public static readonly Guid AudioCompressorCategory =
            new Guid( 0x33D9A761, 0x90C8, 0x11D0, 0xBD, 0x43, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86 );
    }
}
