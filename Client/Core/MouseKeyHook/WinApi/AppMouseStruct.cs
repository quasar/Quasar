// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Runtime.InteropServices;

namespace xClient.Core.MouseKeyHook.WinApi
{
    /// <summary>
    ///     The AppMouseStruct structure contains information about a application-level mouse input event.
    /// </summary>
    /// <remarks>
    ///     See full documentation at http://globalmousekeyhook.codeplex.com/wikipage?title=MouseStruct
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    internal struct AppMouseStruct
    {
        /// <summary>
        ///     Specifies a Point structure that contains the X- and Y-coordinates of the cursor, in screen coordinates.
        /// </summary>
        [FieldOffset(0x00)] public Point Point;

        /// <summary>
        ///     Specifies information associated with the message.
        /// </summary>
        /// <remarks>
        ///     The possible values are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>0 - No Information</description>
        ///         </item>
        ///         <item>
        ///             <description>1 - X-Button1 Click</description>
        ///         </item>
        ///         <item>
        ///             <description>2 - X-Button2 Click</description>
        ///         </item>
        ///         <item>
        ///             <description>120 - Mouse Scroll Away from User</description>
        ///         </item>
        ///         <item>
        ///             <description>-120 - Mouse Scroll Toward User</description>
        ///         </item>
        ///     </list>
        /// </remarks>
#if IS_X64
        [FieldOffset(0x22)]
#else
        [FieldOffset(0x16)]
#endif
            public Int16 MouseData;

        /// <summary>
        ///     Converts the current <see cref="AppMouseStruct" /> into a <see cref="MouseStruct" />.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     The AppMouseStruct does not have a timestamp, thus one is generated at the time of this call.
        /// </remarks>
        public MouseStruct ToMouseStruct()
        {
            MouseStruct tmp = new MouseStruct();
            tmp.Point = Point;
            tmp.MouseData = MouseData;
            tmp.Timestamp = Environment.TickCount;
            return tmp;
        }
    }
}