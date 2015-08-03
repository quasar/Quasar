// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using xServer.Core.MouseKeyHook.WinApi;

namespace xServer.Core.MouseKeyHook.Implementation
{
    /// <summary>
    ///     Contains a snapshot of a keyboard state at certain moment and provides methods
    ///     of querying whether specific keys are pressed or locked.
    /// </summary>
    /// <remarks>
    ///     This class is basically a managed wrapper of GetKeyboardState API function
    ///     http://msdn.microsoft.com/en-us/library/ms646299
    /// </remarks>
    internal class KeyboardState
    {
        private readonly byte[] m_KeyboardStateNative;

        private KeyboardState(byte[] keyboardStateNative)
        {
            m_KeyboardStateNative = keyboardStateNative;
        }

        /// <summary>
        ///     Makes a snapshot of a keyboard state to the moment of call and returns an
        ///     instance of <see cref="KeyboardState" /> class.
        /// </summary>
        /// <returns>An instance of <see cref="KeyboardState" /> class representing a snapshot of keyboard state at certain moment.</returns>
        public static KeyboardState GetCurrent()
        {
            byte[] keyboardStateNative = new byte[256];
            KeyboardNativeMethods.GetKeyboardState(keyboardStateNative);
            return new KeyboardState(keyboardStateNative);
        }

        internal byte[] GetNativeState()
        {
            return m_KeyboardStateNative;
        }

        /// <summary>
        ///     Indicates whether specified key was down at the moment when snapshot was created or not.
        /// </summary>
        /// <param name="key">Key (corresponds to the virtual code of the key)</param>
        /// <returns><b>true</b> if key was down, <b>false</b> - if key was up.</returns>
        public bool IsDown(Keys key)
        {
            byte keyState = GetKeyState(key);
            bool isDown = GetHighBit(keyState);
            return isDown;
        }

        /// <summary>
        ///     Indicate weather specified key was toggled at the moment when snapshot was created or not.
        /// </summary>
        /// <param name="key">Key (corresponds to the virtual code of the key)</param>
        /// <returns>
        ///     <b>true</b> if toggle key like (CapsLock, NumLocke, etc.) was on. <b>false</b> if it was off.
        ///     Ordinal (non toggle) keys return always false.
        /// </returns>
        public bool IsToggled(Keys key)
        {
            byte keyState = GetKeyState(key);
            bool isToggled = GetLowBit(keyState);
            return isToggled;
        }

        /// <summary>
        ///     Indicates weather every of specified keys were down at the moment when snapshot was created.
        ///     The method returns false if even one of them was up.
        /// </summary>
        /// <param name="keys">Keys to verify whether they were down or not.</param>
        /// <returns><b>true</b> - all were down. <b>false</b> - at least one was up.</returns>
        public bool AreAllDown(IEnumerable<Keys> keys)
        {
            foreach (Keys key in keys)
            {
                if (!IsDown(key))
                {
                    return true;
                }
            }
            return false;
        }

        private byte GetKeyState(Keys key)
        {
            int virtualKeyCode = (int) key;
            if (virtualKeyCode < 0 || virtualKeyCode > 255)
            {
                throw new ArgumentOutOfRangeException("key", key, "The value must be between 0 and 255.");
            }
            return m_KeyboardStateNative[virtualKeyCode];
        }

        private static bool GetHighBit(byte value)
        {
            return (value >> 7) != 0;
        }

        private static bool GetLowBit(byte value)
        {
            return (value & 1) != 0;
        }
    }
}