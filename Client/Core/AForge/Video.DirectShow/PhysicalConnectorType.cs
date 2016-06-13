// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2012
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    /// <summary>
    /// Specifies the physical type of pin (audio or video).
    /// </summary>
    public enum PhysicalConnectorType
    {
        /// <summary>
        /// Default value of connection type. Physically it does not exist, but just either to specify that
        /// connection type should not be changed (input) or was not determined (output).
        /// </summary>
        Default = 0,
        /// <summary>
        /// Specifies a tuner pin for video.
        /// </summary>
        VideoTuner = 1,
        /// <summary>
        /// Specifies a composite pin for video.
        /// </summary>
        VideoComposite,
        /// <summary>
        /// Specifies an S-Video (Y/C video) pin.
        /// </summary>
        VideoSVideo,
        /// <summary>
        /// Specifies an RGB pin for video.
        /// </summary>
        VideoRGB,
        /// <summary>
        /// Specifies a YRYBY (Y, R–Y, B–Y) pin for video.
        /// </summary>
        VideoYRYBY,
        /// <summary>
        /// Specifies a serial digital pin for video.
        /// </summary>
        VideoSerialDigital,
        /// <summary>
        /// Specifies a parallel digital pin for video.
        /// </summary>
        VideoParallelDigital,
        /// <summary>
        /// Specifies a SCSI (Small Computer System Interface) pin for video.
        /// </summary>
        VideoSCSI,
        /// <summary>
        /// Specifies an AUX (auxiliary) pin for video.
        /// </summary>
        VideoAUX,
        /// <summary>
        /// Specifies an IEEE 1394 pin for video.
        /// </summary>
        Video1394,
        /// <summary>
        /// Specifies a USB (Universal Serial Bus) pin for video.
        /// </summary>
        VideoUSB,
        /// <summary>
        /// Specifies a video decoder pin.
        /// </summary>
        VideoDecoder,
        /// <summary>
        /// Specifies a video encoder pin.
        /// </summary>
        VideoEncoder,
        /// <summary>
        /// Specifies a SCART (Peritel) pin for video.
        /// </summary>
        VideoSCART,
        /// <summary>
        /// Not used.
        /// </summary>
        VideoBlack,

        /// <summary>
        /// Specifies a tuner pin for audio.
        /// </summary>
        AudioTuner = 4096,
        /// <summary>
        /// Specifies a line pin for audio.
        /// </summary>
        AudioLine,
        /// <summary>
        /// Specifies a microphone pin.
        /// </summary>
        AudioMic,
        /// <summary>
        /// Specifies an AES/EBU (Audio Engineering Society/European Broadcast Union) digital pin for audio.
        /// </summary>
        AudioAESDigital,
        /// <summary>
        /// Specifies an S/PDIF (Sony/Philips Digital Interface Format) digital pin for audio.
        /// </summary>
        AudioSPDIFDigital,
        /// <summary>
        /// Specifies a SCSI pin for audio.
        /// </summary>
        AudioSCSI,
        /// <summary>
        /// Specifies an AUX pin for audio.
        /// </summary>
        AudioAUX,
        /// <summary>
        /// Specifies an IEEE 1394 pin for audio.
        /// </summary>
        Audio1394,
        /// <summary>
        /// Specifies a USB pin for audio.
        /// </summary>
        AudioUSB,
        /// <summary>
        /// Specifies an audio decoder pin.
        /// </summary>
        AudioDecoder
    }
}
