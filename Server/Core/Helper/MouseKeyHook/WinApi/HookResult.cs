// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;

namespace xServer.Core.MouseKeyHook.WinApi
{
    internal class HookResult : IDisposable
    {
        private readonly HookProcedureHandle m_Handle;
        private readonly HookProcedure m_Procedure;

        public HookResult(HookProcedureHandle handle, HookProcedure procedure)
        {
            m_Handle = handle;
            m_Procedure = procedure;
        }

        public HookProcedureHandle Handle
        {
            get { return m_Handle; }
        }

        public HookProcedure Procedure
        {
            get { return m_Procedure; }
        }

        public void Dispose()
        {
            m_Handle.Dispose();
        }
    }
}