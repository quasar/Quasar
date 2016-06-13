// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    using System;

    /// <summary>
    /// The enumeration specifies a setting on a camera.
    /// </summary>
    public enum CameraControlProperty
    {
        /// <summary>
        /// Pan control.
        /// </summary>
        Pan = 0,
        /// <summary>
        /// Tilt control.
        /// </summary>
        Tilt,
        /// <summary>
        /// Roll control.
        /// </summary>
        Roll,
        /// <summary>
        /// Zoom control.
        /// </summary>
        Zoom,
        /// <summary>
        /// Exposure control.
        /// </summary>
        Exposure,
        /// <summary>
        /// Iris control.
        /// </summary>
        Iris,
        /// <summary>
        /// Focus control.
        /// </summary>
        Focus
    }

    /// <summary>
    /// The enumeration defines whether a camera setting is controlled manually or automatically.
    /// </summary>
    [Flags]
    public enum CameraControlFlags
    {
        /// <summary>
        /// No control flag.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Auto control Flag.
        /// </summary>
        Auto = 0x0001,
        /// <summary>
        /// Manual control Flag.
        /// </summary>
        Manual = 0x0002
    }
}
