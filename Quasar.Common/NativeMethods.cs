using System;
using System.Runtime.InteropServices;

namespace Quasar.Common
{
    public class NativeMethods
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int memcmp(byte* ptr1, byte* ptr2, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(IntPtr ptr1, IntPtr ptr2, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcpy(IntPtr dst, IntPtr src, uint count);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int memcpy(void* dst, void* src, uint count);
    }
}
