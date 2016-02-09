using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls.HexEditor
{
    public class StringViewHandler
    {
        #region Field

        /// <summary>
        /// Contains the boundary of
        /// a single line
        /// </summary>
        Rectangle _recStringView;

        /// <summary>
        /// Contains the format of the 
        /// string to be used in the
        /// string view
        /// </summary>
        StringFormat _stringFormat;

        private HexEditor _editor;

        #endregion

        #region Properties

        public int MaxWidth
        {
            get { return _recStringView.X + _recStringView.Width; }
        }

        #endregion

        #region Constructor

        public StringViewHandler(HexEditor editor)
        {
            _editor = editor;

            //Set String format for the values
            _stringFormat = new StringFormat(StringFormat.GenericTypographic);
            _stringFormat.Alignment = StringAlignment.Center;
            _stringFormat.LineAlignment = StringAlignment.Center;
        }

        #endregion

        #region KeyMouseEvents

        #region Key

        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar))
            {
                HandleUserInput(e.KeyChar);
            }
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
                    _editor.RemoveByteAt(index);
                    Point newLocation = GetCaretLocation(index);
                    _editor.SetCaretStart(index, newLocation);
                }
            }
            else if (e.KeyCode == Keys.Up && (_editor.CaretIndex - _editor.BytesPerLine) >= 0)
            {
                int index = _editor.CaretIndex - _editor.BytesPerLine;

                //Check ig caret is att the end of the line
                if (index % _editor.BytesPerLine == 0 && _editor.CaretPosX >= _recStringView.X + _recStringView.Width)
                {
                    Point position = new Point(_editor.CaretPosX, _editor.CaretPosY - _recStringView.Height);

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
                    Point position = new Point(_editor.CaretPosX, _editor.CaretPosY + _recStringView.Height);

                    if (e.Shift)
                        _editor.SetCaretEnd(index, position);
                    else
                        _editor.SetCaretStart(index, position);
                }
            }
            else if (e.KeyCode == Keys.Left && (_editor.CaretIndex - 1) >= 0)
            {
                int index = _editor.CaretIndex - 1;
                HandleArrowKeys(index, e.Shift);
            }
            else if (e.KeyCode == Keys.Right && (_editor.CaretIndex + 1) <= _editor.LastVisibleByte)
            {
                int index = _editor.CaretIndex + 1;
                HandleArrowKeys(index, e.Shift);
            }
        }

        public void HandleArrowKeys(int index, bool isShiftDown)
        {
            Point newLocation = GetCaretLocation(index);
            if (isShiftDown)
                _editor.SetCaretEnd(index, newLocation);
            else
                _editor.SetCaretStart(index, newLocation);
        }

        #endregion

        #region Mouse

        public void OnMouseDown(int x, int y)
        {
            int iX = (x - _recStringView.X) / (int)_editor.CharSize.Width;
            int iY = (y - _recStringView.Y) / _recStringView.Height;

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
            int index = Math.Min(_editor.LastVisibleByte, _editor.FirstVisibleByte + iX + iY * _editor.BytesPerLine);

            int xPos = (iX * (int)_editor.CharSize.Width) + _recStringView.X;
            int yPos = (iY * _recStringView.Height) + _recStringView.Y;

            _editor.SetCaretStart(index, new Point(xPos, yPos));
        }

        public void OnMouseDragged(int x, int y)
        {
            int iX = (x - _recStringView.X) / (int)_editor.CharSize.Width;
            int iY = (y - _recStringView.Y) / _recStringView.Height;

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
            int index = Math.Min(_editor.LastVisibleByte, _editor.FirstVisibleByte + iX + iY * _editor.BytesPerLine);

            int xPos = (iX * (int)_editor.CharSize.Width) + _recStringView.X;
            int yPos = (iY * _recStringView.Height) + _recStringView.Y;

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

        #region Focus

        public void Focus()
        {
            int index = _editor.CaretIndex;
            Point location = GetCaretLocation(index);
            _editor.SetCaretStart(index, location);
        }

        #endregion

        #endregion

        #region Paint

        public void Update(int startPositionX, Rectangle area)
        {
            _recStringView = new Rectangle(
                startPositionX,
                area.Y,
                (int)(_editor.CharSize.Width * _editor.BytesPerLine),
                (int)(_editor.CharSize.Height) - 2
            );

            _recStringView.X += _editor.EntityMargin;
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
            char value = _editor.GetByteAsChar(index);
            string strValue = (Char.IsControl(value) ? "." : value.ToString());

            g.FillRectangle(backBrush, drawSurface);
            g.DrawString(strValue, _editor.Font, textBrush, drawSurface, _stringFormat);
        }

        private void PaintByte(Graphics g, Point point, int index)
        {
            SolidBrush brush = new SolidBrush(_editor.ForeColor);
            RectangleF drawLocation = GetBound(point);
            char value = _editor.GetByteAsChar(index);
            string strValue = (Char.IsControl(value) ? "." : value.ToString());

            g.DrawString(strValue, _editor.Font, brush, drawLocation, _stringFormat);
        }

        #endregion

        #region Caret

        /// <summary>
        /// Get the caret current location
        /// in the given bound.
        /// </summary>
        private Point GetCaretLocation(int index)
        {
            int xPos = _recStringView.X + ((int)_editor.CharSize.Width * (index % _editor.BytesPerLine));
            int yPos = _recStringView.Y + ((int)_recStringView.Height * ((index - (_editor.FirstVisibleByte + index % _editor.BytesPerLine)) / _editor.BytesPerLine));

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

            HandleUserRemove();

            byte newByte = Convert.ToByte(key);

            if (_editor.HexTableLength <= 0)
                _editor.AppendByte(newByte);
            else
                _editor.InsertByte(_editor.CaretIndex, newByte);

            int index = _editor.CaretIndex + 1;
            Point newLocation = GetCaretLocation(index);
            _editor.SetCaretStart(index, newLocation);
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
                _recStringView.X + (point.X * (int)_editor.CharSize.Width),
                _recStringView.Y + (point.Y * _recStringView.Height),
                _editor.CharSize.Width,
                _recStringView.Height
                );

            return ret;
        }

        #endregion
    }
}
