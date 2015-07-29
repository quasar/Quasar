// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

namespace xServer.Core.MouseKeyHook.Implementation
{
    internal class AppEventFacade : EventFacade
    {
        protected override MouseListener CreateMouseListener()
        {
            return new AppMouseListener();
        }

        protected override KeyListener CreateKeyListener()
        {
            return new AppKeyListener();
        }
    }
}