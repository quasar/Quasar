// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using xServer.Core.MouseKeyHook.Implementation;
using xServer.Core.MouseKeyHook.WinApi;

namespace xServer.Core.MouseKeyHook
{
    /// <summary>
    ///     Provides extended argument data for the <see cref='KeyListener.KeyDown' /> or
    ///     <see cref='KeyListener.KeyUp' /> event.
    /// </summary>
    public class KeyEventArgsExt : KeyEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyEventArgsExt" /> class.
        /// </summary>
        /// <param name="keyData"></param>
        public KeyEventArgsExt(Keys keyData)
            : base(keyData)
        {
        }

        internal KeyEventArgsExt(Keys keyData, int timestamp, bool isKeyDown, bool isKeyUp)
            : this(keyData)
        {
            Timestamp = timestamp;
            IsKeyDown = isKeyDown;
            IsKeyUp = isKeyUp;
        }

        /// <summary>
        ///     The system tick count of when the event occurred.
        /// </summary>
        public int Timestamp { get; private set; }

        /// <summary>
        ///     True if event signals key down..
        /// </summary>
        public bool IsKeyDown { get; private set; }

        /// <summary>
        ///     True if event signals key up.
        /// </summary>
        public bool IsKeyUp { get; private set; }

        internal static KeyEventArgsExt FromRawDataApp(CallbackData data)
        {
            var wParam = data.WParam;
            var lParam = data.LParam;

            //http://msdn.microsoft.com/en-us/library/ms644984(v=VS.85).aspx

            const uint maskKeydown = 0x40000000; // for bit 30
            const uint maskKeyup = 0x80000000; // for bit 31

            int timestamp = Environment.TickCount;

            var flags = (uint) lParam.ToInt64();

            //bit 30 Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
            bool wasKeyDown = (flags & maskKeydown) > 0;
            //bit 31 Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
            bool isKeyReleased = (flags & maskKeyup) > 0;

            Keys keyData = AppendModifierStates((Keys) wParam);

            bool isKeyDown = !wasKeyDown && !isKeyReleased;
            bool isKeyUp = wasKeyDown && isKeyReleased;

            return new KeyEventArgsExt(keyData, timestamp, isKeyDown, isKeyUp);
        }

        internal static KeyEventArgsExt FromRawDataGlobal(CallbackData data)
        {
            var wParam = data.WParam;
            var lParam = data.LParam;
            var keyboardHookStruct =
                (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));
            var keyData = AppendModifierStates((Keys) keyboardHookStruct.VirtualKeyCode);

            var keyCode = (int) wParam;
            bool isKeyDown = (keyCode == Messages.WM_KEYDOWN || keyCode == Messages.WM_SYSKEYDOWN);
            bool isKeyUp = (keyCode == Messages.WM_KEYUP || keyCode == Messages.WM_SYSKEYUP);

            return new KeyEventArgsExt(keyData, keyboardHookStruct.Time, isKeyDown, isKeyUp);
        }

        // # It is not possible to distinguish Keys.LControlKey and Keys.RControlKey when they are modifiers
        // Check for Keys.Control instead
        // Same for Shift and Alt(Menu)
        // See more at http://www.tech-archive.net/Archive/DotNet/microsoft.public.dotnet.framework.windowsforms/2008-04/msg00127.html #

        // A shortcut to make life easier
        private static bool CheckModifier(int vKey)
        {
            return (KeyboardNativeMethods.GetKeyState(vKey) & 0x8000) > 0;
        }

        private static Keys AppendModifierStates(Keys keyData)
        {
            // Is Control being held down?
            bool control = CheckModifier(KeyboardNativeMethods.VK_CONTROL);
            // Is Shift being held down?
            bool shift = CheckModifier(KeyboardNativeMethods.VK_SHIFT);
            // Is Alt being held down?
            bool alt = CheckModifier(KeyboardNativeMethods.VK_MENU);

            // Windows keys
            // # combine LWin and RWin key with other keys will potentially corrupt the data
            // notable F5 | Keys.LWin == F12, see https://globalmousekeyhook.codeplex.com/workitem/1188
            // and the KeyEventArgs.KeyData don't recognize combined data either

            // Function (Fn) key
            // # CANNOT determine state due to conversion inside keyboard
            // See http://en.wikipedia.org/wiki/Fn_key#Technical_details #

            return keyData |
                   (control ? Keys.Control : Keys.None) |
                   (shift ? Keys.Shift : Keys.None) |
                   (alt ? Keys.Alt : Keys.None);
        }
    }
}