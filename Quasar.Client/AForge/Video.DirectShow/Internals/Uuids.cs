// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

	/// <summary>
	/// DirectShow class IDs.
	/// </summary>
    [ComVisible( false )]
    static internal class Clsid
    {
        /// <summary>
        /// System device enumerator.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_SystemDeviceEnum.</remarks>
        /// 
        public static readonly Guid SystemDeviceEnum =
            new Guid( 0x62BE5D10, 0x60EB, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86 );

        /// <summary>
        /// Filter graph.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_FilterGraph.</remarks>
        /// 
        public static readonly Guid FilterGraph =
            new Guid( 0xE436EBB3, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// Sample grabber.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_SampleGrabber.</remarks>
        /// 
        public static readonly Guid SampleGrabber =
            new Guid( 0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37 );

        /// <summary>
        /// Capture graph builder.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_CaptureGraphBuilder2.</remarks>
        /// 
        public static readonly Guid CaptureGraphBuilder2 =
            new Guid( 0xBF87B6E1, 0x8C27, 0x11D0, 0xB3, 0xF0, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5 );

        /// <summary>
        /// Async reader.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_AsyncReader.</remarks>
        /// 
        public static readonly Guid AsyncReader =
            new Guid( 0xE436EBB5, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );
    }

    /// <summary>
    /// DirectShow format types.
    /// </summary>
    /// 
    [ComVisible( false )]
    static internal class FormatType
    {
        /// <summary>
        /// VideoInfo.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo.</remarks>
        /// 
        public static readonly Guid VideoInfo =
            new Guid( 0x05589F80, 0xC356, 0x11CE, 0xBF, 0x01, 0x00, 0xAA, 0x00, 0x55, 0x59, 0x5A );

        /// <summary>
        /// VideoInfo2.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo2.</remarks>
        /// 
        public static readonly Guid VideoInfo2 =
            new Guid( 0xf72A76A0, 0xEB0A, 0x11D0, 0xAC, 0xE4, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA );
    }

    /// <summary>
    /// DirectShow media types.
    /// </summary>
    /// 
    [ComVisible( false )]
    static internal class MediaType
    {
        /// <summary>
        /// Video.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Video.</remarks>
        /// 
        public static readonly Guid Video =
            new Guid( 0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// Interleaved. Used by Digital Video (DV).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Interleaved.</remarks>
        /// 
        public static readonly Guid Interleaved =
            new Guid( 0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// Audio.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Audio.</remarks>
        /// 
        public static readonly Guid Audio =
            new Guid( 0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// Text.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Text.</remarks>
        /// 
        public static readonly Guid Text =
            new Guid( 0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// Byte stream with no time stamps.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Stream.</remarks>
        /// 
        public static readonly Guid Stream =
            new Guid( 0xE436EB83, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );
    }

    /// <summary>
    /// DirectShow media subtypes.
    /// </summary>
    /// 
    [ComVisible( false )]
    static internal class MediaSubType
    {
        /// <summary>
        /// YUY2 (packed 4:2:2).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_YUYV.</remarks>
        /// 
        public static readonly Guid YUYV =
            new Guid( 0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// IYUV.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_IYUV.</remarks>
        /// 
        public static readonly Guid IYUV =
            new Guid( 0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// A DV encoding format. (FOURCC 'DVSD')
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_DVSD.</remarks>
        /// 
        public static readonly Guid DVSD =
            new Guid( 0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );

        /// <summary>
        /// RGB, 1 bit per pixel (bpp), palettized.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB1.</remarks>
        /// 
        public static readonly Guid RGB1 =
            new Guid( 0xE436EB78, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB, 4 bpp, palettized.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB4.</remarks>
        /// 
        public static readonly Guid RGB4 =
            new Guid( 0xE436EB79, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB, 8 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB8.</remarks>
        /// 
        public static readonly Guid RGB8 =
            new Guid( 0xE436EB7A, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB 565, 16 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB565.</remarks>
        /// 
        public static readonly Guid RGB565 =
            new Guid( 0xE436EB7B, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB 555, 16 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB555.</remarks>
        /// 
        public static readonly Guid RGB555 =
            new Guid( 0xE436EB7C, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB, 24 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB24.</remarks>
        /// 
        public static readonly Guid RGB24 =
            new Guid( 0xE436Eb7D, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// RGB, 32 bpp, no alpha channel.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB32.</remarks>
        /// 
        public static readonly Guid RGB32 =
            new Guid( 0xE436EB7E, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// Data from AVI file.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_Avi.</remarks>
        /// 
        public static readonly Guid Avi =
            new Guid( 0xE436EB88, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 );

        /// <summary>
        /// Advanced Streaming Format (ASF).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_Asf.</remarks>
        /// 
        public static readonly Guid Asf =
            new Guid( 0x3DB80F90, 0x9412, 0x11D1, 0xAD, 0xED, 0x00, 0x00, 0xF8, 0x75, 0x4B, 0x99 );
    }

    /// <summary>
    /// DirectShow pin categories.
    /// </summary>
    /// 
    [ComVisible( false )]
    static internal class PinCategory
    {
        /// <summary>
        /// Capture pin.
        /// </summary>
        /// 
        /// <remarks>Equals to PIN_CATEGORY_CAPTURE.</remarks>
        /// 
        public static readonly Guid Capture =
            new Guid( 0xFB6C4281, 0x0353, 0x11D1, 0x90, 0x5F, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA );

        /// <summary>
        /// Still image pin.
        /// </summary>
        /// 
        /// <remarks>Equals to PIN_CATEGORY_STILL.</remarks>
        /// 
        public static readonly Guid StillImage =
            new Guid( 0xFB6C428A, 0x0353, 0x11D1, 0x90, 0x5F, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA );
    }

    // Below GUIDs are used by ICaptureGraphBuilder::FindInterface().
    [ComVisible( false )]
    static internal class FindDirection
    {
        /// <summary>Equals to LOOK_UPSTREAM_ONLY.</summary>
        public static readonly Guid UpstreamOnly =
            new Guid( 0xAC798BE0, 0x98E3, 0x11D1, 0xB3, 0xF1, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5 );

        /// <summary>Equals to LOOK_DOWNSTREAM_ONLY.</summary>
        public static readonly Guid DownstreamOnly =
            new Guid( 0xAC798BE1, 0x98E3, 0x11D1, 0xB3, 0xF1, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5 );
    }
}
