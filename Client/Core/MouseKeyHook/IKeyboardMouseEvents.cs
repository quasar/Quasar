// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;

namespace xClient.Core.MouseKeyHook
{
    /// <summary>
    ///     Provides keyboard and mouse events.
    /// </summary>
    public interface IKeyboardMouseEvents : IKeyboardEvents, IMouseEvents, IDisposable
    {
    }
}