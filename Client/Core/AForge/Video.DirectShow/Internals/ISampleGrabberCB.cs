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
    /// The interface provides callback methods for the <see cref="ISampleGrabber.SetCallback"/> method.
    /// </summary>
    /// 
	[ComImport,
	Guid("0579154A-2B53-4994-B0D0-E773148EFF85"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabberCB
    {
        /// <summary>
        /// Callback method that receives a pointer to the media sample.
        /// </summary>
        /// 
        /// <param name="sampleTime">Starting time of the sample, in seconds.</param>
        /// <param name="sample">Pointer to the sample's <b>IMediaSample</b> interface.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SampleCB( double sampleTime, IntPtr sample );

        /// <summary>
        /// Callback method that receives a pointer to the sample bufferþ
        /// </summary>
        /// 
        /// <param name="sampleTime">Starting time of the sample, in seconds.</param>
        /// <param name="buffer">Pointer to a buffer that contains the sample data.</param>
        /// <param name="bufferLen">Length of the buffer pointed to by <b>buffer</b>, in bytes</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int BufferCB( double sampleTime, IntPtr buffer, int bufferLen );
    }
}
