using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls.HexEditor
{
    public class EditView : IKeyMouseEventHandler
    {
        #region Fields

        /// <summary>
        /// Contains the handler for the hex
        /// view.
        /// </summary>
        private HexViewHandler _hexView;

        /// <summary>
        /// Contains the handler for the 
        /// string view
        /// </summary>
        private StringViewHandler _stringView;

        private HexEditor _editor;

        #endregion

        #region Contructor

        public EditView(HexEditor editor)
        {
            _editor = editor;
            _hexView = new HexViewHandler(editor);
            _stringView = new StringViewHandler(editor);
        }

        #endregion

        #region KeyMouseEvent

        #region Key

        public void OnKeyPress(KeyPressEventArgs e)
        {
            if (InHexView(_editor.CaretPosX))
            {
                _hexView.OnKeyPress(e);
            }
            else
            {
                _stringView.OnKeyPress(e);
            }
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (InHexView(_editor.CaretPosX))
            {
                _hexView.OnKeyDown(e);
            }
            else
            {
                _stringView.OnKeyDown(e);
            }
        }

        public void OnKeyUp(KeyEventArgs e)
        { /* ... */ }

        #endregion

        #region Mouse

        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (InHexView(e.X))
                {
                    _hexView.OnMouseDown(e.X, e.Y);
                }
                else
                {
                    _stringView.OnMouseDown(e.X, e.Y);
                }
            }
        }

        public void OnMouseDragged(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (InHexView(e.X))
                {
                    _hexView.OnMouseDragged(e.X, e.Y);
                }
                else
                {
                    _stringView.OnMouseDragged(e.X, e.Y);
                }
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        { /* ... */ }

        public void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (InHexView(e.X))
                {
                    _hexView.OnMouseDoubleClick();
                }
                else
                {
                    _stringView.OnMouseDoubleClick();
                }
            }
        }

        #endregion

        #region Focus

        public void OnGotFocus(EventArgs e)
        {
            if (InHexView(_editor.CaretPosX))
                _hexView.Focus();
            else
                _stringView.Focus();
        }

        #endregion

        #endregion

        #region UpdateActions
        public void SetLowerCase()
        {
            _hexView.SetLowerCase();
        }

        public void SetUpperCase()
        {
            _hexView.SetUpperCase();
        }

        public void Update(int startPositionX, Rectangle area)
        {
            _hexView.Update(startPositionX, area);
            _stringView.Update(_hexView.MaxWidth, area);
        }
        #endregion

        #region PaintActions

        public void Paint(Graphics g, int startIndex, int endIndex)
        {
            for (int i = 0; (i + startIndex) < endIndex; i++)
            {
                _hexView.Paint(g, i, startIndex);
                _stringView.Paint(g, i, startIndex);
            }
        }

        #endregion

        #region Misc

        private bool InHexView(int x)
        {
            return (x < (_hexView.MaxWidth + _editor.EntityMargin - 2));
        }

        #endregion
    }
}
