using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace xServer.Controls.HexEditor
{
    public class Caret
    {
        #region Field

        /// <summary>
        /// Contains the start index
        /// where the caret started
        /// </summary>
        int _startIndex;

        /// <summary>
        /// Contains the end index
        /// where the caret is 
        /// currently located
        /// </summary>
        int _endIndex;

        /// <summary>
        /// Tells if the given caret
        /// is active in the controller
        /// (control is in focus)
        /// </summary>
        bool _isCaretActive;


        /// <summary>
        /// Tells if the caret is 
        /// currently hidden or 
        /// not (out of view)
        /// </summary>
        bool _isCaretHidden;

        /// <summary>
        /// Holds the actual position
        /// of the caret
        /// </summary>
        Point _location;

        private HexEditor _editor;

        #endregion

        #region Properties

        public int SelectionStart
        {
            get
            {
                if (_endIndex < _startIndex)
                    return _endIndex;
                return _startIndex;
            }
        }

        public int SelectionLength
        {
            get
            {
                if (_endIndex < _startIndex)
                    return _startIndex - _endIndex;
                return _endIndex - _startIndex;
            }
        }

        public bool Focused
        {
            get { return _isCaretActive; }
        }

        public int CurrentIndex
        {
            get { return _endIndex; }
        }

        public Point Location
        {
            get { return _location; }
        }

        #endregion

        #region EventHandlers

        public event EventHandler SelectionStartChanged;

        public event EventHandler SelectionLengthChanged;

        #endregion

        #region Constructor

        public Caret(HexEditor editor)
        {
            _editor = editor;
            _isCaretActive = false;
            _startIndex = 0;
            _endIndex = 0;
            _isCaretHidden = true;
            _location = new Point(0, 0);
        }

        #endregion

        #region Methods

        #region Caret

        private bool Create(IntPtr hWHandler)
        {
            if (!_isCaretActive)
            {
                _isCaretActive = true;
                return CreateCaret(hWHandler, IntPtr.Zero, 0, (int)_editor.CharSize.Height - 2);
            }

            return false;
        }

        private bool Show(IntPtr hWnd)
        {
            if (_isCaretActive)
            {
                _isCaretHidden = false;
                return ShowCaret(hWnd);
            }

            return false;
        }

        public bool Hide(IntPtr hWnd)
        {
            if (_isCaretActive && !_isCaretHidden)
            {
                _isCaretHidden = true;
                return HideCaret(hWnd);
            }
            return false;
        }

        public bool Destroy()
        {
            if (_isCaretActive)
            {
                _isCaretActive = false;
                DeSelect();
                DestroyCaret();
            }

            return false;
        }

        #endregion

        public void SetStartIndex(int index)
        {
            _startIndex = index;
            _endIndex = _startIndex;

            if (SelectionStartChanged != null)
                SelectionStartChanged(this, EventArgs.Empty);

            if (SelectionLengthChanged != null)
                SelectionLengthChanged(this, EventArgs.Empty);

        }

        public void SetEndIndex(int index)
        {
            _endIndex = index;

            if (SelectionStartChanged != null)
                SelectionStartChanged(this, EventArgs.Empty);

            if (SelectionLengthChanged != null)
                SelectionLengthChanged(this, EventArgs.Empty);
        }

        public void SetCaretLocation(Point start)
        {
            Create(_editor.Handle);
            _location = start;
            SetCaretPos(_location.X, _location.Y);
            Show(_editor.Handle);
        }

        public bool IsSelected(int byteIndex)
        {
            return (SelectionStart <= byteIndex && byteIndex < (SelectionStart + SelectionLength));
        }

        private void DeSelect()
        {
            if (_endIndex < _startIndex)
                _startIndex = _endIndex;
            else
                _endIndex = _startIndex;

            if (SelectionStartChanged != null)
                SelectionStartChanged(this, EventArgs.Empty);

            if (SelectionLengthChanged != null)
                SelectionLengthChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Caret import

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyCaret();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool HideCaret(IntPtr hWnd);

        #endregion
    }
}
