using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls.HexEditor
{
    public class HexViewHandler
    {
        #region Fields

        bool _isEditing;

        /// <summary>
        /// Contains info about how to 
        /// present the hex values 
        /// (Upper or Lower case)
        /// </summary>
        string _hexType = "X2";

        /// <summary>
        /// Contains the boundary for one single
        /// hexa value that is visible
        /// </summary>
        Rectangle _recHexValue;

        /// <summary>
        /// Contains the format of the hexadecimal
        /// strings that are presented
        /// </summary>
        StringFormat _stringFormat;

        private HexEditor _editor;

        #endregion

        #region Properties

        public int MaxWidth
        {
            get { return _recHexValue.X + (_recHexValue.Width * _editor.BytesPerLine); }
        }

        #endregion

        #region Constructor

        public HexViewHandler(HexEditor editor)
        {
            _editor = editor;

            //Set String format for the hex values
            _stringFormat = new StringFormat(StringFormat.GenericTypographic);
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;
        }

        #endregion

        #region Method

        #region KeyMouseEvents

        #region KeyEvents

        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (IsHex(e.KeyChar))
                HandleUserInput(e.KeyChar);
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (_editor.SelectionLength > 0)
                {
                    //Remove the selected bytes
                    HandleUserRemove();
                    int index = _editor.CaretIndex;
                    Point newLocation = GetCaretLocation(index);
                    _editor.SetCaretStart(index, newLocation);
                }
                else if (_editor.CaretIndex < _editor.LastVisibleByte && e.KeyCode == Keys.Delete)
                {
                    //Remove the byte after the caret
                    _editor.RemoveByteAt(_editor.CaretIndex);
                    Point newLocation = GetCaretLocation(_editor.CaretIndex);
                    _editor.SetCaretStart(_editor.CaretIndex, newLocation);
                }
                else if (_editor.CaretIndex > 0 && e.KeyCode == Keys.Back)
                {
                    //Remove byte before the caret
                    int index = _editor.CaretIndex - 1;
                    if (_isEditing)
                    {
                        //Remove the byte that is being edited
                        index = _editor.CaretIndex;
                    }
                    _editor.RemoveByteAt(index);
                    Point newLocation = GetCaretLocation(index);
                    _editor.SetCaretStart(index, newLocation);
                }
                _isEditing = false;
            }
            else if (e.KeyCode == Keys.Up && (_editor.CaretIndex - _editor.BytesPerLine) >= 0)
            {
                int index = _editor.CaretIndex - _editor.BytesPerLine;

                //Check ig caret is att the end of the line
                if (index % _editor.BytesPerLine == 0 && _editor.CaretPosX >= _recHexValue.X + _recHexValue.Width * _editor.BytesPerLine)
                {
                    Point position = new Point(_editor.CaretPosX, _editor.CaretPosY - _recHexValue.Height);

                    //check that this is not the last row (nothing above)
                    if (index == 0)
                    {
                        //Last row do not change index and position
                        position = new Point(_editor.CaretPosX, _editor.CaretPosY);
                        index = _editor.BytesPerLine;
                    }

                    if (e.Shift)
                        _editor.SetCaretEnd(index, position);
                    else
                        _editor.SetCaretStart(index, position);
                    _isEditing = false;
                }
                else
                {
                    HandleArrowKeys(index, e.Shift);
                }
            }
            else if (e.KeyCode == Keys.Down && (_editor.CaretIndex - 1) / _editor.BytesPerLine < _editor.HexTableLength / _editor.BytesPerLine)
            {
                int index = _editor.CaretIndex + _editor.BytesPerLine;

                if (index > _editor.HexTableLength)
                {
                    index = _editor.HexTableLength;
                    HandleArrowKeys(index, e.Shift);
                }
                else
                {
                    Point position = new Point(_editor.CaretPosX, _editor.CaretPosY + _recHexValue.Height);

                    if (e.Shift)
                        _editor.SetCaretEnd(index, position);
                    else
                        _editor.SetCaretStart(index, position);
                    _isEditing = false;
                }
            }
            else if (e.KeyCode == Keys.Left && (_editor.CaretIndex - 1) >= 0)
            {
                int index = _editor.CaretIndex - 1;
                HandleArrowKeys(index, e.Shift);
            }
            else if (e.KeyCode == Keys.Right && (_editor.CaretIndex + 1) <= _editor.HexTableLength)
            {
                int index = _editor.CaretIndex + 1;
                HandleArrowKeys(index, e.Shift);
            }
        }

        public void HandleArrowKeys(int index, bool isShiftDown)
        {
            Point position = GetCaretLocation(index);

            if (isShiftDown)
                _editor.SetCaretEnd(index, position);
            else
                _editor.SetCaretStart(index, position);
            _isEditing = false;
        }

        #endregion

        #region MouseEvent

        public void OnMouseDown(int x, int y)
        {
            int iX = (x - _recHexValue.X) / _recHexValue.Width;
            int iY = (y - _recHexValue.Y) / _recHexValue.Height;

            //Check that values are good
            iX = iX > _editor.BytesPerLine ? _editor.BytesPerLine : iX;
            iX = iX < 0 ? 0 : iX;
            iY = iY > _editor.MaxBytesV ? _editor.MaxBytesV : iY;
            iY = iY < 0 ? 0 : iY;

            //Make sure values are withing the given bounds
            if ((_editor.LastVisibleByte - _editor.FirstVisibleByte) / _editor.BytesPerLine <= iY)
            {
                //Check that column is not greater than max
                if ((_editor.LastVisibleByte - _editor.FirstVisibleByte) % _editor.BytesPerLine <= iX)
                {
                    iX = (_editor.LastVisibleByte - _editor.FirstVisibleByte) % _editor.BytesPerLine;
                }
                iY = (_editor.LastVisibleByte - _editor.FirstVisibleByte) / _editor.BytesPerLine;
            }

            //Get the smallest possible location (do not want to exceed the max)
            int index = Math.Min(_editor.LastVisibleByte, _editor.FirstVisibleByte + iX + (iY * _editor.BytesPerLine));

            int xPos = (iX * _recHexValue.Width) + _recHexValue.X;
            int yPos = (iY * _recHexValue.Height) + _recHexValue.Y;

            _editor.SetCaretStart(index, new Point(xPos, yPos));
            _isEditing = false;
        }

        public void OnMouseDragged(int x, int y)
        {
            int iX = (x - _recHexValue.X) / _recHexValue.Width;
            int iY = (y - _recHexValue.Y) / _recHexValue.Height;

            //Check that values are good
            iX = iX > _editor.BytesPerLine ? _editor.BytesPerLine : iX;
            iX = iX < 0 ? 0 : iX;
            iY = iY > _editor.MaxBytesV ? _editor.MaxBytesV : iY;

            if (_editor.FirstVisibleByte > 0)
            {
                iY = iY < 0 ? -1 : iY;
            }
            else
            {
                iY = iY < 0 ? 0 : iY;
            }

            //Make sure values are withing the given bounds
            if ((_editor.LastVisibleByte - _editor.FirstVisibleByte) / _editor.BytesPerLine <= iY)
            {
                //Check that column is not greater than max
                if ((_editor.LastVisibleByte - _editor.FirstVisibleByte) % _editor.BytesPerLine <= iX)
                {
                    iX = (_editor.LastVisibleByte - _editor.FirstVisibleByte) % _editor.BytesPerLine;
                }
                iY = (_editor.LastVisibleByte - _editor.FirstVisibleByte) / _editor.BytesPerLine;
            }

            //Get the smallest possible location (do not want to exceed the max)
            int index = Math.Min(_editor.LastVisibleByte, _editor.FirstVisibleByte + iX + (iY * _editor.BytesPerLine));

            int xPos = (iX * _recHexValue.Width) + _recHexValue.X;
            int yPos = (iY * _recHexValue.Height) + _recHexValue.Y;

            _editor.SetCaretEnd(index, new Point(xPos, yPos));
        }

        public void OnMouseDoubleClick()
        {
            if (_editor.CaretIndex < _editor.LastVisibleByte)
            {
                int index = _editor.CaretIndex + 1;
                Point newLocation = GetCaretLocation(index);
                _editor.SetCaretEnd(index, newLocation);
            }
        }

        #endregion

        #endregion

        #region PaintMethod

        public void Update(int startPositionX, Rectangle area)
        {
            _recHexValue = new Rectangle(
                startPositionX,
                area.Y,
                (int)(_editor.CharSize.Width * 3),
                (int)(_editor.CharSize.Height) - 2
            );

            _recHexValue.X += _editor.EntityMargin;
        }

        public void Paint(Graphics g, int index, int startIndex)
        {
            Point columnAndRow = GetByteColumnAndRow(index);

            if (_editor.IsSelected(index + startIndex))
            {
                PaintByteAsSelected(g, columnAndRow, (index + startIndex));
            }
            else
            {
                PaintByte(g, columnAndRow, (index + startIndex));
            }
        }

        private void PaintByteAsSelected(Graphics g, Point point, int index)
        {
            SolidBrush backBrush = new SolidBrush(_editor.SelectionBackColor);
            SolidBrush textBrush = new SolidBrush(_editor.SelectionForeColor);
            RectangleF drawSurface = GetBound(point);
            string hexValue = _editor.GetByte(index).ToString(_hexType);

            g.FillRectangle(backBrush, drawSurface);
            g.DrawString(hexValue, _editor.Font, textBrush, drawSurface, _stringFormat);
        }

        private void PaintByte(Graphics g, Point point, int index)
        {
            SolidBrush brush = new SolidBrush(_editor.ForeColor);
            RectangleF drawSurface = GetBound(point);
            string hexValue = _editor.GetByte(index).ToString(_hexType);

            g.DrawString(hexValue, _editor.Font, brush, drawSurface, _stringFormat);
        }

        #endregion

        public void SetLowerCase()
        {
            _hexType = "x2";
        }

        public void SetUpperCase()
        {
            _hexType = "X2";
        }

        public void Focus()
        {
            int index = _editor.CaretIndex;
            Point location = GetCaretLocation(index);
            _editor.SetCaretStart(index, location);
        }

        #endregion

        #region Caret

        /// <summary>
        /// Get the caret current location
        /// in the given bound.
        /// </summary>
        private Point GetCaretLocation(int index)
        {
            int xPos = _recHexValue.X + (_recHexValue.Width * (index % _editor.BytesPerLine));
            int yPos = _recHexValue.Y + (_recHexValue.Height * ((index - (_editor.FirstVisibleByte + index % _editor.BytesPerLine)) / _editor.BytesPerLine));

            Point ret = new Point(xPos, yPos);
            return ret;
        }

        #endregion

        #region Misc

        private void HandleUserRemove()
        {
            //Calculate where to position the caret after the removal
            int index = _editor.SelectionStart;
            Point position = GetCaretLocation(index);
            //Remove all of the selected bytes
            _editor.RemoveSelectedBytes();

            //Set the new position of the caret
            _editor.SetCaretStart(index, position);
        }

        private void HandleUserInput(char key)
        {
            if (!_editor.CaretFocused)
                return;

            //Perform overwrite
            HandleUserRemove();

            if (_isEditing)
            {
                //Editing has already started, should change the second nibble
                _isEditing = false;
                //Load old bytes to allow change
                byte oldByte = _editor.GetByte(_editor.CaretIndex);
                //Append the new nibble
                oldByte += Convert.ToByte(key.ToString(), 16);
                _editor.SetByte(_editor.CaretIndex, oldByte);
                //Relocate the caret
                int index = _editor.CaretIndex + 1;
                Point newLocation = GetCaretLocation(index);
                _editor.SetCaretStart(index, newLocation);

            }
            else
            {
                //Begin new edit phase
                _isEditing = true;
                string hexByte = key.ToString() + "0";
                byte newByte = Convert.ToByte(hexByte, 16);

                if (_editor.HexTable.Length <= 0)
                {
                    _editor.AppendByte(newByte);
                }
                else
                {
                    _editor.InsertByte(_editor.CaretIndex, newByte);
                }

                //Relocate the caret to the middle of the hex value (provide illusion of editing the second value)
                int xPos = (_recHexValue.X + (_recHexValue.Width * ((_editor.CaretIndex) % _editor.BytesPerLine)) + (_recHexValue.Width / 2));
                int yPos = _recHexValue.Y + (_recHexValue.Height * ((_editor.CaretIndex - (_editor.FirstVisibleByte + _editor.CaretIndex % _editor.BytesPerLine)) / _editor.BytesPerLine));

                _editor.SetCaretStart(_editor.CaretIndex, new Point(xPos, yPos));
            }
        }

        private Point GetByteColumnAndRow(int index)
        {
            int column = index % _editor.BytesPerLine;
            int row = index / _editor.BytesPerLine;

            Point ret = new Point(column, row);
            return ret;
        }

        private RectangleF GetBound(Point point)
        {
            RectangleF ret = new RectangleF(
                _recHexValue.X + (point.X * _recHexValue.Width),
                _recHexValue.Y + (point.Y * _recHexValue.Height),
                _recHexValue.Width,
                _recHexValue.Height
                );

            return ret;
        }

        private bool IsHex(char c)
        {
            return (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F') ||
                        Char.IsDigit(c);
        }

        #endregion
    }
}
