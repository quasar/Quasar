// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System.Windows.Forms;

namespace xServer.Core.MouseKeyHook.Implementation
{
    internal class ButtonSet
    {
        private MouseButtons m_Set;

        public ButtonSet()
        {
            m_Set = MouseButtons.None;
        }

        public void Add(MouseButtons element)
        {
            m_Set |= element;
        }

        public void Remove(MouseButtons element)
        {
            m_Set &= ~element;
        }

        public bool Contains(MouseButtons element)
        {
            return (m_Set & element) != MouseButtons.None;
        }
    }
}