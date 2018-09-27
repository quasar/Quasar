using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls.HexEditor
{
    /*
    * Derived and Adapted from Bernhard Elbl 
    * Be.HexEditor v1.6 (Last Update: Dec 27, 2013).
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * This code (and the rest that is avaliable in the
    * HexEditor-folder) has been derived from Bernhard
    * ElblBe's Be.HexEditor v1.6, modifications have 
    * been made to make it better fit the new application
    * area. 
    * First Modified by StingRaptor on Febuary 7, 2016
    */
    /* [Original License, ONLY for Current HexEditor Folder]
     * This license is applied only to the files in the current
     * HexEditor Folder
     
     * The MIT License

        Copyright (c) 2011 Bernhard Elbl

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in
        all copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
        THE SOFTWARE.
    */
    /*
    * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    * Unmodified Source:
    * http://sourceforge.net/projects/hexbox/?source=navbar
    */

    public class HexEditor : Control
    {
        #region Locks

        private object _caretLock = new object();

        private object _hexTableLock = new object();

        #endregion

        #region IKeyMouseEventHandlers

        private IKeyMouseEventHandler _handler;

        #endregion

        #region EditView

        private EditView _editView;

        #endregion

        #region Fields

        /// <summary>
        /// Contains all of the bytes that
        /// are to be displayed in the 
        /// HexEditor
        /// </summary>
        ByteCollection _hexTable;

        /// <summary>
        /// Contains the type of counter
        /// for the linecount
        /// </summary>
        string _lineCountCaps = "X";

        /// <summary>
        /// Contains the number of chars
        /// to be used in the line count
        /// </summary>
        int _nrCharsLineCount = 4;

        /// <summary>
        /// Contains the caret for the
        /// control
        /// </summary>
        Caret _caret;

        #region Boundarys

        /// <summary>
        /// Contains the bound for everything that
        /// is to be displayed in the control
        /// </summary>
        Rectangle _recContent;

        /// <summary>
        /// Contains the boundary for every 
        /// line count for the control
        /// </summary>
        Rectangle _recLineCount;

        #endregion

        #region String Format

        /// <summary>
        /// Contains the format of the line count
        /// string that is presented in this control
        /// </summary>
        StringFormat _stringFormat;

        #endregion

        #region Byte specific

        /// <summary>
        /// Contains the index of the first 
        /// visible byte
        /// </summary>
        int _firstByte;

        /// <summary>
        /// Contains the index of the last 
        /// visible byte
        /// </summary>
        int _lastByte;

        /// <summary>
        /// Contains the maximum bytes that
        /// can be visible horizontally
        /// </summary>
        int _maxBytesH;

        /// <summary>
        /// Contains the maximum bytes that
        /// can be visible vertically
        /// </summary>
        int _maxBytesV;

        /// <summary>
        /// Contains the maximum number of 
        /// bytes that can be visible at a 
        /// time.
        /// </summary>
        int _maxBytes;

        /// <summary>
        /// Contains the maximum number of 
        /// rows with bytes that are fully 
        /// visible.
        /// </summary>
        int _maxVisibleBytesV;

        #endregion

        #region Scrollbar

        /// <summary>
        /// Contains the vertical scroll
        /// bar for the control
        /// </summary>
        VScrollBar _vScrollBar;

        /// <summary>
        /// Contains the with of the scrollbar
        /// in pixels
        /// </summary>
        int _vScrollBarWidth = 20;

        /// <summary>
        /// Contains the current position of the 
        /// scrollbar
        /// </summary>
        int _vScrollPos;

        /// <summary>
        /// Contains the maximum value that
        /// the scrollbar can have
        /// </summary>
        int _vScrollMax;

        /// <summary>
        /// Contains the minimum value
        /// that the scrollbar may have
        /// </summary>
        int _vScrollMin;

        /// <summary>
        /// Contains the value for the 
        /// size of a  smallchange in the
        /// scrollbar
        /// </summary>
        int _vScrollSmall;

        /// <summary>
        /// Contains the value for the 
        /// size of a largechange in the
        /// scrollbar
        /// </summary>
        int _vScrollLarge;

        #endregion

        #endregion

        #region Properties

        #region enums

        public enum CaseStyle { LowerCase, UpperCase }

        #endregion

        #region Overriden

        //Override font to trigger update on font change
        public override Font Font
        {
            set
            {
                base.Font = value;

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        //Hides the property Text that is not used for the control
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        #endregion

        #region Hidden

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] HexTable
        {
            get
            {
                lock (_hexTableLock)
                {
                    return _hexTable.ToArray();
                }
            }
            set
            {
                lock (_hexTableLock)
                {
                    if (value == _hexTable.ToArray())
                        return;

                    _hexTable = new ByteCollection(value);
                }
                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF CharSize
        {
            get { return _charSize; }
            private set
            {
                if (_charSize == value)
                    return;

                _charSize = value;

                if (CharSizeChanged != null)
                    CharSizeChanged(this, EventArgs.Empty);
            }
        }SizeF _charSize;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxBytesV
        {
            get { return _maxBytesV; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FirstVisibleByte
        {
            get { return _firstByte; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LastVisibleByte
        {
            get { return _lastByte; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VScrollBarHidden
        {
            get { return _isVScrollHidden; }
            set
            {
                if (_isVScrollHidden == value)
                    return;

                _isVScrollHidden = value;

                if (!_isVScrollHidden)
                    Controls.Add(_vScrollBar);
                else
                    Controls.Remove(_vScrollBar);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }bool _isVScrollHidden = true;

        #endregion

        #region Visible

        /// <summary>
        /// Contains the number of bytes to
        /// display per line
        /// </summary>
        [DefaultValue(8), Category("Hex"), Description("Property that specifies the number of bytes to display per line.")]
        public int BytesPerLine
        {
            get { return _bytesPerLine; }
            set
            {
                if (_bytesPerLine == value)
                    return;

                _bytesPerLine = value;
                UpdateRectanglePositioning();
                Invalidate();
            }
        }int _bytesPerLine = 8;


        /// <summary>
        /// Contains the margin between each
        /// of the entitys
        /// </summary>
        [DefaultValue(10), Category("Hex"), Description("Property that specifies the margin between each of the entitys in the control.")]
        public int EntityMargin
        {
            get { return _entityMargin; }
            set
            {
                if (_entityMargin == value)
                    return;

                _entityMargin = value;
                UpdateRectanglePositioning();
                Invalidate();
            }
        }int _entityMargin = 10;

        /// <summary>
        /// Contains the type of border 
        /// that is used for the control
        /// </summary>
        [DefaultValue(BorderStyle.Fixed3D), Category("Appearance"), Description("Indicates where the control should have a border.")]
        public BorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                if (_borderStyle == value)
                    return;

                if (value != BorderStyle.FixedSingle)
                    _borderColor = Color.Empty;

                _borderStyle = value;
                UpdateRectanglePositioning();
                Invalidate();
            }
        }BorderStyle _borderStyle = BorderStyle.Fixed3D;

        /// <summary>
        /// Contains the color for the border 
        /// that is used for the control 
        /// (Only used when BorderStyle is FixedSingle)
        /// </summary>
        [DefaultValue(typeof(Color), "Empty"), Category("Appearance"), Description("Indicates the color to be used when displaying a FixedSingle border.")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (BorderStyle != BorderStyle.FixedSingle || _borderColor == value)
                    return;

                _borderColor = value;
                Invalidate();
            }
        }Color _borderColor = Color.Empty;

        /// <summary>
        /// Contains the color of the selected
        /// background area in the control
        /// </summary>
        [DefaultValue(typeof(Color), "Blue"), Category("Hex"), Description("Property for the background color of the selected text areas.")]
        public Color SelectionBackColor
        {
            get { return _selectionBackColor; }
            set
            {
                if (_selectionBackColor == value)
                    return;

                _selectionBackColor = value;
            }
        }Color _selectionBackColor = Color.Blue;

        /// <summary>
        /// Contains the color of the selected
        /// foreground area in the control
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("Hex"), Description("Property for the foreground color of the selected text areas.")]
        public Color SelectionForeColor
        {
            get { return _selectionForeColor; }
            set
            {
                if (_selectionForeColor == value)
                    return;

                _selectionForeColor = value;
            }
        }Color _selectionForeColor = Color.White;

        /// <summary>
        /// Contains the case type for the
        /// line counter
        /// </summary>
        [DefaultValue(CaseStyle.UpperCase), Category("Hex"), Description("Property for the case type to use on the line counter.")]
        public CaseStyle LineCountCaseStyle
        {
            get { return _lineCountCaseStyle; }
            set
            {
                if (_lineCountCaseStyle == value)
                    return;

                _lineCountCaseStyle = value;

                if (_lineCountCaseStyle == CaseStyle.LowerCase)
                    _lineCountCaps = "x";
                else
                    _lineCountCaps = "X";

                Invalidate();
            }
        }CaseStyle _lineCountCaseStyle = CaseStyle.UpperCase;

        /// <summary>
        /// Contains the case type for the
        /// hex view.
        /// </summary>
        [DefaultValue(CaseStyle.UpperCase), Category("Hex"), Description("Property for the case type to use for the hexadecimal values view.")]
        public CaseStyle HexViewCaseStyle
        {
            get { return _hexViewCaseStyle; }
            set
            {
                if (_hexViewCaseStyle == value)
                    return;

                _hexViewCaseStyle = value;

                if (_hexViewCaseStyle == CaseStyle.LowerCase)
                    _editView.SetLowerCase();
                else
                    _editView.SetUpperCase();

                Invalidate();
            }
        }CaseStyle _hexViewCaseStyle = CaseStyle.UpperCase;

        /// <summary>
        /// Property that contains if the
        /// vertical scrollbar should be 
        /// visible or not
        /// </summary>
        [DefaultValue(false), Category("Hex"), Description("Property for the visibility of the vertical scrollbar.")]
        public bool VScrollBarVisisble
        {
            get { return _isVScrollVisible; }
            set
            {
                if (_isVScrollVisible == value)
                    return;

                _isVScrollVisible = value;

                UpdateRectanglePositioning();
                Invalidate();
            }
        }bool _isVScrollVisible = false;

        #endregion

        #endregion

        #region EventHandlers

        [Description("Event that is triggered whenever the hextable has been changed.")]
        public event EventHandler HexTableChanged;

        [Description("Event that is triggered whenever the SelectionStart value is changed.")]
        public event EventHandler SelectionStartChanged;

        [Description("Event that is triggered whenever the SelectionLength value is changed.")]
        public event EventHandler SelectionLengthChanged;

        [Description("Event that is triggered whenever the size of the char is changed.")]
        public event EventHandler CharSizeChanged;

        #endregion

        #region Events

        protected void OnVScrollBarScroll(object sender, ScrollEventArgs e)
        {
            switch (e.Type)
            {
                case ScrollEventType.SmallIncrement:
                    ScrollLineDown(1);
                    break;
                case ScrollEventType.SmallDecrement:
                    ScrollLineUp(1);
                    break;
                case ScrollEventType.LargeIncrement:
                    ScrollLineDown(_vScrollLarge);
                    break;
                case ScrollEventType.LargeDecrement:
                    ScrollLineUp(_vScrollLarge);
                    break;
                case ScrollEventType.ThumbTrack:
                    ScrollThumbTrack(e.NewValue - e.OldValue);
                    break;
            }
            Invalidate();
        }

        protected void CaretSelectionStartChanged(object sender, EventArgs e)
        {
            if (SelectionStartChanged != null)
                SelectionStartChanged(this, e);
        }

        protected void CaretSelectionLengthChanged(object sender, EventArgs e)
        {
            if (SelectionLengthChanged != null)
                SelectionLengthChanged(this, e);
        }

        #region Overriden

        protected override void OnMarginChanged(EventArgs e)
        {
            base.OnMarginChanged(e);
            UpdateRectanglePositioning();
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (_handler != null)
                _handler.OnGotFocus(e);

            UpdateRectanglePositioning();
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _dragging = false;
            DestroyCaret();
            base.OnLostFocus(e);
        }

        #endregion

        #endregion

        #region KeyEvents

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_handler != null)
                _handler.OnKeyPress(e);

            base.OnKeyPress(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
            {
                if (!_isVScrollHidden)
                {
                    ScrollLineDown(_vScrollLarge);
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                if (!_isVScrollHidden)
                {
                    ScrollLineUp(_vScrollLarge);
                    Invalidate();
                }
            }
            else
            {
                if (_handler != null)
                    _handler.OnKeyDown(e);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_handler != null)
                _handler.OnKeyUp(e);

            base.OnKeyUp(e);
        }

        #endregion

        #region MouseEvents

        //Indicates if the mouse is currently being dragged or not
        private bool _dragging;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.Focused)
            {
                if (_handler != null)
                    _handler.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                {
                    _dragging = true;
                    Invalidate();
                }
            }
            else
            {
                this.Focus();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Focused)
            {
                if (_dragging)
                {
                    if (_handler != null)
                        _handler.OnMouseDragged(e);

                    Invalidate();
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;

            if (this.Focused)
            {
                if (_handler != null)
                    _handler.OnMouseUp(e);
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.Focused)
            {
                if (_handler != null)
                    _handler.OnMouseDoubleClick(e);
            }

            base.OnMouseDoubleClick(e);
        }

        #endregion

        #region Caret

        #region Properties

        public int CaretPosX
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.Location.X;
                }
            }
        }

        public int CaretPosY
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.Location.Y;
                }
            }
        }

        public int SelectionStart
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.SelectionStart;
                }
            }
        }

        public int SelectionLength
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.SelectionLength;
                }
            }
        }

        public int CaretIndex
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.CurrentIndex;
                }
            }
        }

        public bool CaretFocused
        {
            get
            {
                lock (_caretLock)
                {
                    return _caret.Focused;
                }
            }
        }

        #endregion

        #region Methods

        public void SetCaretStart(int index, Point location)
        {
            location = ScrollToCaret(index, location);

            lock (_caretLock)
            {
                _caret.SetStartIndex(index);
                _caret.SetCaretLocation(location);
            }

            Invalidate();
        }

        public void SetCaretEnd(int index, Point location)
        {
            location = ScrollToCaret(index, location);

            lock (_caretLock)
            {
                _caret.SetEndIndex(index);
                _caret.SetCaretLocation(location);
            }

            Invalidate();
        }

        public bool IsSelected(int byteIndex)
        {
            lock (_caretLock)
            {
                return _caret.IsSelected(byteIndex);
            }
        }

        public void DestroyCaret()
        {
            lock (_caretLock)
            {
                _caret.Destroy();
            }
        }

        #endregion

        #endregion

        #region HexTable

        #region Properties

        public int HexTableLength
        {
            get
            {
                lock (_hexTableLock)
                {
                    return _hexTable.Length;
                }
            }
        }

        #endregion

        #region Methods

        public void RemoveSelectedBytes()
        {
            int index = SelectionStart;
            int length = SelectionLength;
            if (length > 0)
            {
                lock (_hexTableLock)
                {
                    _hexTable.RemoveRange(index, length);
                }

                UpdateRectanglePositioning();
                Invalidate();

                if (HexTableChanged != null)
                    HexTableChanged(this, EventArgs.Empty);
            }
        }

        public void RemoveByteAt(int index)
        {
            lock (_hexTableLock)
            {
                _hexTable.RemoveAt(index);
            }

            UpdateRectanglePositioning();
            Invalidate();

            if (HexTableChanged != null)
                HexTableChanged(this, EventArgs.Empty);
        }

        public void AppendByte(byte item)
        {
            lock (_hexTableLock)
            {
                _hexTable.Add(item);
            }

            UpdateRectanglePositioning();
            Invalidate();

            if (HexTableChanged != null)
                HexTableChanged(this, EventArgs.Empty);
        }

        public void InsertByte(int index, byte item)
        {
            lock (_hexTableLock)
            {
                _hexTable.Insert(index, item);
            }

            UpdateRectanglePositioning();
            Invalidate();

            if (HexTableChanged != null)
                HexTableChanged(this, EventArgs.Empty);
        }

        public char GetByteAsChar(int index)
        {
            lock (_hexTableLock)
            {
                return _hexTable.GetCharAt(index);
            }
        }

        public byte GetByte(int index)
        {
            lock (_hexTableLock)
            {
                return _hexTable.GetAt(index);
            }
        }

        public void SetByte(int index, byte item)
        {
            lock (_hexTableLock)
            {
                _hexTable.SetAt(index, item);
            }

            Invalidate();

            if (HexTableChanged != null)
                HexTableChanged(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Scrollbar

        public void ScrollLineUp(int lines)
        {
            if (_firstByte > 0)
            {
                lines = lines > _vScrollPos ? _vScrollPos : lines;
                //Scroll up
                _vScrollPos -= _vScrollSmall * lines;
                //Update the visible bytes
                UpdateVisibleByteIndex();
                UpdateScrollValues();
                //Update the Caret
                if (CaretFocused)
                {
                    Point caretLocation = new Point(CaretPosX, CaretPosY);
                    caretLocation.Y += _recLineCount.Height * lines;

                    lock (_caretLock)
                    {
                        _caret.SetCaretLocation(caretLocation);
                    }
                }
            }
        }

        public void ScrollLineDown(int lines)
        {
            if (_vScrollPos <= _vScrollMax - _vScrollLarge)
            {
                lines = (lines + _vScrollPos) > (_vScrollMax - _vScrollLarge) ? (_vScrollMax - _vScrollLarge) - _vScrollPos + 1 : lines;
                //Scroll up
                _vScrollPos += _vScrollSmall * lines;
                //Update the visible bytes
                UpdateVisibleByteIndex();
                UpdateScrollValues();
                //Update the Caret
                if (CaretFocused)
                {
                    Point caretLocation = new Point(CaretPosX, CaretPosY);
                    caretLocation.Y -= _recLineCount.Height * lines;

                    lock (_caretLock)
                    {
                        _caret.SetCaretLocation(caretLocation);
                        if (caretLocation.Y < _recContent.Y)
                            _caret.Hide(this.Handle);
                    }
                }
            }
        }

        public void ScrollThumbTrack(int lines)
        {
            if (lines == 0)
                return;

            if (lines < 0)
            {
                ScrollLineUp((-1) * lines);
            }
            else
            {
                ScrollLineDown(lines);
            }
        }

        /// <summary>
        /// Performs a scroll to the given caretIndex
        /// </summary>
        /// <param name="caretIndex">The caret index to scroll to</param>
        public Point ScrollToCaret(int caretIndex, Point position)
        {
            if (position.Y < 0)
            {
                //Need to scroll up until caret is visible
                _vScrollPos -= Math.Abs((position.Y - _recContent.Y) / _recLineCount.Height) * _vScrollSmall;
                UpdateVisibleByteIndex();
                UpdateScrollValues();

                if (CaretFocused)
                {
                    position.Y = _recContent.Y;
                }
            }
            else if (position.Y > _maxVisibleBytesV * _recLineCount.Height)
            {
                //Need to scroll down until caret is visible
                _vScrollPos += ((position.Y) / _recLineCount.Height - (_maxVisibleBytesV - 1)) * _vScrollSmall;

                if (_vScrollPos > (_vScrollMax - (_vScrollLarge - 1)))
                    _vScrollPos = (_vScrollMax - (_vScrollLarge - 1));

                UpdateVisibleByteIndex();
                UpdateScrollValues();

                if (CaretFocused)
                {
                    position.Y = (_maxVisibleBytesV - 1) * _recLineCount.Height + _recContent.Y;
                }
            }
            return position;
        }

        #endregion

        #region Update Actions

        private void UpdateRectanglePositioning()
        {
            if (ClientRectangle.Width == 0)
                return;

            //Start by calculating the size of a char
            SizeF charSize;
            using (var graphics = this.CreateGraphics())
            {
                charSize = graphics.MeasureString("D", Font, 100, _stringFormat);
            }
            CharSize = new SizeF((float)Math.Ceiling(charSize.Width), (float)Math.Ceiling(charSize.Height));

            //Set the main content bounds (remove margins)
            _recContent = ClientRectangle;
            _recContent.X += Margin.Left;
            _recContent.Y += Margin.Top;
            _recContent.Width -= Margin.Right;
            _recContent.Height -= Margin.Bottom;

            //Check if the border is active and decrease the bounds
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                _recContent.X += 2;
                _recContent.Y += 2;
                _recContent.Width -= 1;
                _recContent.Height -= 1;
            }
            else if (BorderStyle == BorderStyle.FixedSingle)
            {
                _recContent.X += 1;
                _recContent.Y += 1;
                _recContent.Width -= 1;
                _recContent.Height -= 1;
            }

            //Handle if the scrollbar is visible
            if (!VScrollBarHidden)
            {
                _recContent.Width -= _vScrollBarWidth;
                _vScrollBar.Left = _recContent.X + _recContent.Width - Margin.Left;
                _vScrollBar.Top = _recContent.Y - Margin.Top;
                _vScrollBar.Width = _vScrollBarWidth;
                _vScrollBar.Height = _recContent.Height;
            }

            _recLineCount = new Rectangle(_recContent.X, _recContent.Y, (int)(_charSize.Width * 4), (int)(_charSize.Height) - 2);

            _editView.Update(_recLineCount.X + _recLineCount.Width + _entityMargin / 2, _recContent);

            //Calculate needed maximums for the bytes
            _maxBytesH = _bytesPerLine;
            _maxBytesV = (int)Math.Ceiling(((float)_recContent.Height / (float)_recLineCount.Height));
            _maxBytes = _maxBytesH * _maxBytesV;
            _maxVisibleBytesV = (int)Math.Floor(((float)_recContent.Height / (float)_recLineCount.Height));

            UpdateScrollBarSize();
        }

        private void UpdateVisibleByteIndex()
        {
            if (_hexTable.Length == 0)
            {
                _firstByte = 0;
                _lastByte = 0;
            }
            else
            {
                _firstByte = _vScrollPos * _maxBytesH;
                _lastByte = (int)Math.Min(HexTableLength, _firstByte + _maxBytes);
            }
        }

        private void UpdateScrollValues()
        {
            if (!_isVScrollHidden)
            {
                _vScrollBar.Minimum = _vScrollMin;
                _vScrollBar.Maximum = _vScrollMax;
                _vScrollBar.Value = _vScrollPos;

                _vScrollBar.SmallChange = _vScrollSmall;
                _vScrollBar.LargeChange = _vScrollLarge;

                _vScrollBar.Visible = true;
            }
            else
            {
                _vScrollBar.Visible = false;
            }
        }

        private void UpdateScrollBarSize()
        {
            if (VScrollBarVisisble && _maxVisibleBytesV > 0 && _maxBytesH > 0)
            {
                //Holds the size of a page (number of rows per page)
                int largeScroll = _maxVisibleBytesV;
                //Holds the row size (1)
                int smallScroll = 1;
                //Holds the minimum value of the scrollbar
                int minScroll = 0;
                //Holds the maximum value of the scrollbar
                int maxScroll = HexTableLength / _maxBytesH;
                //Holds the current positon on the scrollbar
                int posScroll = _firstByte / _maxBytesH;

                if (largeScroll != _vScrollLarge || smallScroll != _vScrollSmall)
                {
                    _vScrollLarge = largeScroll;
                    _vScrollSmall = smallScroll;
                }

                if (maxScroll >= largeScroll)
                {
                    if (maxScroll != _vScrollMax || posScroll != _vScrollPos)
                    {
                        _vScrollMin = minScroll;
                        _vScrollMax = maxScroll;
                        _vScrollPos = posScroll;
                    }

                    VScrollBarHidden = false;

                    UpdateScrollValues();
                }
                else
                {
                    VScrollBarHidden = true;
                }
            }
            else
            {
                VScrollBarHidden = true;
            }
        }

        #endregion

        #region Constructor

        public HexEditor()
            : this(new ByteCollection())
        { }

        public HexEditor(ByteCollection collection)
        {
            //Set String format for the hex values
            _stringFormat = new StringFormat(StringFormat.GenericTypographic);
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;

            //Set the provided byte collection
            _hexTable = collection;

            //Set the vertical scrollbar
            _vScrollBar = new VScrollBar();
            _vScrollBar.Scroll += new ScrollEventHandler(OnVScrollBarScroll);

            //Redraw whenever the control is resized
            SetStyle(ControlStyles.ResizeRedraw, true);

            //Enable double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable selectable
            SetStyle(ControlStyles.Selectable, true);

            //Handle initialization of Caret
            _caret = new Caret(this);
            _caret.SelectionStartChanged += new EventHandler(CaretSelectionStartChanged);
            _caret.SelectionLengthChanged += new EventHandler(CaretSelectionLengthChanged);

            //Create the needed edit view
            _editView = new EditView(this);
            _handler = _editView;

            //Set defualt cursor
            this.Cursor = Cursors.IBeam;
        }

        #endregion

        #region Boundary Calculator

        private RectangleF GetLineCountBound(int index)
        {
            RectangleF ret = new RectangleF(
                _recLineCount.X,
                _recLineCount.Y + (_recLineCount.Height * index),
                _recLineCount.Width,
                _recLineCount.Height
                );

            return ret;
        }

        #endregion

        #region OnPaint - Functions

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

            if (BorderStyle == BorderStyle.Fixed3D)
            {
                SolidBrush brush = new SolidBrush(BackColor);
                Rectangle rect = ClientRectangle;
                pevent.Graphics.FillRectangle(brush, rect);
                ControlPaint.DrawBorder3D(pevent.Graphics, ClientRectangle, Border3DStyle.Sunken);
            }
            else if (BorderStyle == BorderStyle.FixedSingle)
            {
                SolidBrush brush = new SolidBrush(BackColor);
                Rectangle rect = ClientRectangle;
                pevent.Graphics.FillRectangle(brush, rect);
                ControlPaint.DrawBorder(pevent.Graphics, ClientRectangle, BorderColor, ButtonBorderStyle.Solid);
            }
            else
            {
                base.OnPaintBackground(pevent);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Region r = new Region(ClientRectangle);
            r.Exclude(_recContent);
            e.Graphics.ExcludeClip(r);

            UpdateVisibleByteIndex();

            PaintLineCount(e.Graphics, _firstByte, _lastByte);

            _editView.Paint(e.Graphics, _firstByte, _lastByte);
        }

        private void PaintLineCount(Graphics g, int startIndex, int lastIndex)
        {
            SolidBrush brush = new SolidBrush(ForeColor);

            for (int i = 0; ((i * _maxBytesH) + startIndex) <= lastIndex; i++)
            {
                RectangleF drawSurface = GetLineCountBound(i);
                string lineCount = (startIndex + (i * _maxBytesH)).ToString(_lineCountCaps);
                //Calculate how many '0' need to be added to the current count
                int zeros = _nrCharsLineCount - lineCount.Length;

                string lineStr;
                if (zeros > -1)
                {
                    lineStr = new string('0', zeros) + lineCount;
                }
                else
                {
                    lineStr = new string('~', _nrCharsLineCount);
                }
                g.DrawString(lineStr, Font, brush, drawSurface, _stringFormat);
            }
        }

        #endregion

        #region OnRezie

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateRectanglePositioning();
            Invalidate();
        }

        #endregion
    }
}
