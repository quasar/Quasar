// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Windows.Forms;
using xClient.Core.MouseKeyHook.WinApi;

namespace xClient.Core.MouseKeyHook.Implementation
{
    internal abstract class MouseListener : BaseListener, IMouseEvents
    {
        private readonly ButtonSet m_DoubleDown;
        private readonly ButtonSet m_SingleDown;
        private Point m_PreviousPosition;

        protected MouseListener(Subscribe subscribe)
            : base(subscribe)
        {
            m_PreviousPosition = new Point(-1, -1);
            m_DoubleDown = new ButtonSet();
            m_SingleDown = new ButtonSet();
        }

        protected override bool Callback(CallbackData data)
        {
            var e = GetEventArgs(data);

            if (e.IsMouseKeyDown)
            {
                ProcessDown(ref e);
            }

            if (e.IsMouseKeyUp)
            {
                ProcessUp(ref e);
            }

            if (e.WheelScrolled)
            {
                ProcessWheel(ref e);
            }

            if (HasMoved(e.Point))
            {
                ProcessMove(ref e);
            }

            return !e.Handled;
        }

        protected abstract MouseEventExtArgs GetEventArgs(CallbackData data);

        protected virtual void ProcessWheel(ref MouseEventExtArgs e)
        {
            OnWheel(e);
        }

        protected virtual void ProcessDown(ref MouseEventExtArgs e)
        {
            OnDown(e);
            OnDownExt(e);
            if (e.Handled)
            {
                return;
            }

            if (e.Clicks == 2)
            {
                m_DoubleDown.Add(e.Button);
            }

            if (e.Clicks == 1)
            {
                m_SingleDown.Add(e.Button);
            }
        }

        protected virtual void ProcessUp(ref MouseEventExtArgs e)
        {
            if (m_SingleDown.Contains(e.Button))
            {
                OnUp(e);
                OnUpExt(e);
                if (e.Handled)
                {
                    return;
                }
                OnClick(e);
                m_SingleDown.Remove(e.Button);
            }

            if (m_DoubleDown.Contains(e.Button))
            {
                e = e.ToDoubleClickEventArgs();
                OnUp(e);
                OnDoubleClick(e);
                m_DoubleDown.Remove(e.Button);
            }
        }

        private void ProcessMove(ref MouseEventExtArgs e)
        {
            m_PreviousPosition = e.Point;

            OnMove(e);
            OnMoveExt(e);
        }

        private bool HasMoved(Point actualPoint)
        {
            return m_PreviousPosition != actualPoint;
        }

        public event MouseEventHandler MouseMove;
        public event EventHandler<MouseEventExtArgs> MouseMoveExt;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDown;
        public event EventHandler<MouseEventExtArgs> MouseDownExt;
        public event MouseEventHandler MouseUp;
        public event EventHandler<MouseEventExtArgs> MouseUpExt;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler MouseDoubleClick;

        protected virtual void OnMove(MouseEventArgs e)
        {
            var handler = MouseMove;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnMoveExt(MouseEventExtArgs e)
        {
            var handler = MouseMoveExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnClick(MouseEventArgs e)
        {
            var handler = MouseClick;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDown(MouseEventArgs e)
        {
            var handler = MouseDown;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDownExt(MouseEventExtArgs e)
        {
            var handler = MouseDownExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUp(MouseEventArgs e)
        {
            var handler = MouseUp;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUpExt(MouseEventExtArgs e)
        {
            var handler = MouseUpExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnWheel(MouseEventArgs e)
        {
            var handler = MouseWheel;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDoubleClick(MouseEventArgs e)
        {
            var handler = MouseDoubleClick;
            if (handler != null) handler(this, e);
        }
    }
}