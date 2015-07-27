namespace xServer.Core.Helper
{
    public static class NativeMethodsHelper
    {
        public static int MakeLong(int wLow, int wHigh)
        {
            int low = (int)IntLoWord(wLow);
            short high = IntLoWord(wHigh);
            int product = 0x10000 * (int)high;
            int mkLong = (int)(low | product);
            return mkLong;
        }

        private static short IntLoWord(int word)
        {
            return (short)(word & short.MaxValue);
        }
    }
}
