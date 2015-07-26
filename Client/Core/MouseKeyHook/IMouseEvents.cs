// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Windows.Forms;

namespace xClient.Core.MouseKeyHook
{
    /// <summary>
    ///     Provides all mouse events.
    /// </summary>
    public interface IMouseEvents
    {
        /// <summary>
        ///     Occurs when the mouse pointer is moved.
        /// </summary>
        event MouseEventHandler MouseMove;

        /// <summary>
        ///     Occurs when the mouse pointer is moved.
        /// </summary>
        /// <remarks>
        ///     This event provides extended arguments of type <see cref="MouseEventArgs" /> enabling you to
        ///     suppress further processing of mouse movement in other applications.
        /// </remarks>
        event EventHandler<MouseEventExtArgs> MouseMoveExt;

        /// <summary>
        ///     Occurs when a click was performed by the mouse.
        /// </summary>
        event MouseEventHandler MouseClick;

        /// <summary>
        ///     Occurs when the mouse a mouse button is pressed.
        /// </summary>
        event MouseEventHandler MouseDown;

        /// <summary>
        ///     Occurs when the mouse a mouse button is pressed.
        /// </summary>
        /// <remarks>
        ///     This event provides extended arguments of type <see cref="MouseEventArgs" /> enabling you to
        ///     suppress further processing of mouse click in other applications.
        /// </remarks>
        event EventHandler<MouseEventExtArgs> MouseDownExt;

        /// <summary>
        ///     Occurs when a mouse button is released.
        /// </summary>
        event MouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when a mouse button is released.
        /// </summary>
        /// <remarks>
        ///     This event provides extended arguments of type <see cref="MouseEventArgs" /> enabling you to
        ///     suppress further processing of mouse click in other applications.
        /// </remarks>
        event EventHandler<MouseEventExtArgs> MouseUpExt;


        /// <summary>
        ///     Occurs when the mouse wheel moves.
        /// </summary>
        event MouseEventHandler MouseWheel;

        /// <summary>
        ///     Occurs when a mouse button is double-clicked.
        /// </summary>
        event MouseEventHandler MouseDoubleClick;
    }
}