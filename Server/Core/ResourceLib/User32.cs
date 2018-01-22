using System;
using System.Runtime.InteropServices;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// User32.dll functions.
    /// </summary>
    public abstract class User32
    {
        /// <summary>
        /// Contains information about an icon or a cursor. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            /// <summary>
            /// Specifies whether this structure defines an icon or a cursor. 
            /// A value of TRUE specifies an icon; FALSE specifies a cursor. 
            /// </summary>
            public bool IsIcon;
            /// <summary>
            /// Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot spot is 
            /// always in the center of the icon, and this member is ignored.
            /// </summary>
            public int xHotspot;
            /// <summary>
            /// Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot spot 
            /// is always in the center of the icon, and this member is ignored.
            /// </summary>
            public int yHotspot;
            /// <summary>
            /// Specifies the icon bitmask bitmap. 
            /// </summary>
            public IntPtr MaskBitmap;
            /// <summary>
            /// Handle to the icon color bitmap.
            /// </summary>
            public IntPtr ColorBitmap;
        }

        /// <summary>
        /// Retrieve a handle to a device context (DC) for the client area of a specified window or for the entire screen. 
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area. 
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases a device context (DC), freeing it for use by other applications.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
        /// <param name="hDC">A handle to the DC to be released.</param>
        /// <returns>
        /// The return value indicates whether the DC was released. If the DC was released, the return value is 1.
        /// If the DC was not released, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">Handle to the icon to be destroyed.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Creates an icon or cursor from an ICONINFO structure.
        /// </summary>
        /// <param name="piconInfo">Pointer to an ICONINFO structure the function uses to create the icon or cursor.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the icon or cursor that is created.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32,dll", SetLastError = true)]
        internal static extern IntPtr CreateIconIndirect(ref ICONINFO piconInfo);

        /// <summary>
        /// The DIALOGTEMPLATE structure defines the dimensions and style of a dialog box. 
        /// This structure, always the first in a standard template for a dialog box, 
        /// also specifies the number of controls in the dialog box and therefore specifies 
        /// the number of subsequent DIALOGITEMTEMPLATE structures in the template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DIALOGTEMPLATE
        {
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Extended styles for a window.
            /// </summary>
            public UInt32 dwExtendedStyle;
            /// <summary>
            /// Specifies the number of items in the dialog box. 
            /// </summary>
            public UInt16 cdit;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
        }

        /// <summary>
        /// The DIALOGITEMTEMPLATE structure defines the dimensions and style of a control in a dialog box.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DIALOGITEMTEMPLATE
        {
            /// <summary>
            /// Specifies the style of the control.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Extended styles for a window.
            /// </summary>
            public UInt32 dwExtendedStyle;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the control. 
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the control.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the control.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the control.
            /// </summary>
            public Int16 cy;
            /// <summary>
            /// Specifies the control identifier.
            /// </summary>
            public Int16 id;
        }

        /// <summary>
        /// An extended dialog box template begins with a DIALOGEXTEMPLATE header that describes
        /// the dialog box and specifies the number of controls in the dialog box. For each 
        /// control in a dialog box, an extended dialog box template has a block of data that
        /// uses the DIALOGEXITEMTEMPLATE format to describe the control. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DIALOGEXTEMPLATE
        {
            /// <summary>
            /// Specifies the version number of the extended dialog box template. This member must be 1.
            /// </summary>
            public UInt16 dlgVer;
            /// <summary>
            /// Indicates whether a template is an extended dialog box template. 
            /// </summary>
            public UInt16 signature;
            /// <summary>
            /// Specifies the help context identifier for the dialog box window. When the system
            /// sends a WM_HELP message, it passes this value in the wContextId member of the 
            /// HELPINFO structure. 
            /// </summary>
            public UInt32 helpID;
            /// <summary>
            /// Specifies extended windows styles.
            /// </summary>
            public UInt32 exStyle;
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Specifies the number of controls in the dialog box.
            /// </summary>
            public UInt16 cDlgItems;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
        }

        /// <summary>
        /// A control entry in an extended dialog template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DIALOGEXITEMTEMPLATE
        {
            /// <summary>
            /// Specifies the help context identifier for the dialog box window. When the system
            /// sends a WM_HELP message, it passes this value in the wContextId member of the 
            /// HELPINFO structure. 
            /// </summary>
            public UInt32 helpID;
            /// <summary>
            /// Specifies extended windows styles.
            /// </summary>
            public UInt32 exStyle;
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
            /// <summary>
            /// Specifies the control identifier.
            /// </summary>
            public Int32 id;
        }

        /// <summary>
        /// Window styles.
        /// http://msdn.microsoft.com/en-us/library/ms632600(VS.85).aspx
        /// </summary>
        public enum WindowStyles : uint
        {
            /// <summary>
            /// Creates an overlapped window. An overlapped window has a title bar and 
            /// a border. Same as the WS_TILED style.
            /// </summary>
            WS_OVERLAPPED = 0x00000000,
            /// <summary>
            /// Creates a pop-up window. This style cannot be used with the WS_CHILD style.
            /// </summary>
            WS_POPUP = 0x80000000,
            /// <summary>
            /// Creates a child window. A window with this style cannot have
            /// a menu bar. This style cannot be used with the WS_POPUP style.
            /// </summary>
            WS_CHILD = 0x40000000,
            /// <summary>
            /// Creates a window that is initially minimized. Same as the WS_ICONIC style.
            /// </summary>
            WS_MINIMIZE = 0x20000000,
            /// <summary>
            /// Creates a window that is initially visible.
            /// </summary>
            WS_VISIBLE = 0x10000000,
            /// <summary>
            /// Creates a window that is initially disabled. A disabled window cannot receive 
            /// input from the user.
            /// </summary>
            WS_DISABLED = 0x08000000,
            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular
            /// child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips
            /// all other overlapping child windows out of the region of the child window 
            /// to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap,
            /// it is possible, when drawing within the client area of a child window, to draw
            /// within the client area of a neighboring child window.
            /// </summary>
            WS_CLIPSIBLINGS = 0x04000000,
            /// <summary>
            /// Excludes the area occupied by child windows when drawing occurs 
            /// within the parent window. This style is used when creating the parent window.
            /// </summary>
            WS_CLIPCHILDREN = 0x02000000,
            /// <summary>
            /// Creates a window that is initially maximized.
            /// </summary>
            WS_MAXIMIZE = 0x01000000,
            /// <summary>
            /// Creates a window that has a title bar (includes the WS_BORDER style).
            /// </summary>
            WS_CAPTION = 0x00C00000, /* WS_BORDER | WS_DLGFRAME */
            /// <summary>
            /// Creates a window that has a thin-line border.
            /// </summary>
            WS_BORDER = 0x00800000,
            /// <summary>
            /// Creates a window that has a border of a style typically used with dialog 
            /// boxes. A window with this style cannot have a title bar.
            /// </summary>
            WS_DLGFRAME = 0x00400000,
            /// <summary>
            /// Creates a window that has a vertical scroll bar.
            /// </summary>
            WS_VSCROLL = 0x00200000,
            /// <summary>
            /// Creates a window that has a horizontal scroll bar.
            /// </summary>
            WS_HSCROLL = 0x00100000,
            /// <summary>
            /// Creates a window that has a window menu on its title bar. The WS_CAPTION 
            /// style must also be specified.
            /// </summary>
            WS_SYSMENU = 0x00080000,
            /// <summary>
            /// Creates a window that has a sizing border. Same as the WS_SIZEBOX style.
            /// </summary>
            WS_THICKFRAME = 0x00040000,
            /// <summary>
            /// Specifies the first control of a group of controls. The group consists of this
            /// first control and all controls defined after it, up to the next control with 
            /// the WS_GROUP style. The first control in each group usually has the WS_TABSTOP
            /// style so that the user can move from group to group. The user can subsequently
            /// change the keyboard focus from one control in the group to the next control in 
            /// the group by using the direction keys. 
            /// </summary>
            WS_GROUP = 0x00020000,
            /// <summary>
            /// Specifies a control that can receive the keyboard focus when the user presses 
            /// the TAB key. Pressing the TAB key changes the keyboard focus to the next 
            /// control with the WS_TABSTOP style. 
            /// </summary>
            WS_TABSTOP = 0x00010000,
            /*
            /// <summary>
            /// Creates a window that has a minimize button. Cannot be combined with the
            /// WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. 
            /// </summary>
            // WS_MINIMIZEBOX = 0x00020000,
            /// <summary>
            /// Creates a window that has a maximize button. Cannot be combined with the 
            /// WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. 
            /// </summary>
            // WS_MAXIMIZEBOX = 0x00010000,
            // WS_TILED = WS_OVERLAPPED,
            // WS_ICONIC = WS_MINIMIZE,
            // WS_SIZEBOX = WS_THICKFRAME,
            // WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
            */
        }

        /// <summary>
        /// Dialog styles.
        /// http://msdn.microsoft.com/en-us/library/ms644994(VS.85).aspx
        /// </summary>
        public enum DialogStyles : uint
        {
            /// <summary>
            /// Specifying this style in the dialog template tells Windows that the dtX and dtY values
            /// of the DIALOGTEMPLATE struct are relative to the screen origin, not the owner of the 
            /// dialog box. 
            /// </summary>
            DS_ABSALIGN = 0x01,
            /// <summary>
            /// Create a dialog box with the WS_EX_TOPMOST flag. This flag cannot be combined with the 
            /// DS_CONTROL style. This flag is obsolete and is included for compatibility with 16-bit 
            /// versions of Windows. 
            /// </summary>
            DS_SYSMODAL = 0x02,
            /// <summary>
            /// Applies to 16-bit applications only. This style directs edit controls in the dialog
            /// box to allocate memory from the application data segment. Otherwise, edit controls
            /// allocate storage from a global memory object.
            /// </summary>
            DS_LOCALEDIT = 0x20,
            /// <summary>
            /// Indicates that the header of the dialog box template contains additional data specifying
            /// the font to use for text in the client area and controls of the dialog box.
            /// </summary>
            DS_SETFONT = 0x40,
            /// <summary>
            /// Creates a dialog box with a modal dialog-box frame that can be combined with a title
            /// bar and window menu by specifying the WS_CAPTION and WS_SYSMENU styles.
            /// </summary>
            DS_MODALFRAME = 0x80,
            /// <summary>
            /// Suppresses WM_ENTERIDLE messages that the system would otherwise send to the owner of the
            /// dialog box while the dialog box is displayed.
            /// </summary>
            DS_NOIDLEMSG = 0x100,
            /// <summary>
            /// Causes the system to use the SetForegroundWindow function to bring the dialog 
            /// box to the foreground. 
            /// </summary>
            DS_SETFOREGROUND = 0x200,
            /// <summary>
            /// Gives the dialog box a nonbold font and draws three-dimensional borders around
            /// control windows in the dialog box. 
            /// </summary>
            DS_3DLOOK = 0x0004,
            /// <summary>
            /// Causes the dialog box to use the SYSTEM_FIXED_FONT instead of the default SYSTEM_FONT. 
            /// This is a monospace font compatible with the System font in 16-bit versions of Windows
            /// earlier than 3.0.
            /// </summary>
            DS_FIXEDSYS = 0x0008,
            /// <summary>
            /// Creates the dialog box even if errors occur — for example, if a child window cannot be 
            /// created or if the system cannot create a special data segment for an edit control.
            /// </summary>
            DS_NOFAILCREATE = 0x0010,
            /// <summary>
            /// Creates a dialog box that works well as a child window of another dialog box,
            /// much like a page in a property sheet. This style allows the user to tab among
            /// the control windows of a child dialog box, use its accelerator keys, and so on.
            /// </summary>
            DS_CONTROL = 0x0400,
            /// <summary>
            /// Centers the dialog box in the working area; that is, the area not obscured by the tray.
            /// </summary>
            DS_CENTER = 0x0800,
            /// <summary>
            /// Centers the dialog box on the mouse cursor.
            /// </summary>
            DS_CENTERMOUSE = 0x1000,
            /// <summary>
            /// Includes a question mark in the title bar of the dialog box. When the user clicks the 
            /// question mark, the cursor changes to a question mark with a pointer. If the user then 
            /// clicks a control in the dialog box, the control receives a WM_HELP message. The control
            /// should pass the message to the dialog box procedure, which should call the function 
            /// using the HELP_WM_HELP command. The help application displays a pop-up window that 
            /// typically contains help for the control. 
            /// </summary>
            DS_CONTEXTHELP = 0x2000,
            /// <summary>
            /// Indicates that the dialog box should use the system font.
            /// </summary>
            DS_SHELLFONT = DS_SETFONT | DS_FIXEDSYS,
            /// <summary>
            /// 
            /// </summary>
            DS_USEPIXELS = 0x8000
        }

        /// <summary>
        /// Extended dialog styles.
        /// </summary>
        public enum ExtendedDialogStyles : uint
        {
            /// <summary>
            /// Creates a window that has a double border; the window can, optionally, be created 
            /// with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
            /// </summary>
            WS_EX_DLGMODALFRAME = 0x00000001,
            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY 
            /// message to its parent window when it is created or destroyed.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            /// <summary>
            /// Specifies that a window created with this style should be placed above all non-topmost 
            /// windows and should stay above them, even when the window is deactivated. To add or remove
            /// this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,
            /// <summary>
            /// Specifies that a window created with this style accepts drag-drop files.
            /// </summary>
            WS_EX_ACCEPTFILES = 0x00000010,
            /// <summary>
            /// Specifies that a window created with this style should not be painted until siblings beneath
            /// the window (that were created by the same thread) have been painted. The window appears 
            /// transparent because the bits of underlying sibling windows have already been painted.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,
            /// <summary>
            /// Creates a multiple-document interface (MDI) child window.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,
            /// <summary>
            /// Creates a tool window; that is, a window intended to be used as a floating toolbar. 
            /// A tool window has a title bar that is shorter than a normal title bar, and the
            /// window title is drawn using a smaller font. A tool window does not appear in 
            /// the taskbar or in the dialog that appears when the user presses ALT+TAB. If a 
            /// tool window has a system menu, its icon is not displayed on the title bar. However, 
            /// you can display the system menu by right-clicking or by typing ALT+SPACE. 
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,
            /// <summary>
            /// Specifies that a window has a border with a raised edge.
            /// </summary>
            WS_EX_WINDOWEDGE = 0x00000100,
            /// <summary>
            /// Specifies that a window has a border with a sunken edge.
            /// </summary>
            WS_EX_CLIENTEDGE = 0x00000200,
            /// <summary>
            /// Includes a question mark in the title bar of the window. When the user clicks the question mark,
            /// the cursor changes to a question mark with a pointer. If the user then clicks a child window,
            /// the child receives a WM_HELP message. The child window should pass the message to the parent
            /// window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The 
            /// Help application displays a pop-up window that typically contains help for the child window.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,
            /// <summary>
            /// The window has generic "right-aligned" properties. This depends on the window class. This style has 
            /// an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order
            /// alignment; otherwise, the style is ignored. Using the WS_EX_RIGHT style for static or edit controls 
            /// has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with 
            /// button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles. 
            /// </summary>
            WS_EX_RIGHT = 0x00001000,
            /// <summary>
            /// Creates a window that has generic left-aligned properties. This is the default.
            /// </summary>
            WS_EX_LEFT = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading-order 
            /// alignment, the window text is displayed using right-to-left reading-order properties. 
            /// For other languages, the style is ignored.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,
            /// <summary>
            /// The window text is displayed using left-to-right reading-order properties. This is the default.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading order 
            /// alignment, the vertical scroll bar (if present) is to the left of the client area. For other
            /// languages, the style is ignored.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            /// <summary>
            /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
            /// </summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            /// <summary>
            /// The window itself contains child windows that should take part in dialog box navigation. 
            /// If this style is specified, the dialog manager recurses into children of this window when
            /// performing navigation operations such as handling the TAB key, an arrow key, or a 
            /// keyboard mnemonic.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,
            /// <summary>
            /// Creates a window with a three-dimensional border style intended to be used for items that 
            /// do not accept user input.
            /// </summary>
            WS_EX_STATICEDGE = 0x00020000,
            /// <summary>
            /// Forces a top-level window onto the taskbar when the window is visible.
            /// </summary>
            WS_EX_APPWINDOW = 0x00040000,
            /// <summary>
            /// Combines the WS_EX_CLIENTEDGE and WS_EX_WINDOWEDGE styles.
            /// </summary>
            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            /// <summary>
            /// Combines the WS_EX_WINDOWEDGE, WS_EX_TOOLWINDOW, and WS_EX_TOPMOST styles.
            /// </summary>
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            /// <summary>
            /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child 
            /// windows. Also, this cannot be used if the window has a class style of either CS_OWNDC
            /// or CS_CLASSDC. 
            /// </summary>
            WS_EX_LAYERED = 0x00080000,
            /// <summary>
            /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            /// <summary>
            /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose
            /// horizontal origin is on the right edge. Increasing horizontal values advance to the left. 
            /// </summary>
            WS_EX_LAYOUTRTL = 0x00400000,
            /// <summary>
            /// Windows XP: Paints all descendants of a window in bottom-to-top painting order 
            /// using double-buffering. For more information, see Remarks. This cannot be used
            /// if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
            /// </summary>
            WS_EX_COMPOSITED = 0x02000000,
            /// <summary>
            /// Windows 2000/XP: A top-level window created with this style does not become the foreground
            /// window when the user clicks it. The system does not bring this window to the foreground 
            /// when the user minimizes or closes the foreground window.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,
        }

        /// <summary>
        /// Possible DIALOGEXITEMTEMPLATE WindowClass ordinals.
        /// </summary>
        public enum DialogItemClass : uint
        {
            /// <summary>
            /// A button.
            /// </summary>
            Button = 0x0080,
            /// <summary>
            /// An edit box.
            /// </summary>
            Edit = 0x0081,
            /// <summary>
            /// A static control.
            /// </summary>
            Static = 0x0082,
            /// <summary>
            /// A list box.
            /// </summary>
            ListBox = 0x0083,
            /// <summary>
            /// A scroll bar.
            /// </summary>
            ScrollBar = 0x0084,
            /// <summary>
            /// A combo box.
            /// </summary>
            ComboBox = 0x0085
        }

        /// <summary>
        /// Static control styles.
        /// A static control specifies the STATIC class, appropriate window style constants, 
        /// and a combination of the following static control styles.
        /// http://msdn.microsoft.com/en-us/library/bb760773(VS.85).aspx
        /// </summary>
        public enum StaticControlStyles : uint
        {
            /// <summary>
            /// Specifies a simple rectangle and left-aligns the text in the rectangle. 
            /// The text is formatted before it is displayed. Words that extend past the 
            /// end of a line are automatically wrapped to the beginning of the next left-aligned
            /// line. Words that are longer than the width of the control are truncated.
            /// </summary>
            SS_LEFT = 0x00000000,
            /// <summary>
            /// Specifies a simple rectangle and centers the text in the rectangle. 
            /// The text is formatted before it is displayed. Words that extend past the 
            /// end of a line are automatically wrapped to the beginning of the next centered 
            /// line. Words that are longer than the width of the control are truncated.
            /// </summary>
            SS_CENTER = 0x00000001,
            /// <summary>
            /// Specifies a simple rectangle and right-aligns the text in the rectangle. 
            /// The text is formatted before it is displayed. Words that extend past the 
            /// end of a line are automatically wrapped to the beginning of the next 
            /// right-aligned line. Words that are longer than the width of the control 
            /// are truncated.
            /// </summary>
            SS_RIGHT = 0x00000002,
            /// <summary>
            /// Specifies an icon to be displayed in the dialog box. If the control is created 
            /// as part of a dialog box, the text is the name of an icon (not a filename) defined 
            /// elsewhere in the resource file. If the control is created via CreateWindow or a 
            /// related function, the text is the name of an icon (not a filename) defined in the
            /// resource file associated with the module specified by the hInstance parameter to 
            /// CreateWindow. The icon can be an animated cursor.
            /// </summary>
            SS_ICON = 0x00000003,
            /// <summary>
            /// Specifies a rectangle filled with the current window frame color.
            /// This color is black in the default color scheme.
            /// </summary>
            SS_BLACKRECT = 0x00000004,
            /// <summary>
            /// Specifies a rectangle filled with the current screen background color. 
            /// This color is gray in the default color scheme.
            /// </summary>
            SS_GRAYRECT = 0x00000005,
            /// <summary>
            /// Specifies a rectangle filled with the current window background color. This color is white 
            /// in the default color scheme.
            /// </summary>
            SS_WHITERECT = 0x00000006,
            /// <summary>
            /// Specifies a box with a frame drawn in the same color as the window frames. 
            /// This color is black in the default color scheme.
            /// </summary>
            SS_BLACKFRAME = 0x00000007,
            /// <summary>
            /// Specifies a box with a frame drawn with the same color as the screen background (desktop).
            /// This color is gray in the default color scheme.
            /// </summary>
            SS_GRAYFRAME = 0x00000008,
            /// <summary>
            /// Specifies a box with a frame drawn with the same color as the window background. This color is 
            /// white in the default color scheme.
            /// </summary>
            SS_WHITEFRAME = 0x00000009,
            /// <summary>
            /// 
            /// </summary>
            SS_USERITEM = 0x0000000A,
            /// <summary>
            /// Specifies a simple rectangle and displays a single line of left-aligned text in the rectangle.
            /// The text line cannot be shortened or altered in any way. Also, if the control is disabled,
            /// the control does not gray its text.
            /// </summary>
            SS_SIMPLE = 0x0000000B,
            /// <summary>
            /// Specifies a simple rectangle and left-aligns the text in the rectangle. 
            /// Tabs are expanded, but words are not wrapped. Text that extends past the 
            /// end of a line is clipped.
            /// </summary>
            SS_LEFTNOWORDWRAP = 0x0000000C,
            /// <summary>
            /// Specifies that the owner of the static control is responsible for drawing 
            /// the control. The owner window receives a WM_DRAWITEM message whenever the
            /// control needs to be drawn.
            /// </summary>
            SS_OWNERDRAW = 0x0000000D,
            /// <summary>
            /// Specifies that a bitmap is to be displayed in the static control. 
            /// The text is the name of a bitmap (not a filename) defined elsewhere in the 
            /// resource file. The style ignores the nWidth and nHeight parameters; the control 
            /// automatically sizes itself to accommodate the bitmap.
            /// </summary>
            SS_BITMAP = 0x0000000E,
            /// <summary>
            /// Specifies that an enhanced metafile is to be displayed in the static control.
            /// The text is the name of a metafile. An enhanced metafile static control has a 
            /// fixed size; the metafile is scaled to fit the static control's client area.
            /// </summary>
            SS_ENHMETAFILE = 0x0000000F,
            /// <summary>
            /// Draws the top and bottom edges of the static control using the EDGE_ETCHED edge style.
            /// </summary>
            SS_ETCHEDHORZ = 0x00000010,
            /// <summary>
            /// Draws the left and right edges of the static control using the EDGE_ETCHED edge style.
            /// </summary>
            SS_ETCHEDVERT = 0x00000011,
            /// <summary>
            /// Draws the frame of the static control using the EDGE_ETCHED edge style. 
            /// </summary>
            SS_ETCHEDFRAME = 0x00000012,
            /// <summary>
            /// Windows 2000: A composite style bit that results from using the OR operator on 
            /// SS_* style bits. Can be used to mask out valid SS_* bits from a given bitmask.
            /// Note that this is out of date and does not correctly include all valid styles.
            /// Thus, you should not use this style.
            /// </summary>
            SS_TYPEMASK = 0x0000001F,
            /// <summary>
            /// Windows XP or later: Adjusts the bitmap to fit the size of the static control.
            /// For example, changing the locale can change the system font, and thus controls
            /// might be resized. If a static control had a bitmap, the bitmap would no longer
            /// fit the control. This style bit dictates automatic redimensioning of bitmaps 
            /// to fit their controls. 
            /// </summary>
            SS_REALSIZECONTROL = 0x00000040,
            /// <summary>
            /// Prevents interpretation of any ampersand characters in the control's text as
            /// accelerator prefix characters. These are displayed with the ampersand removed and 
            /// the next character in the string underlined. This static control style may be 
            /// included with any of the defined static controls. You can combine SS_NOPREFIX 
            /// with other styles. This can be useful when filenames or other strings that may 
            /// contain an ampersand must be displayed in a static control in a dialog box.
            /// </summary>
            SS_NOPREFIX = 0x00000080,
            /// <summary>
            /// Sends the parent window STN_CLICKED, STN_DBLCLK, STN_DISABLE, and STN_ENABLE 
            /// notification messages when the user clicks or double-clicks the control.
            /// </summary>
            SS_NOTIFY = 0x00000100,
            /// <summary>
            /// Specifies that a bitmap is centered in the static control that contains it. 
            /// The control is not resized, so that a bitmap too large for the control will 
            /// be clipped. If the static control contains a single line of text, the text 
            /// is centered vertically in the client area of the control. 
            /// </summary>
            SS_CENTERIMAGE = 0x00000200,
            /// <summary>
            /// Specifies that the lower right corner of a static control with the SS_BITMAP
            /// or SS_ICON style is to remain fixed when the control is resized. Only the top
            /// and left sides are adjusted to accommodate a new bitmap or icon.
            /// </summary>
            SS_RIGHTJUST = 0x00000400,
            /// <summary>
            /// Specifies that the actual resource width is used and the icon is loaded using
            /// LoadImage. SS_REALSIZEIMAGE is always used in conjunction with SS_ICON. 
            /// </summary>
            SS_REALSIZEIMAGE = 0x00000800,
            /// <summary>
            /// Draws a half-sunken border around a static control.
            /// </summary>
            SS_SUNKEN = 0x00001000,
            /// <summary>
            /// Microsoft Windows 2000: Specifies that the static control duplicates the 
            /// text-displaying characteristics of a multiline edit control. Specifically, the 
            /// average character width is calculated in the same manner as with an edit control, 
            /// and the function does not display a partially visible last line.
            /// </summary>
            SS_EDITCONTROL = 0x00002000,
            /// <summary>
            /// Microsoft Windows NT or later: If the end of a string does not fit in the rectangle,
            /// it is truncated and ellipses are added. If a word that is not at the end of the string 
            /// goes beyond the limits of the rectangle, it is truncated without ellipses. Using this 
            /// style will force the controls text to be on one line with no word wrap. Compare with 
            /// SS_PATHELLIPSIS and SS_WORDELLIPSIS.
            /// </summary>
            SS_ENDELLIPSIS = 0x00004000,
            /// <summary>
            /// Windows NT or later: Replaces characters in the middle of the string with ellipses so 
            /// that the result fits in the specified rectangle. If the string contains backslash (\) 
            /// characters, SS_PATHELLIPSIS preserves as much as possible of the text after the last 
            /// backslash. Using this style will force the controls text to be on one line with no 
            /// word wrap. Compare with SS_ENDELLIPSIS and SS_WORDELLIPSIS.
            /// </summary>
            SS_PATHELLIPSIS = 0x00008000,
            /// <summary>
            /// Windows NT or later: Truncates any word that does not fit in the rectangle and adds ellipses. 
            /// Using this style will force the controls text to be on one line with no word wrap. 
            /// </summary>
            SS_WORDELLIPSIS = 0x0000C000,
            /// <summary>
            /// 
            /// </summary>
            SS_ELLIPSISMASK = 0x0000C000,
        }

        /// <summary>
        /// Push button styles.
        /// http://msdn.microsoft.com/en-us/library/bb775951(VS.85).aspx
        /// </summary>
        public enum ButtonControlStyles : uint
        {
            /// <summary>
            /// Creates a push button that posts a WM_COMMAND message to the owner window when the
            /// user selects the button.
            /// </summary>
            BS_PUSHBUTTON = 0x00000000,
            /// <summary>
            /// Creates a push button that behaves like a BS_PUSHBUTTON style button, but has a distinct
            /// appearance. If the button is in a dialog box, the user can select the button by pressing
            /// the ENTER key, even when the button does not have the input focus. This style is useful 
            /// for enabling the user to quickly select the most likely (default) option.
            /// </summary>
            BS_DEFPUSHBUTTON = 0x00000001,
            /// <summary>
            /// Creates a small, empty check box with text. By default, the text is displayed to
            /// the right of the check box. To display the text to the left of the check box, 
            /// combine this flag with the BS_LEFTTEXT style (or with the equivalent BS_RIGHTBUTTON style).
            /// </summary>
            BS_CHECKBOX = 0x00000002,
            /// <summary>
            /// Creates a button that is the same as a check box, except that the check state 
            /// automatically toggles between checked and cleared each time the user selects the
            /// check box.
            /// </summary>
            BS_AUTOCHECKBOX = 0x00000003,
            /// <summary>
            /// Creates a small circle with text. By default, the text is displayed to the right of the
            /// circle. To display the text to the left of the circle, combine this flag with the 
            /// BS_LEFTTEXT style (or with the equivalent BS_RIGHTBUTTON style). Use radio buttons for 
            /// groups of related, but mutually exclusive choices.
            /// </summary>
            BS_RADIOBUTTON = 0x00000004,
            /// <summary>
            /// Creates a button that is the same as a check box, except that the box can be grayed
            /// as well as checked or cleared. Use the grayed state to show that the state of the 
            /// check box is not determined.
            /// </summary>
            BS_3STATE = 0x00000005,
            /// <summary>
            /// Creates a button that is the same as a three-state check box, except that the box 
            /// changes its state when the user selects it. The state cycles through checked, 
            /// indeterminate, and cleared.
            /// </summary>
            BS_AUTO3STATE = 0x00000006,
            /// <summary>
            /// Creates a rectangle in which other controls can be grouped. Any text associated with
            /// this style is displayed in the rectangle's upper left corner.
            /// </summary>
            BS_GROUPBOX = 0x00000007,
            /// <summary>
            /// Obsolete, but provided for compatibility with 16-bit versions of Windows. Applications 
            /// should use BS_OWNERDRAW instead.
            /// </summary>
            BS_USERBUTTON = 0x00000008,
            /// <summary>
            /// Creates a button that is the same as a radio button, except that when the user selects it,
            /// the system automatically sets the button's check state to checked and automatically sets 
            /// the check state for all other buttons in the same group to cleared.
            /// </summary>
            BS_AUTORADIOBUTTON = 0x00000009,
            /// <summary>
            /// 
            /// </summary>
            BS_PUSHBOX = 0x0000000A,
            /// <summary>
            /// Creates an owner-drawn button. The owner window receives a WM_DRAWITEM message when a visual 
            /// aspect of the button has changed. Do not combine the BS_OWNERDRAW style with any other
            /// button styles.
            /// </summary>
            BS_OWNERDRAW = 0x0000000B,
            /// <summary>
            /// Microsoft Windows 2000: A composite style bit that results from using the OR operator on
            /// BS_* style bits. It can be used to mask out valid BS_* bits from a given bitmask.
            /// </summary>
            BS_TYPEMASK = 0x0000000F,
            /// <summary>
            /// Places text on the left side of the radio button or check box when combined with a radio button
            /// or check box style. Same as the BS_RIGHTBUTTON style.
            /// </summary>
            BS_LEFTTEXT = 0x00000020,
            /// <summary>
            /// Specifies that the button displays text.
            /// </summary>
            BS_TEXT = 0x00000000,
            /// <summary>
            /// Specifies that the button displays an icon.
            /// </summary>
            BS_ICON = 0x00000040,
            /// <summary>
            /// Specifies that the button displays a bitmap. See the Remarks section for its interaction
            /// with BS_ICON.
            /// </summary>
            BS_BITMAP = 0x00000080,
            /// <summary>
            /// Left-justifies the text in the button rectangle. However, if the button is a check box or radio
            /// button that does not have the BS_RIGHTBUTTON style, the text is left justified on the right side
            /// of the check box or radio button.
            /// </summary>
            BS_LEFT = 0x00000100,
            /// <summary>
            /// Right-justifies text in the button rectangle. However, if the button is a check box or radio
            /// button that does not have the BS_RIGHTBUTTON style, the text is right justified on the right
            /// side of the check box or radio button.
            /// </summary>
            BS_RIGHT = 0x00000200,
            /// <summary>
            /// Centers text horizontally in the button rectangle.
            /// </summary>
            BS_CENTER = 0x00000300,
            /// <summary>
            /// Places text at the top of the button rectangle.
            /// </summary>
            BS_TOP = 0x00000400,
            /// <summary>
            /// Places text at the bottom of the button rectangle.
            /// </summary>
            BS_BOTTOM = 0x00000800,
            /// <summary>
            /// Places text in the middle (vertically) of the button rectangle.
            /// </summary>
            BS_VCENTER = 0x00000C00,
            /// <summary>
            /// Makes a button (such as a check box, three-state check box, or radio button) look and 
            /// act like a push button. The button looks raised when it isn't pushed or checked, and 
            /// sunken when it is pushed or checked.
            /// </summary>
            BS_PUSHLIKE = 0x00001000,
            /// <summary>
            /// Wraps the button text to multiple lines if the text string is too long to fit on a 
            /// single line in the button rectangle.
            /// </summary>
            BS_MULTILINE = 0x00002000,
            /// <summary>
            /// Enables a button to send BN_KILLFOCUS and BN_SETFOCUS notification messages to its 
            /// parent window. 
            /// </summary>
            BS_NOTIFY = 0x00004000,
            /// <summary>
            /// Specifies that the button is two-dimensional; it does not use the default 
            /// shading to create a 3-D image. 
            /// </summary>
            BS_FLAT = 0x00008000,
            /// <summary>
            /// Microsoft Windows Vista and Version 6.00. Creates a split button that behaves like a 
            /// BS_PUSHBUTTON style button, but also has a distinctive appearance. If the split button 
            /// is in a dialog box, the user can select the split button by pressing the ENTER key, even 
            /// when the split button does not have the input focus. This style is useful for enabling
            /// the user to quickly select the most likely (default) option.
            /// </summary>
            BS_DEFSPLITBUTTON = 0x0000000D,
            /// <summary>
            /// Microsoft Windows Vista and Version 6.00. Creates a command link button that behaves like a
            /// BS_PUSHBUTTON style button, but the command link button has a green arrow on the left pointing
            /// to the button text. A caption for the button text can be set by sending the BCM_SETNOTE
            /// message to the button.
            /// </summary>
            BS_COMMANDLINK = 0x0000000E,
            /// <summary>
            /// Microsoft Windows Vista and Version 6.00. Creates a command link button that behaves like
            /// a BS_PUSHBUTTON style button. If the button is in a dialog box, the user can select the 
            /// command link button by pressing the ENTER key, even when the command link button does
            /// not have the input focus. This style is useful for enabling the user to quickly select
            /// the most likely (default) option.
            /// </summary>
            BS_DEFCOMMANDLINK = 0x0000000F,
        }

        /// <summary>
        /// Edit control styles.
        /// http://msdn.microsoft.com/en-us/library/bb775464(VS.85).aspx
        /// </summary>
        public enum EditControlStyles : uint
        {
            /// <summary>
            /// Aligns text with the left margin.
            /// </summary>
            ES_LEFT = 0x0000,
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Centers text in a single-line or multiline edit control.
            /// Windows 95, Windows NT 4.0 and earlier: Centers text in a multiline edit control.
            /// </summary>
            ES_CENTER = 0x0001,
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Right-aligns text in a single-line or multiline edit control.
            /// Windows 95, Windows NT 4.0 and earlier: Right aligns text in a multiline edit control.
            /// </summary>
            ES_RIGHT = 0x0002,
            /// <summary>
            /// Designates a multiline edit control. The default is single-line edit control. 
            /// </summary>
            ES_MULTILINE = 0x0004,
            /// <summary>
            /// Converts all characters to uppercase as they are typed into the edit control. 
            /// </summary>
            ES_UPPERCASE = 0x0008,
            /// <summary>
            /// Converts all characters to lowercase as they are typed into the edit control.
            /// </summary>
            ES_LOWERCASE = 0x0010,
            /// <summary>
            /// Displays an asterisk (*) for each character typed into the edit control. 
            /// This style is valid only for single-line edit controls. 
            /// </summary>
            ES_PASSWORD = 0x0020,
            /// <summary>
            /// Automatically scrolls text up one page when the user presses the ENTER key on 
            /// the last line.
            /// </summary>
            ES_AUTOVSCROLL = 0x0040,
            /// <summary>
            /// Automatically scrolls text to the right by 10 characters when the user types
            /// a character at the end of the line. When the user presses the ENTER key, 
            /// the control scrolls all text back to position zero.
            /// </summary>
            ES_AUTOHSCROLL = 0x0080,
            /// <summary>
            /// Negates the default behavior for an edit control. 
            /// </summary>
            ES_NOHIDESEL = 0x0100,
            /// <summary>
            /// Converts text entered in the edit control.
            /// </summary>
            ES_OEMCONVERT = 0x0400,
            /// <summary>
            /// Prevents the user from typing or editing text in the edit control.
            /// </summary>
            ES_READONLY = 0x0800,
            /// <summary>
            /// Specifies that a carriage return be inserted when the user presses the 
            /// ENTER key while entering text into a multiline edit control in a dialog box. 
            /// If you do not specify this style, pressing the ENTER key has the same effect
            /// as pressing the dialog box's default push button. This style has no effect 
            /// on a single-line edit control. 
            /// </summary>
            ES_WANTRETURN = 0x1000,
            /// <summary>
            /// Allows only digits to be entered into the edit control. 
            /// </summary>
            ES_NUMBER = 0x2000,
        }

        /// <summary>
        /// Defines the header for a menu template. 
        /// A complete menu template consists of a header and one or more menu item lists. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct MENUTEMPLATE
        {
            /// <summary>
            /// Specifies the version number. This member must be zero. 
            /// </summary>
            public UInt16 wVersion;
            /// <summary>
            /// Specifies the offset, in bytes, from the end of the header. 
            /// The menu item list begins at this offset. Usually, this member is zero, and the menu 
            /// item list follows immediately after the header. 
            /// </summary>
            public UInt16 wOffset;
        }

        /// <summary>
        /// Defines a menu item in a menu template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct MENUITEMTEMPLATE
        {
            /// <summary>
            /// Specifies one or more of the following predefined menu options that control the appearance of the menu item.
            ///  TODO
            /// </summary>
            public UInt16 mtOption;
        }

        /// <summary>
        /// Defines the header for an extended menu template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MENUEXTEMPLATE
        {
            /// <summary>
            /// Template version number. This member must be 1 for extended menu templates.
            /// </summary>
            public UInt16 wVersion;
            /// <summary>
            /// Offset of the first MENUEXITEMTEMPLATE structure, relative to the end of 
            /// this structure member. If the first item definition immediately follows the 
            /// dwHelpId member, this member should be 4. 
            /// </summary>
            public UInt16 wOffset;
        }

        /// <summary>
        /// Drop-down menu or submenu item.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MENUEXITEMTEMPLATE
        {
            /// <summary>
            /// Menu item type. This member can be a combination of the type (beginning with MFT) values 
            /// listed with the MENUITEMINFO structure. 
            /// </summary>
            public UInt32 dwType;
            /// <summary>
            /// Menu item state. This member can be a combination of the state (beginning with MFS) values 
            /// listed with the MENUITEMINFO structure. 
            /// </summary>
            public UInt32 dwState;
            /// <summary>
            /// Menu item identifier. This is an application-defined value that identifies the menu item. 
            /// </summary>
            public UInt32 dwMenuId;
            /// <summary>
            /// Value specifying whether the menu item is the last item in the menu bar, drop-down menu, 
            /// submenu, or shortcut menu and whether it is an item that opens a drop-down menu or submenu.
            /// </summary>
            public UInt16 bResInfo;
        }

        /// <summary>
        /// Specifies one or more of the following predefined menu options that control the 
        /// appearance of the menu item.
        /// </summary>
        public enum MenuFlags : uint
        {
            /// <summary>
            /// 
            /// </summary>
            MF_INSERT = 0x00000000,
            /// <summary>
            /// 
            /// </summary>
            MF_CHANGE = 0x00000080,
            /// <summary>
            /// 
            /// </summary>
            MF_APPEND = 0x00000100,
            /// <summary>
            /// 
            /// </summary>
            MF_DELETE = 0x00000200,
            /// <summary>
            /// 
            /// </summary>
            MF_REMOVE = 0x00001000,
            /// <summary>
            /// 
            /// </summary>
            MF_BYCOMMAND = 0x00000000,
            /// <summary>
            /// 
            /// </summary>
            MF_BYPOSITION = 0x00000400,
            /// <summary>
            /// 
            /// </summary>
            MF_SEPARATOR = 0x00000800,
            /// <summary>
            /// 
            /// </summary>
            MF_ENABLED = 0x00000000,
            /// <summary>
            /// Indicates that the menu item is initially inactive and drawn with a gray effect.
            /// </summary>
            MF_GRAYED = 0x00000001,
            /// <summary>
            /// 
            /// </summary>
            MF_DISABLED = 0x00000002,
            /// <summary>
            /// 
            /// </summary>
            MF_UNCHECKED = 0x00000000,
            /// <summary>
            /// Indicates that the menu item has a check mark next to it.
            /// </summary>
            MF_CHECKED = 0x00000008,
            /// <summary>
            /// 
            /// </summary>
            MF_USECHECKBITMAPS = 0x00000200,
            /// <summary>
            /// 
            /// </summary>
            MF_STRING = 0x00000000,
            /// <summary>
            /// 
            /// </summary>
            MF_BITMAP = 0x00000004,
            /// <summary>
            /// Indicates that the owner window of the menu is responsible for drawing all visual 
            /// aspects of the menu item, including highlighted, selected, and inactive states. 
            /// This option is not valid for an item in a menu bar.
            /// </summary>
            MF_OWNERDRAW = 0x00000100,
            /// <summary>
            /// Indicates that the item is one that opens a drop-down menu or submenu.
            /// </summary>
            MF_POPUP = 0x00000010,
            /// <summary>
            /// Indicates that the menu item is placed in a new column. The old and new columns 
            /// are separated by a bar.
            /// </summary>
            MF_MENUBARBREAK = 0x00000020,
            /// <summary>
            /// Indicates that the menu item is placed in a new column.
            /// </summary>
            MF_MENUBREAK = 0x00000040,
            /// <summary>
            /// 
            /// </summary>
            MF_UNHILITE = 0x00000000,
            /// <summary>
            /// 
            /// </summary>
            MF_HILITE = 0x00000080,
            /// <summary>
            /// 
            /// </summary>
            MF_DEFAULT = 0x00001000,
            /// <summary>
            /// 
            /// </summary>
            MF_SYSMENU = 0x00002000,
            /// <summary>
            /// Indicates that the menu item has a vertical separator to its left.
            /// </summary>
            MF_HELP = 0x00004000,
            /// <summary>
            /// 
            /// </summary>
            MF_RIGHTJUSTIFY = 0x00004000,
            /// <summary>
            /// 
            /// </summary>
            MF_MOUSESELECT = 0x00008000,
            /// <summary>
            /// 
            /// </summary>
            MF_END = 0x00000080,
            /// <summary>
            /// 
            /// </summary>
            MFT_STRING = MF_STRING,
            /// <summary>
            /// 
            /// </summary>
            MFT_BITMAP = MF_BITMAP,
            /// <summary>
            /// 
            /// </summary>
            MFT_MENUBARBREAK = MF_MENUBARBREAK,
            /// <summary>
            /// 
            /// </summary>
            MFT_MENUBREAK = MF_MENUBREAK,
            /// <summary>
            /// 
            /// </summary>
            MFT_OWNERDRAW = MF_OWNERDRAW,
            /// <summary>
            /// 
            /// </summary>
            MFT_RADIOCHECK = 0x00000200,
            /// <summary>
            /// 
            /// </summary>
            MFT_SEPARATOR = MF_SEPARATOR,
            /// <summary>
            /// 
            /// </summary>
            MFT_RIGHTORDER = 0x00002000,
            /// <summary>
            /// 
            /// </summary>
            MFT_RIGHTJUSTIFY = MF_RIGHTJUSTIFY,
            /// <summary>
            /// 
            /// </summary>
            MFS_GRAYED = 0x00000003,
            /// <summary>
            /// 
            /// </summary>
            MFS_DISABLED = MFS_GRAYED,
            /// <summary>
            /// 
            /// </summary>
            MFS_CHECKED = MF_CHECKED,
            /// <summary>
            /// 
            /// </summary>
            MFS_HILITE = MF_HILITE,
            /// <summary>
            /// 
            /// </summary>
            MFS_ENABLED = MF_ENABLED,
            /// <summary>
            /// 
            /// </summary>
            MFS_UNCHECKED = MF_UNCHECKED,
            /// <summary>
            /// 
            /// </summary>
            MFS_UNHILITE = MF_UNHILITE,
            /// <summary>
            /// 
            /// </summary>
            MFS_DEFAULT = MF_DEFAULT
        }


        /// <summary>
        /// Specifies whether the menu item is the last item in the menu bar, drop-down menu, submenu, or shortcut 
        /// menu and whether it is an item that opens a drop-down menu or submenu. This member can be zero or more 
        /// of these values.
        /// </summary>
        public enum MenuResourceType
        {
            /// <summary>
            /// Defines the last menu item in the menu bar, drop-down menu, submenu, or shortcut menu.
            /// </summary>
            Last = 0x80,
            /// <summary>
            /// Defines a item that opens a drop-down menu or submenu. Subsequent structures define menu 
            /// items in the corresponding drop-down menu or submenu.
            /// </summary>
            Sub = 0x01
        }

        /// <summary>
        /// Defines an accelerator key used in an accelerator table.
        /// http://msdn.microsoft.com/en-us/library/ms646340(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct ACCEL
        {
            /// <summary>
            /// Accelerator flags.
            /// </summary>
            public UInt16 fVirt;
            /// <summary>
            /// Accelerator key. This member can be either a virtual-key code or an ASCII character code. 
            /// </summary>
            public UInt16 key;
            /// <summary>
            /// Accelerator identifier.
            /// </summary>
            public UInt32 cmd;
        }

        /// <summary>
        /// Flags, fVirt field of the Accelerator table structure.
        /// </summary>
        public enum AcceleratorVirtualKey : uint
        {
            /// <summary>
            /// Virtual key.
            /// </summary>
            VIRTKEY = 0x01,
            /// <summary>
            /// Specifies that no top-level menu item is highlighted when the accelerator is used. 
            /// This is useful when defining accelerators for actions such as scrolling that do not 
            /// correspond to a menu item. If NOINVERT is omitted, a top-level menu item will be 
            /// highlighted (if possible) when the accelerator is used.
            /// </summary>
            NOINVERT = 0x02,
            /// <summary>
            /// Causes the accelerator to be activated only if the SHIFT key is down. 
            /// Applies only to virtual keys.
            /// </summary>
            SHIFT = 0x04,
            /// <summary>
            /// Causes the accelerator to be activated only if the CONTROL key is down. 
            /// Applies only to virtual keys.
            /// </summary>
            CONTROL = 0x08,
            /// <summary>
            /// Causes the accelerator to be activated only if the ALT key is down. 
            /// Applies only to virtual keys.
            /// </summary>
            ALT = 0x10
        }

        /// <summary>
        /// Virtual key definitions.
        /// </summary>
        public enum VirtualKeys : uint
        {
            /// <summary>
            /// Standard virtual left mouse button.
            /// </summary>
            VK_LBUTTON = 0x01,
            /// <summary>
            /// Standard virtual right mouse button.
            /// </summary>
            VK_RBUTTON = 0x02,
            /// <summary>
            /// Ctrl-Break / Ctrl-C.
            /// </summary>
            VK_CANCEL = 0x03,
            /// <summary>
            /// Standard virtual middle mouse button.
            /// </summary>
            VK_MBUTTON = 0x04,
            /// <summary>
            /// 
            /// </summary>
            VK_XBUTTON1 = 0x05,
            /// <summary>
            /// 
            /// </summary>
            VK_XBUTTON2 = 0x06,
            /// <summary>
            /// Backspace.
            /// </summary>
            VK_BACK = 0x08,
            /// <summary>
            /// Tab.
            /// </summary>
            VK_TAB = 0x09,
            /// <summary>
            /// Delete.
            /// </summary>
            VK_CLEAR = 0x0C,
            /// <summary>
            /// Return.
            /// </summary>
            VK_RETURN = 0x0D,
            /// <summary>
            /// Shift.
            /// </summary>
            VK_SHIFT = 0x10,
            /// <summary>
            /// Control.
            /// </summary>
            VK_CONTROL = 0x11,
            /// <summary>
            /// Menu.
            /// </summary>
            VK_MENU = 0x12,
            /// <summary>
            /// Pause.
            /// </summary>
            VK_PAUSE = 0x13,
            /// <summary>
            /// Caps lock.
            /// </summary>
            VK_CAPITAL = 0x14,
            /// <summary>
            /// 
            /// </summary>
            VK_KANA = 0x15,
            /// <summary>
            ///
            /// </summary>
            VK_JUNJA = 0x17,
            /// <summary>
            /// 
            /// </summary>
            VK_FINAL = 0x18,
            /// <summary>
            /// 
            /// </summary>
            VK_KANJI = 0x19,
            /// <summary>
            /// Escape.
            /// </summary>
            VK_ESCAPE = 0x1B,
            /// <summary>
            /// 
            /// </summary>
            VK_CONVERT = 0x1C,
            /// <summary>
            ///
            /// </summary>
            VK_NONCONVERT = 0x1D,
            /// <summary>
            ///
            /// </summary>
            VK_ACCEPT = 0x1E,
            /// <summary>
            ///
            /// </summary>
            VK_MODECHANGE = 0x1F,
            /// <summary>
            /// Space.
            /// </summary>
            VK_SPACE = 0x20,
            /// <summary>
            ///
            /// </summary>
            VK_PRIOR = 0x21,
            /// <summary>
            ///
            /// </summary>
            VK_NEXT = 0x22,
            /// <summary>
            /// End.
            /// </summary>
            VK_END = 0x23,
            /// <summary>
            /// Home.
            /// </summary>
            VK_HOME = 0x24,
            /// <summary>
            /// Left arrow.
            /// </summary>
            VK_LEFT = 0x25,
            /// <summary>
            /// Up arrow.
            /// </summary>
            VK_UP = 0x26,
            /// <summary>
            /// Right arrow.
            /// </summary>
            VK_RIGHT = 0x27,
            /// <summary>
            /// Down arrow.
            /// </summary>
            VK_DOWN = 0x28,
            /// <summary>
            ///
            /// </summary>
            VK_SELECT = 0x29,
            /// <summary>
            /// Print Screen.
            /// </summary>
            VK_PRINT = 0x2A,
            /// <summary>
            ///
            /// </summary>
            VK_EXECUTE = 0x2B,
            /// <summary>
            ///
            /// </summary>
            VK_SNAPSHOT = 0x2C,
            /// <summary>
            /// Insert.
            /// </summary>
            VK_INSERT = 0x2D,
            /// <summary>
            /// Delete.
            /// </summary>
            VK_DELETE = 0x2E,
            /// <summary>
            ///
            /// </summary>
            VK_HELP = 0x2F,
            /// <summary>
            ///
            /// </summary>
            VK_LWIN = 0x5B,
            /// <summary>
            ///
            /// </summary>
            VK_RWIN = 0x5C,
            /// <summary>
            ///
            /// </summary>
            VK_APPS = 0x5D,
            /// <summary>
            ///
            /// </summary>
            VK_SLEEP = 0x5F,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD0 = 0x60,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD1 = 0x61,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD2 = 0x62,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD3 = 0x63,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD4 = 0x64,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD5 = 0x65,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD6 = 0x66,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD7 = 0x67,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD8 = 0x68,
            /// <summary>
            ///
            /// </summary>
            VK_NUMPAD9 = 0x69,
            /// <summary>
            ///
            /// </summary>
            VK_MULTIPLY = 0x6A,
            /// <summary>
            ///
            /// </summary>
            VK_ADD = 0x6B,
            /// <summary>
            ///
            /// </summary>
            VK_SEPARATOR = 0x6C,
            /// <summary>
            ///
            /// </summary>
            VK_SUBTRACT = 0x6D,
            /// <summary>
            ///
            /// </summary>
            VK_DECIMAL = 0x6E,
            /// <summary>
            ///
            /// </summary>
            VK_DIVIDE = 0x6F,
            /// <summary>
            ///
            /// </summary>
            VK_F1 = 0x70,
            /// <summary>
            ///
            /// </summary>
            VK_F2 = 0x71,
            /// <summary>
            ///
            /// </summary>
            VK_F3 = 0x72,
            /// <summary>
            ///
            /// </summary>
            VK_F4 = 0x73,
            /// <summary>
            ///
            /// </summary>
            VK_F5 = 0x74,
            /// <summary>
            ///
            /// </summary>
            VK_F6 = 0x75,
            /// <summary>
            ///
            /// </summary>
            VK_F7 = 0x76,
            /// <summary>
            ///
            /// </summary>
            VK_F8 = 0x77,
            /// <summary>
            ///
            /// </summary>
            VK_F9 = 0x78,
            /// <summary>
            ///
            /// </summary>
            VK_F10 = 0x79,
            /// <summary>
            ///
            /// </summary>
            VK_F11 = 0x7A,
            /// <summary>
            ///
            /// </summary>
            VK_F12 = 0x7B,
            /// <summary>
            ///
            /// </summary>
            VK_F13 = 0x7C,
            /// <summary>
            ///
            /// </summary>
            VK_F14 = 0x7D,
            /// <summary>
            ///
            /// </summary>
            VK_F15 = 0x7E,
            /// <summary>
            ///
            /// </summary>
            VK_F16 = 0x7F,
            /// <summary>
            ///
            /// </summary>
            VK_F17 = 0x80,
            /// <summary>
            ///
            /// </summary>
            VK_F18 = 0x81,
            /// <summary>
            ///
            /// </summary>
            VK_F19 = 0x82,
            /// <summary>
            ///
            /// </summary>
            VK_F20 = 0x83,
            /// <summary>
            ///
            /// </summary>
            VK_F21 = 0x84,
            /// <summary>
            ///
            /// </summary>
            VK_F22 = 0x85,
            /// <summary>
            ///
            /// </summary>
            VK_F23 = 0x86,
            /// <summary>
            ///
            /// </summary>
            VK_F24 = 0x87,
            /// <summary>
            ///
            /// </summary>
            VK_NUMLOCK = 0x90,
            /// <summary>
            ///
            /// </summary>
            VK_SCROLL = 0x91,
            /// <summary>
            /// NEC PC-9800 keyboard '=' key on numpad.
            /// </summary>
            VK_OEM_NEC_EQUAL = 0x92,
            /// <summary>
            /// Fujitsu/OASYS keyboard 'Dictionary' key.
            /// </summary>
            VK_OEM_FJ_JISHO = 0x92,
            /// <summary>
            /// Fujitsu/OASYS keyboard 'Unregister word' key.
            /// </summary>
            VK_OEM_FJ_MASSHOU = 0x93,
            /// <summary>
            /// Fujitsu/OASYS keyboard 'Register word' key.
            /// </summary>
            VK_OEM_FJ_TOUROKU = 0x94,
            /// <summary>
            /// Fujitsu/OASYS keyboard 'Left OYAYUBI' key.
            /// </summary>
            VK_OEM_FJ_LOYA = 0x95,
            /// <summary>
            /// Fujitsu/OASYS keyboard 'Right OYAYUBI' key.
            /// </summary>
            VK_OEM_FJ_ROYA = 0x96,
            /// <summary>
            ///
            /// </summary>
            VK_LSHIFT = 0xA0,
            /// <summary>
            ///
            /// </summary>
            VK_RSHIFT = 0xA1,
            /// <summary>
            ///
            /// </summary>
            VK_LCONTROL = 0xA2,
            /// <summary>
            ///
            /// </summary>
            VK_RCONTROL = 0xA3,
            /// <summary>
            ///
            /// </summary>
            VK_LMENU = 0xA4,
            /// <summary>
            ///
            /// </summary>
            VK_RMENU = 0xA5,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_BACK = 0xA6,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_FORWARD = 0xA7,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_REFRESH = 0xA8,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_STOP = 0xA9,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_SEARCH = 0xAA,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_FAVORITES = 0xAB,
            /// <summary>
            ///
            /// </summary>
            VK_BROWSER_HOME = 0xAC,
            /// <summary>
            ///
            /// </summary>
            VK_VOLUME_MUTE = 0xAD,
            /// <summary>
            ///
            /// </summary>
            VK_VOLUME_DOWN = 0xAE,
            /// <summary>
            ///
            /// </summary>
            VK_VOLUME_UP = 0xAF,
            /// <summary>
            ///
            /// </summary>
            VK_MEDIA_NEXT_TRACK = 0xB0,
            /// <summary>
            ///
            /// </summary>
            VK_MEDIA_PREV_TRACK = 0xB1,
            /// <summary>
            ///
            /// </summary>
            VK_MEDIA_STOP = 0xB2,
            /// <summary>
            ///
            /// </summary>
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            /// <summary>
            ///
            /// </summary>
            VK_LAUNCH_MAIL = 0xB4,
            /// <summary>
            ///
            /// </summary>
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            /// <summary>
            ///
            /// </summary>
            VK_LAUNCH_APP1 = 0xB6,
            /// <summary>
            ///
            /// </summary>
            VK_LAUNCH_APP2 = 0xB7,
            /// <summary>
            /// ';:' for US
            /// </summary>
            VK_OEM_1 = 0xBA,
            /// <summary>
            /// '+' any country
            /// </summary>
            VK_OEM_PLUS = 0xBB,
            /// <summary>
            /// ',' any country
            /// </summary>
            VK_OEM_COMMA = 0xBC,
            /// <summary>
            /// '-' any country
            /// </summary>
            VK_OEM_MINUS = 0xBD,
            /// <summary>
            /// '.' any country
            /// </summary>
            VK_OEM_PERIOD = 0xBE,
            /// <summary>
            /// '/?' for US
            /// </summary>
            VK_OEM_2 = 0xBF,
            /// <summary>
            /// '`~' for US
            /// </summary>
            VK_OEM_3 = 0xC0,
            /// <summary>
            /// '[{' for US
            /// </summary>
            VK_OEM_4 = 0xDB,
            /// <summary>
            /// '\|' for US
            /// </summary>
            VK_OEM_5 = 0xDC,
            /// <summary>
            /// ']}' for US
            /// </summary>
            VK_OEM_6 = 0xDD,
            /// <summary>
            /// ''"' for US
            /// </summary>
            VK_OEM_7 = 0xDE,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_8 = 0xDF,
            /// <summary>
            /// 'AX' key on Japanese AX kbd
            /// </summary>
            VK_OEM_AX = 0xE1,
            /// <summary>
            /// "&lt;&gt;" or "\|" on RT 102-key kbd.
            /// </summary>
            VK_OEM_102 = 0xE2,
            /// <summary>
            /// Help key on ICO
            /// </summary>
            VK_ICO_HELP = 0xE3,
            /// <summary>
            /// 00 key on ICO
            /// </summary>
            VK_ICO_00 = 0xE4,
            /// <summary>
            ///
            /// </summary>
            VK_PROCESSKEY = 0xE5,
            /// <summary>
            ///
            /// </summary>
            VK_ICO_CLEAR = 0xE6,
            /// <summary>
            ///
            /// </summary>
            VK_PACKET = 0xE7,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_RESET = 0xE9,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_JUMP = 0xEA,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_PA1 = 0xEB,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_PA2 = 0xEC,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_PA3 = 0xED,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_WSCTRL = 0xEE,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_CUSEL = 0xEF,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_ATTN = 0xF0,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_FINISH = 0xF1,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_COPY = 0xF2,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_AUTO = 0xF3,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_ENLW = 0xF4,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_BACKTAB = 0xF5,
            /// <summary>
            ///
            /// </summary>
            VK_ATTN = 0xF6,
            /// <summary>
            ///
            /// </summary>
            VK_CRSEL = 0xF7,
            /// <summary>
            ///
            /// </summary>
            VK_EXSEL = 0xF8,
            /// <summary>
            ///
            /// </summary>
            VK_EREOF = 0xF9,
            /// <summary>
            ///
            /// </summary>
            VK_PLAY = 0xFA,
            /// <summary>
            ///
            /// </summary>
            VK_ZOOM = 0xFB,
            /// <summary>
            ///
            /// </summary>
            VK_NONAME = 0xFC,
            /// <summary>
            ///
            /// </summary>
            VK_PA1 = 0xFD,
            /// <summary>
            ///
            /// </summary>
            VK_OEM_CLEAR = 0xFE
        }

        /// <summary>
        /// Contains information about an individual font in a font resource group. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct FONTDIRENTRY
        {
            /// <summary>
            /// Specifies a user-defined version number for the resource data that tools can 
            /// use to read and write resource files. 
            /// </summary>
            public UInt16 dfVersion;
            /// <summary>
            /// Specifies the size of the file, in bytes. 
            /// </summary>
            public UInt32 dfSize;
            /// <summary>
            /// Contains a 60-character string with the font supplier's copyright information.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
            public string dfCopyright;
            /// <summary>
            /// Specifies the type of font file.
            /// </summary>
            public UInt16 dfType;
            /// <summary>
            /// Specifies the point size at which this character set looks best. 
            /// </summary>
            public UInt16 dfPoints;
            /// <summary>
            /// Specifies the vertical resolution, in dots per inch, at which this character set was digitized.
            /// </summary>
            public UInt16 dfVertRes;
            /// <summary>
            /// Specifies the horizontal resolution, in dots per inch, at which this character set was digitized.
            /// </summary>
            public UInt16 dfHorizRes;
            /// <summary>
            /// Specifies the distance from the top of a character definition cell to the baseline of the 
            /// typographical font. 
            /// </summary>
            public UInt16 dfAscent;
            /// <summary>
            /// Specifies the amount of leading inside the bounds set by the dfPixHeight member. Accent marks and 
            /// other diacritical characters can occur in this area. 
            /// </summary>
            public UInt16 dfInternalLeading;
            /// <summary>
            /// Specifies the amount of extra leading that the application adds between rows. 
            /// </summary>
            public UInt16 dfExternalLeading;
            /// <summary>
            /// Specifies an italic font if not equal to zero.
            /// </summary>
            public byte dfItalic;
            /// <summary>
            /// Specifies an underlined font if not equal to zero.
            /// </summary>
            public byte dfUnderline;
            /// <summary>
            /// Specifies a strikeout font if not equal to zero.
            /// </summary>
            public byte dfStrikeOut;
            /// <summary>
            /// Specifies the weight of the font in the range 0 through 1000. For example, 400 is roman and 
            /// 700 is bold. If this value is zero, a default weight is used.
            /// </summary>
            public UInt16 dfWeight;
            /// <summary>
            /// Specifies the character set of the font.
            /// </summary>
            public byte dfCharSet;
            /// <summary>
            /// Specifies the width of the grid on which a vector font was digitized. For raster fonts, 
            /// if the member is not equal to zero, it represents the width for all the characters in the 
            /// bitmap. If the member is equal to zero, the font has variable-width characters. 
            /// </summary>
            public UInt16 dfPixWidth;
            /// <summary>
            /// Specifies the height of the character bitmap for raster fonts or the height of the grid 
            /// on which a vector font was digitized. 
            /// </summary>
            public UInt16 dfPixHeight;
            /// <summary>
            /// Specifies the pitch and the family of the font.
            /// </summary>
            public byte dfPitchAndFamily;
            /// <summary>
            /// Specifies the average width of characters in the font (generally defined as the width of 
            /// the letter x). This value does not include the overhang required for bold or italic characters. 
            /// </summary>
            public UInt16 dfAvgWidth;
            /// <summary>
            /// Specifies the width of the widest character in the font.
            /// </summary>
            public UInt16 dfMaxWidth;
            /// <summary>
            /// Specifies the first character code defined in the font.
            /// </summary>
            public byte dfFirstChar;
            /// <summary>
            /// Specifies the last character code defined in the font.
            /// </summary>
            public byte dfLastChar;
            /// <summary>
            /// Specifies the character to substitute for characters not in the font.
            /// </summary>
            public byte dfDefaultChar;
            /// <summary>
            /// Specifies the character that will be used to define word breaks for text justification.
            /// </summary>
            public byte dfBreakChar;
            /// <summary>
            /// Specifies the number of bytes in each row of the bitmap. This value is always even so 
            /// that the rows start on word boundaries. For vector fonts, this member has no meaning. 
            /// </summary>
            public UInt16 dfWidthBytes;
            /// <summary>
            /// Specifies the offset in the file to a null-terminated string that specifies a device name. 
            /// For a generic font, this value is zero. 
            /// </summary>
            public UInt32 dfDevice;
            /// <summary>
            /// Specifies the offset in the file to a null-terminated string that names the typeface. 
            /// </summary>
            public UInt32 dfFace;
            /// <summary>
            /// This member is reserved.
            /// </summary>
            public UInt32 dfReserved;
        };
    }
}
